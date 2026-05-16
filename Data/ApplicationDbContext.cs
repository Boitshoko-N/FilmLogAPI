using FilmLogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmLogAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<WatchedList> WatchedLists { get; set; }
        public DbSet<WatchListMovie> WatchlistMovies { get; set; }
        public DbSet<WatchedListMovie> WatchedListMovies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Watchlist)
                .WithOne(w => w.User)
                .HasForeignKey<Watchlist>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .HasOne(u => u.WatchedList)
                .WithOne(w => w.User)
                .HasForeignKey<WatchedList>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            modelBuilder.Entity<WatchListMovie>()
                .HasKey(wm => new { wm.WatchlistId, wm.MovieId });

            modelBuilder.Entity<WatchListMovie>()
                .HasOne(wm => wm.Watchlist)
                .WithMany(w => w.WatchlistMovies)
                .HasForeignKey(wm => wm.WatchlistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchListMovie>()
                .HasOne(wm => wm.Movie)
                .WithMany(m => m.WatchlistMovies)
                .HasForeignKey(wm => wm.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchedListMovie>()
                .HasKey(wm => new { wm.WatchedListId, wm.MovieId });

            modelBuilder.Entity<WatchedListMovie>()
                .HasOne(wm => wm.WatchedList)
                .WithMany(w => w.WatchedListMovies)
                .HasForeignKey(wm => wm.WatchedListId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchedListMovie>()
                .HasOne(wm => wm.Movie)
                .WithMany(m => m.WatchedListMovies)
                .HasForeignKey(wm => wm.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        
    }
}