namespace SalesApp.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderRefId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public Order Order { get; set; }
    }
}
