using AutoMapper;
using Moq;
using SalesApp.DTOs;
using SalesApp.Models;
using SalesApp.Repositories.Interfaces;
using SalesApp.Services;
using SalesApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesApp.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;
        private readonly IMapper _mapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockCustomerRepo = new Mock<ICustomerRepository>();
            _mapper = MapperHelper.GetMapper();
            _service = new OrderService(_mockOrderRepo.Object, _mockCustomerRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrderDTO_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var customer = new Customer { CustomerId = 1, CustomerName = "Test Customer" };
            var order = new Order
            {
                OrderId = orderId,
                CustomerId = customer.CustomerId,
                CustomerRefId = customer.CustomerId,
                Customer = customer,
                OrderDate = DateTime.Now,
                OrderStatus = OrderStatus.Pending,
                OrderTotal = 100.0m,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem
                    {
                        OrderItemId = 1,
                        OrderRefId = orderId,
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 50.0m
                    }
                }
            };

            _mockOrderRepo.Setup(repo => repo.GetOrderWithDetailsAsync(orderId))
                .ReturnsAsync(order);

            // Act
            var result = await _service.GetOrderByIdAsync(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal(customer.CustomerName, result.CustomerName);
            Assert.Equal(order.OrderTotal, result.OrderTotal);
            Assert.Equal(OrderStatus.Pending.ToString(), result.OrderStatus);
            Assert.Single(result.OrderItems);
            Assert.Equal("Test Product", result.OrderItems.First().ProductName);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;
            _mockOrderRepo.Setup(repo => repo.GetOrderWithDetailsAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act
            var result = await _service.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, CustomerName = "Test Customer" };
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    CustomerId = customer.CustomerId,
                    Customer = customer,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    OrderTotal = 100.0m
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = customer.CustomerId,
                    Customer = customer,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Processed,
                    OrderTotal = 200.0m
                }
            };

            _mockOrderRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(orders);

            // Act
            var result = await _service.GetAllOrdersAsync();

            // Assert
            var orderList = result.ToList();
            Assert.Equal(2, orderList.Count);
            Assert.Equal(1, orderList[0].OrderId);
            Assert.Equal(2, orderList[1].OrderId);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ReturnsOrdersWithSpecifiedStatus()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, CustomerName = "Test Customer" };
            var orders = new List<Order>
            {
                new Order
                {
                    OrderId = 1,
                    CustomerId = customer.CustomerId,
                    Customer = customer,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    OrderTotal = 100.0m
                },
                new Order
                {
                    OrderId = 2,
                    CustomerId = customer.CustomerId,
                    Customer = customer,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.Pending,
                    OrderTotal = 200.0m
                }
            };

            _mockOrderRepo.Setup(repo => repo.GetOrdersByStatusAsync(OrderStatus.Pending))
                .ReturnsAsync(orders);

            // Act
            var result = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);

            // Assert
            var orderList = result.ToList();
            Assert.Equal(2, orderList.Count);
            Assert.All(orderList, item => Assert.Equal(OrderStatus.Pending.ToString(), item.OrderStatus));
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsNewOrderId_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer { CustomerId = customerId, CustomerName = "Test Customer" };
            var orderDto = new CreateOrderDTO
            {
                CustomerRefId = customerId,
                OrderItems = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO
                    {
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 50.0m
                    }
                }
            };

            int newOrderId = 1;

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            _mockOrderRepo.Setup(repo => repo.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(newOrderId);

            // Act
            var result = await _service.CreateOrderAsync(orderDto);

            // Assert
            Assert.Equal(newOrderId, result);
            _mockOrderRepo.Verify(repo => repo.AddAsync(It.Is<Order>(o =>
                o.CustomerId == customerId &&
                o.OrderItems.Count == 1 &&
                o.OrderTotal == 100.0m)), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsException_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 1;
            var orderDto = new CreateOrderDTO
            {
                CustomerRefId = customerId,
                OrderItems = new List<CreateOrderItemDTO>
                {
                    new CreateOrderItemDTO
                    {
                        ProductName = "Test Product",
                        Quantity = 2,
                        UnitPrice = 50.0m
                    }
                }
            };

            _mockCustomerRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrderAsync(orderDto));
            Assert.Equal("Cliente não encontrado", exception.Message);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesStatus_WhenOrderExists()
        {
            // Arrange
            var orderId = 1;
            var order = new Order
            {
                OrderId = orderId,
                OrderStatus = OrderStatus.Pending
            };

            _mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _mockOrderRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processed);

            // Assert
            _mockOrderRepo.Verify(repo => repo.UpdateAsync(It.Is<Order>(o =>
                o.OrderId == orderId &&
                o.OrderStatus == OrderStatus.Processed)), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsException_WhenOrderDoesNotExist()
        {
            // Arrange
            var orderId = 1;

            _mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processed));
            Assert.Equal("Pedido não encontrado", exception.Message);
        }
    }
}
