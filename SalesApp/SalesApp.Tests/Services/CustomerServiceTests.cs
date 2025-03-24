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
            var customerId = 1;
            var customer = new Customer
            {
                CustomerId = customerId,
                CustomerName = "Test Customer"
            };

            _mockRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync(customer);

            var result = await _service.GetCustomerByIdAsync(customerId);

            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
            Assert.Equal(customer.CustomerName, result.CustomerName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ThrowsKeyNotFoundException_WhenCustomerDoesNotExist()
        {
            var customerId = 1;

            _mockRepo.Setup(repo => repo.GetByIdAsync(customerId))
                .ReturnsAsync((Customer)null);

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetCustomerByIdAsync(customerId));

            Assert.Equal($"Cliente não encontrado com ID: {customerId}", exception.Message);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, CustomerName = "Customer 1" },
                new Customer { CustomerId = 2, CustomerName = "Customer 2" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(customers);

            var result = await _service.GetAllCustomersAsync();

            var customerList = Assert.IsAssignableFrom<IEnumerable<CustomerDTO>>(result);
            Assert.Equal(2, customerList.Count());
        }

        [Fact]
        public async Task CreateCustomerAsync_ReturnsNewCustomerId()
        {
            var customerDto = new CreateCustomerDTO
            {
                CustomerName = "New Customer"
            };

            var customerId = 1;

            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Customer>()))
                .ReturnsAsync(customerId);

            var result = await _service.CreateCustomerAsync(customerDto);

            Assert.Equal(customerId, result);
            _mockRepo.Verify(repo => repo.AddAsync(It.Is<Customer>(
                c => c.CustomerName == customerDto.CustomerName)), Times.Once);
        }
    }
}
