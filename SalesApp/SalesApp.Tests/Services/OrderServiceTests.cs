using AutoMapper;
using Moq;
using SalesApp.DTOs;
using SalesApp.Models;
using SalesApp.Repositories.Interfaces;
using SalesApp.Services;
using SalesApp.Services.RabbitMQ;
using SalesApp.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SalesApp.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<ICustomerRepository> _mockCustomerRepo;
        private readonly Mock<IRabbitMQService> _mockRabbitMQService;
        private readonly IMapper _mapper;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockCustomerRepo = new Mock<ICustomerRepository>();
            _mockRabbitMQService = new Mock<IRabbitMQService>();
            _mapper = MapperHelper.GetMapper();

            _service = new OrderService(
                _mockOrderRepo.Object,
                _mockCustomerRepo.Object,
                _mapper,
                _mockRabbitMQService.Object);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrderDTO_WhenOrderExists()
        {
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

            var result = await _service.GetOrderByIdAsync(orderId);

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
            var orderId = 1;
            _mockOrderRepo.Setup(repo => repo.GetOrderWithDetailsAsync(orderId))
                .ReturnsAsync((Order)null);

            var result = await _service.GetOrderByIdAsync(orderId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
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

            var result = await _service.GetAllOrdersAsync();

            var orderList = result.ToList();
            Assert.Equal(2, orderList.Count);
            Assert.Equal(1, orderList[0].OrderId);
            Assert.Equal(2, orderList[1].OrderId);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ReturnsOrdersWithSpecifiedStatus()
        {
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

            var result = await _service.GetOrdersByStatusAsync(OrderStatus.Pending);

            var orderList = result.ToList();
            Assert.Equal(2, orderList.Count);
            Assert.All(orderList, item => Assert.Equal(OrderStatus.Pending.ToString(), item.OrderStatus));
        }

        [Fact]
        public async Task CreateOrderAsync_ReturnsNewOrderId_WhenCustomerExists()
        {
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
                .ReturnsAsync(newOrderId)
                .Callback<Order>(o => o.OrderId = newOrderId);

            var result = await _service.CreateOrderAsync(orderDto);

            Assert.Equal(newOrderId, result);

            _mockOrderRepo.Verify(repo => repo.AddAsync(It.Is<Order>(o =>
                o.CustomerId == customerId &&
                o.OrderItems.Count == 1 &&
                o.OrderTotal == 100.0m)), Times.Once);

            _mockRabbitMQService.Verify(svc =>
                svc.SendMessage(
                    It.IsAny<OrderCreatedEvent>(),
                    "order_queue"),
                Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsException_WhenCustomerDoesNotExist()
        {
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

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateOrderAsync(orderDto));
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_UpdatesStatus_WhenOrderExists()
        {
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

            await _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processed);

            _mockOrderRepo.Verify(repo => repo.UpdateAsync(It.Is<Order>(o =>
                o.OrderId == orderId &&
                o.OrderStatus == OrderStatus.Processed)), Times.Once);
        }

        [Fact]
        public async Task UpdateOrderStatusAsync_ThrowsException_WhenOrderDoesNotExist()
        {
            var orderId = 1;

            _mockOrderRepo.Setup(repo => repo.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateOrderStatusAsync(orderId, OrderStatus.Processed));
        }
    }
}