using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;

namespace AuthorizationServer.Models
{
    public class AuthDbContextFactoryDevOnly : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var dbfile = Path.Combine(Directory.GetCurrentDirectory(), "auth.db");
            var builder = new DbContextOptionsBuilder<AuthDbContext>();
            builder.UseSqlite($"Data Source={dbfile}");
            return new AuthDbContext(builder.Options);
        }
    }
}
