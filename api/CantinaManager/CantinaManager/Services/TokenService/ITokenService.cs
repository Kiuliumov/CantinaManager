using CantinaManager.Models;

namespace CantinaManager.Services
{
    public interface ITokenService
    {

        Task<string> GenerateAccessTokenAsync(User user);
        string GenerateRefreshToken();
    }
}
