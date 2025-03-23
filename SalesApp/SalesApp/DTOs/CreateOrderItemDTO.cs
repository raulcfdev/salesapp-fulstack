namespace SalesApp.DTOs
{

    public class CreateOrderItemDTO
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
