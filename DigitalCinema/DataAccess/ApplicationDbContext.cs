using DigitalCinema.DataAccess.EntityTypeConfigurations;
using DigitalCinema.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DigitalCinema.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems{ get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowMovieHall> ShowMovieHalls { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<SupImg> SupImgs { get; set; }
        public DbSet<ActorMovie> ActorMovies { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=DigitalCinema526;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True");
        //}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShowMovieHall>()
                .HasOne(smh => smh.Hall)
                .WithMany() // أو حسب العلاقة في موديل الـ Hall
                .HasForeignKey(smh => smh.HallId)
                .OnDelete(DeleteBehavior.Restrict); // هذا سيحل أي تعارض في مسارات الحذف

            modelBuilder.Entity<Ticket>()
       .HasOne(t => t.Booking)
       .WithMany(b => b.Tickets)
       .HasForeignKey(t => t.BookingId)
       .OnDelete(DeleteBehavior.Cascade);

            // حماية للـ ShowSeat: لو التذكرة موجودة، ممنوع نمسح سجل كرسي العرض
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.ShowSeat)
                .WithMany()
                .HasForeignKey(t => t.ShowSeatId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<OrderItem>()
                .HasOne(o => o.Show)
                .WithMany()
                .HasForeignKey(o => o.ShowId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieEntityTypeConfiguration).Assembly);
        }
    }
}
