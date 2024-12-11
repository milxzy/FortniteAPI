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
        public DbSet<Team> Teams { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.ID).HasName("PK_Teams");
            });
            modelBuilder.Entity<FortnitePlayer>(entity =>
            {

                entity.HasKey(e => e.ID).HasName("Primary");

                modelBuilder.Entity<FortnitePlayer>()
                    .HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamID);
            });
           

        }


    }
}
