using Microsoft.EntityFrameworkCore;


namespace FortniteAPI.Models
{
    public class FortniteContext : DbContext
    {
        // nuget package manager console commands:
        //add-migration InitialDeploy (create script to run on db)
        //update-database (runs the script)
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
                //primary key
                entity.HasKey(e => e.ID).HasName("Primary");
                //default value for a bool

                //one to many relationship
                modelBuilder.Entity<FortnitePlayer>()
                    .HasOne(p => p.Team)
                    .WithMany(t => t.Players)
                    .HasForeignKey(p => p.TeamID);
            });
           

        }

        // on model create (configure foreign keys, or default properties)
    }
}
