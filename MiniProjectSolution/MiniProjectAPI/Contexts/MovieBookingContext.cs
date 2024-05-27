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
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; }

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
                .HasForeignKey(b => b.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Booking to ShowtimeSeat one-to-many relationship
            modelBuilder.Entity<Booking>()
                .HasMany(b => b.ShowtimeSeats)
                .WithOne(ss => ss.Booking)
                .HasForeignKey(ss => ss.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cancellation to ShowtimeSeat one-to-many relationship
            modelBuilder.Entity<Cancellation>()
                .HasMany(c => c.ShowtimeSeats)
                .WithOne(ss => ss.Cancellation)
                .HasForeignKey(ss => ss.CancellationId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BookingSnack>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSnacks)
                .HasForeignKey(bs => bs.BookingId);

            modelBuilder.Entity<BookingSnack>()
                .HasOne(bs => bs.Snack)
                .WithMany(s => s.BookingSnacks)
                .HasForeignKey(bs => bs.SnackId);

            // Seat and ShowtimeSeat relationship
            modelBuilder.Entity<Seat>()
                .HasMany(seat => seat.ShowtimeSeats)
                .WithOne(ss => ss.Seat)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<ShowtimeSeat>()
            .HasOne(ss => ss.Showtime)
            .WithMany(s => s.ShowtimeSeats)
            .HasForeignKey(ss => ss.ShowtimeId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShowtimeSeat>()
                .HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowtimeSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // ShowtimeSeat to Booking
            modelBuilder.Entity<ShowtimeSeat>()
                .HasOne(ss => ss.Booking)
                .WithMany(b => b.ShowtimeSeats)
                .HasForeignKey(ss => ss.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // ShowtimeSeat to Cancellation
            modelBuilder.Entity<ShowtimeSeat>()
                .HasOne(ss => ss.Cancellation)
                .WithMany(c => c.ShowtimeSeats)
                .HasForeignKey(ss => ss.CancellationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
