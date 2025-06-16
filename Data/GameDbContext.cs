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
    }
}