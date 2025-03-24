using Microsoft.AspNetCore.Identity;

namespace SalesApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
