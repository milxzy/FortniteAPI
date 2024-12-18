using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FortniteAPI.Models
{
    public class UsersContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var authConnection = DotNetEnv.Env.GetString("AUTH_CONNECTION");
            var authString = Environment.GetEnvironmentVariable("AUTH_CONNECTION");
            options.UseNpgsql(
               authString,
                npgsqlOptions => npgsqlOptions.CommandTimeout(60)); // Increase timeout to 60 seconds
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
