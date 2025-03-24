using SalesApp.Models;

namespace SalesApp.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
