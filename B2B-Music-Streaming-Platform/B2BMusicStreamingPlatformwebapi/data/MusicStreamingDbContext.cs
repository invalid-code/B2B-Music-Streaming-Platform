using Microsoft.EntityFrameworkCore;
using API.Models.Identity;
using API.Models.Core_Models;
using API.Models.Entities;

namespace API.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for the B2B Music Streaming Platform.
    /// Manages all database entities and relationships.
    /// </summary>
    public class MusicStreamingDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the MusicStreamingDbContext.
        /// </summary>
        public MusicStreamingDbContext(DbContextOptions<MusicStreamingDbContext> options)
            : base(options)
        {
        }

        // DbSets for Tenants and Users
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }

        // DbSets for Music Content
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }

        // DbSets for Venues
        public DbSet<Venue> Venues { get; set; }
        public DbSet<PaidVenue> PaidVenues { get; set; }
        public DbSet<TrialVenue> TrialVenues { get; set; }

        // DbSets for Streaming and Playback
        public DbSet<PlaybackSession> PlaybackSessions { get; set; }
        public DbSet<PlaybackLog> PlaybackLogs { get; set; }

        /// <summary>
        /// Configures the model using Fluent API.
        /// Defines relationships, constraints, and entity configurations.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Tenant entity
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.Property(e => e.PlanType).HasDefaultValue("Trial");
                entity.HasMany(e => e.Users)
                    .WithOne()
                    .HasForeignKey(u => u.TenantId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.StripeCustomerId).IsUnique();
            });

            // Configure ApplicationUser entity
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.TenantId).IsRequired();
                entity.Property(e => e.Role).HasDefaultValue("BusinessOwner");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Track entity
            modelBuilder.Entity<Track>(entity =>
            {
                entity.HasKey(e => e.TrackID);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Artist).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Mood).HasMaxLength(100);
                entity.Property(e => e.CloudflareStorageKey).IsRequired();
                entity.Property(e => e.UploadedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Playlist entity
            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.HasKey(e => e.PlaylistID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
                entity.Property(e => e.VibeOrGenre).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Venue (TPH - Table Per Hierarchy)
            modelBuilder.Entity<Venue>()
                .HasDiscriminator<string>("venue_type")
                .HasValue<PaidVenue>("paid")
                .HasValue<TrialVenue>("trial");

            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.VenueID);
                entity.Property(e => e.BusinessName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.Location).HasMaxLength(500);
                entity.Property(e => e.SubscriptionStatus).HasMaxLength(50);
            });

            // Configure PlaybackSession entity
            modelBuilder.Entity<PlaybackSession>(entity =>
            {
                entity.HasKey(e => e.SessionID);
                entity.Property(e => e.VenueID).IsRequired();
                entity.Property(e => e.TrackID).IsRequired();
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => new { e.VenueID, e.Timestamp });
            });

            // Configure PlaybackLog entity
            modelBuilder.Entity<PlaybackLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TenantId).IsRequired();
                entity.Property(e => e.TrackId).IsRequired();
                entity.Property(e => e.PlayedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => new { e.TenantId, e.PlayedAt });
            });
        }
    }
}
