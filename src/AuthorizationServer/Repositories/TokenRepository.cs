using AuthorizationServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthorizationServer.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private AuthDbContext _context;
        public TokenRepository(AuthDbContext context) => _context = context;

        public async Task<bool> AddTokenAsync(RefreshToken token)
        {
            _context.Tokens.Add(token);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExpireTokenAsync(RefreshToken token)
        {
            _context.Tokens.Update(token);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> GetTokenAsync(string refreshToken, string clientId)
        {
            return await _context.Tokens.FirstOrDefaultAsync(p => p.Token == refreshToken && p.ClientId == clientId);
        }
    }
}
