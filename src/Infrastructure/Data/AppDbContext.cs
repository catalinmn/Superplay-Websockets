using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<GiftTransaction> GiftTransactions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.DeviceId)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(p => p.DeviceId)
                .IsUnique();
        });

        // GiftTransaction configuration
        modelBuilder.Entity<GiftTransaction>(entity =>
        {
            entity.HasKey(g => g.Id);

            // Configure relationships without navigation properties
            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(g => g.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne<Player>()
                .WithMany()
                .HasForeignKey(g => g.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}