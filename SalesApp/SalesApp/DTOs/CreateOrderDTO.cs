namespace SalesApp.DTOs
{
    public class CreateOrderDTO
    {
        public int CustomerRefId { get; set; }
        public ICollection<CreateOrderItemDTO> OrderItems { get; set; }
    }
}
