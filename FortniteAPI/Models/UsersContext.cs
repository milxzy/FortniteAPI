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
            options.UseNpgsql("User Id=postgres.ibgcjhdggdltmgoegwsa;Password=CounterStrike10!1;Server=aws-0-us-east-1.pooler.supabase.com;Port=5432;Database=postgres;");

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
