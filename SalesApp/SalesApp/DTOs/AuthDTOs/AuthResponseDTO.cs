namespace SalesApp.DTOs.AuthDTOs
{
    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
