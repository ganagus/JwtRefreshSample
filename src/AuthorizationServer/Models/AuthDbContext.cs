using Microsoft.EntityFrameworkCore;

namespace AuthorizationServer.Models
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<RefreshToken> Tokens { get; set; }
    }
}
