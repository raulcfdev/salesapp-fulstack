using SalesApp.DTOs;
using SalesApp.Models;

namespace SalesApp.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDTO> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status);
        Task<int> CreateOrderAsync(CreateOrderDTO orderDto);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
