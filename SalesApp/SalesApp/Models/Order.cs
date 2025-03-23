namespace SalesApp.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; } 
        public int CustomerRefId { get; set; } 

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal OrderTotal { get; set; }
        public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public bool CanBeProcessed() => OrderItems.Count > 0 && OrderStatus == OrderStatus.Pending;
    }
}
