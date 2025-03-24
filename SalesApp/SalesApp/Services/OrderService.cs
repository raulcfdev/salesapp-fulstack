using AutoMapper;
using SalesApp.DTOs;
using SalesApp.Models;
using SalesApp.Repositories.Interfaces;
using SalesApp.Services.Interfaces;
using SalesApp.Services.RabbitMQ;

namespace SalesApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IRabbitMQService _rabbitMQService;
        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IMapper mapper,
            IRabbitMQService rabbitMQService)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _rabbitMQService = rabbitMQService;
        }

        public async Task<OrderDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                return null;

            return _mapper.Map<OrderDTO>(order);
        }

        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        public async Task<int> CreateOrderAsync(CreateOrderDTO orderDto)
        {
            var customer = await _customerRepository.GetByIdAsync(orderDto.CustomerRefId);
            if (customer == null)
                throw new KeyNotFoundException($"Cliente não encontrado com ID: {orderDto.CustomerRefId}");

            var order = _mapper.Map<Order>(orderDto);
            order.OrderDate = DateTime.Now;
            order.OrderStatus = OrderStatus.Pending;
            order.OrderItems = _mapper.Map<List<OrderItem>>(orderDto.OrderItems);
            order.OrderTotal = order.OrderItems.Sum(item => item.Quantity * item.UnitPrice);


            int orderId = await _orderRepository.AddAsync(order);


            var orderEvent = new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderTotal = order.OrderTotal
            };


            _rabbitMQService.SendMessage(orderEvent, "order_queue");

            return orderId;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Pedido não encontrado com ID: {orderId}");

            order.OrderStatus = status;
            await _orderRepository.UpdateAsync(order);
        }
    }
}