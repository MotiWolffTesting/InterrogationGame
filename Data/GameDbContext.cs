using InterrogationGame.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterrogationGame.Data;

public class GameDbContext : DbContext
{
    // DbSet for managing Person entities
    public DbSet<Person> People { get; set; }
    // DbSet for managing GameLog entities
    public DbSet<GameLog> GameLogs { get; set; }
    // DbSet for managing Score entities
    public DbSet<Score> Scores { get; set; }

    // Constructor that accepts DbContextOptions for configuration
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    // Configure the database model and relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Person entity
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("people");
            entity.HasKey(e => e.Id);
            // Configure required fields with maximum lengths
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FavoriteWeapon).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContactNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SecretPhrase).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Affiliation).IsRequired().HasMaxLength(100);
            // Set default value for IsExposed to false
            entity.Property(e => e.IsExposed).HasDefaultValue(false);
            // New: SuperiorId as a foreign key
            entity.HasOne<Person>()
                .WithMany()
                .HasForeignKey(e => e.SuperiorId)
                .OnDelete(DeleteBehavior.Restrict);

            // AI & Behavior properties
            entity.Property(e => e.Personality).HasDefaultValue(PersonalityType.Neutral).HasConversion<string>();
            entity.Property(e => e.InterrogationCount).HasDefaultValue(0);
            entity.Property(e => e.LastInterrogation).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.HasLied).HasDefaultValue(false);
            // Note: PreviousResponses will be handled as a simple string list in memory
        });

        // Configure GameLog entity
        modelBuilder.Entity<GameLog>(entity =>
        {
            entity.ToTable("game_logs");
            entity.HasKey(e => e.Id);
            // Configure required fields with maximum lengths
            entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Details).IsRequired();
            // Set default timestamp to current time
            entity.Property(e => e.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure relationship with Person entity
            entity.HasOne<Person>()
                .WithMany()
                .HasForeignKey(e => e.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Score entity
        modelBuilder.Entity<Score>(entity =>
        {
            entity.ToTable("scores");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PlayerName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Points).IsRequired();
            entity.Property(e => e.Timestamp).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}