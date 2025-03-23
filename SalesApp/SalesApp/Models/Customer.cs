namespace SalesApp.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
