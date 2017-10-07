using AuthorizationServer.Models;
using System.Threading.Tasks;

namespace AuthorizationServer.Repositories
{
    public interface ITokenRepository
    {
        Task<bool> AddTokenAsync(RefreshToken token);
        Task<bool> ExpireTokenAsync(RefreshToken token);
        Task<RefreshToken> GetTokenAsync(string refreshToken, string clientId);
    }
}
