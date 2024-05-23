using Microsoft.EntityFrameworkCore;
using MiniProjectAPI.Models;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Contexts
{
    public class MovieBookingContext : DbContext
    {
        public MovieBookingContext(DbContextOptions options) : base(options) {
        
        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<BookingSnack> BookingSnacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Movie to Showtime relationship
            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Showtimes)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);

            // Theatre to Screen relationship
            modelBuilder.Entity<Theater>()
                .HasMany(t => t.Screens)
                .WithOne(s => s.Theater)
                .HasForeignKey(s => s.TheaterId);

            // Screen to Seat relationship
            modelBuilder.Entity<Screen>()
                .HasMany(s => s.Seats)
                .WithOne(se => se.Screen)
                .HasForeignKey(se => se.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Screen to Showtime relationship
            modelBuilder.Entity<Screen>()
                .HasMany(s => s.Showtimes)
                .WithOne(st => st.Screen)
                .HasForeignKey(st => st.ScreenId);

            // Showtime to Booking relationship
            modelBuilder.Entity<Showtime>()
                .HasMany(s => s.Bookings)
                .WithOne(b => b.Showtime)
                .HasForeignKey(b => b.ShowtimeId);

            // Booking to Seat one-to-many relationship
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.Seats)
                .WithOne(s => s.Booking)
                .HasForeignKey(s => s.BookingId);

            // Cancellation to Seat one-to-many relationship
            modelBuilder.Entity<Cancellation>()
                .HasMany(c => c.Seats)
                .WithOne(s => s.Cancellation)
                .HasForeignKey(s => s.CancellationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingSnack>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSnacks)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingSnack>()
                .HasOne(bs => bs.Snack)
                .WithMany(s => s.BookingSnacks)
                .HasForeignKey(bs => bs.SnackId);
        }
    }
}
