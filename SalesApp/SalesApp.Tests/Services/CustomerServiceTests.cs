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
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _mockRepo = new Mock<ICustomerRepository>();
            _mapper = MapperHelper.GetMapper();
            _service = new CustomerService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsCustomerDTO_WhenCustomerExists()
        {
            // Arrange
            var customerId = 1;
            var customer = new Customer
            {
                CustomerId = customerId,
                CustomerName = "Test Customer"
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            // Act
            var result = await _service.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(customer.CustomerName, result.CustomerName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ReturnsNull_WhenCustomerDoesNotExist()
        {
            // Arrange
            var customerId = 1;
            _mockRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            // Act
            var result = await _service.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Customer 1" },
                new Customer { CustomerId = 2, CustomerName = "Customer 2" },
                new Customer { CustomerId = 3, CustomerName = "Customer 3" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(customers);

            // Act
            var result = await _service.GetAllCustomersAsync();

            // Assert
            var customerList = result.ToList();
            Assert.Equal(3, customerList.Count);
            Assert.Equal("Customer 1", customerList[0].CustomerName);
            Assert.Equal("Customer 2", customerList[1].CustomerName);
            Assert.Equal("Customer 3", customerList[2].CustomerName);
        }

        [Fact]
        public async Task CreateCustomerAsync_ReturnsNewCustomerId()
        {
            // Arrange
            var customerDto = new CreateCustomerDTO
            {
                CustomerName = "New Customer"
            };

            int newCustomerId = 1;

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync(newCustomerId);

            // Act
            var result = await _service.CreateCustomerAsync(customerDto);

            // Assert
            Assert.Equal(newCustomerId, result);
            _mockRepo.Verify(repo => repo.AddAsync(It.Is<Customer>(c =>
                c.CustomerName == customerDto.CustomerName)), Times.Once);
        }
    }
}
