using Microsoft.AspNetCore.Mvc;
using SalesApp.DTOs;
using SalesApp.Services.Interfaces;

namespace SalesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<CustomerDTO>>> ListAllCustomers()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(customers);
        }
        [HttpGet("details/{customerId}")]
        public async Task<ActionResult<CustomerDTO>> FindCustomerById(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }
        [HttpPost("add")]
        public async Task<ActionResult<int>> AddNewCustomer(CreateCustomerDTO customerDto)
        {
            var customerId = await _customerService.CreateCustomerAsync(customerDto);
            return CreatedAtAction(nameof(FindCustomerById), new { customerId }, customerId);
        }
    }
}
