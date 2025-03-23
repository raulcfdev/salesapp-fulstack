using SalesApp.Models;

namespace SalesApp.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int orderId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<int> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int orderId);
    }
}
