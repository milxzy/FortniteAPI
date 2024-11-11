using Microsoft.EntityFrameworkCore;


namespace FortniteAPI.Models
{
    public class FortniteContext : DbContext
    {
        public FortniteContext(DbContextOptions<FortniteContext> options)
            : base(options) 
        {
        }
        public DbSet<FortnitePlayer> FortnitePlayers { get; set; } = null!;
    }
}
