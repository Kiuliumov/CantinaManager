using CantinaManager.Models;

namespace CantinaManager.Services.TokenService
{
    public interface ITokenService
    {

        Task<string> GenerateAccessTokenAsync(User user);
        string GenerateRefreshToken();
    }
}
