using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FortniteAPI.Models
{
    public class UsersContext : IdentityDbContext<IdentityUser>
    {
        public UsersContext (DbContextOptions<UsersContext> options)
            :base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=FortnitePlayersDB;Username=postgres;Password=1;");
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
