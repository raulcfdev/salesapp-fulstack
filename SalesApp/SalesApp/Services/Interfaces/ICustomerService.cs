using SalesApp.DTOs;

namespace SalesApp.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDTO> GetCustomerByIdAsync(int customerId);
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<int> CreateCustomerAsync(CreateCustomerDTO customerDto);
    }
}
