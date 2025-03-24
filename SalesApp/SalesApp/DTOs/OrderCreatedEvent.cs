namespace SalesApp.DTOs
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderTotal { get; set; }
       
    }
}
