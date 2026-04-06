using DigitalCinema.DataAccess.EntityTypeConfigurations;
using DigitalCinema.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalCinema.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<SupImg> SupImgs { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DigitalCinema526;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieEntityTypeConfiguration).Assembly);
        }
    }
}
