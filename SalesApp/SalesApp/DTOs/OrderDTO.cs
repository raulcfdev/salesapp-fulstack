namespace SalesApp.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; }

        public class OrderStatusUpdateDTO
        {
            public string Status { get; set; }
        }
    }


}
