using Eventia.Domain.Entities;
using Eventia.Domain.Interfaces;
using Eventia.Domain.ValueObjects;
using Eventia.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Eventia.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- User ---
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Name).IsRequired().HasMaxLength(150);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);
            e.Property(u => u.IsActive).HasDefaultValue(true);
        });

        // --- Event ---
        modelBuilder.Entity<Event>(e =>
        {
            e.HasKey(ev => ev.Id);
            e.Property(ev => ev.Name).IsRequired().HasMaxLength(200);
            e.Property(ev => ev.Description).HasMaxLength(1000);
            e.Property(ev => ev.Location).IsRequired().HasMaxLength(300);

            e.HasOne(ev => ev.CreatedBy)
             .WithMany()
             .HasForeignKey(ev => ev.CreatedById)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // --- Ticket ---
        modelBuilder.Entity<Ticket>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.Property(t => t.Description).HasMaxLength(2000);
            e.Property(t => t.Status).HasConversion<string>().HasMaxLength(30);

            e.HasOne(t => t.CreatedBy)
             .WithMany()
             .HasForeignKey(t => t.CreatedById)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(t => t.AssignedTo)
             .WithMany()
             .HasForeignKey(t => t.AssignedToId)
             .IsRequired(false)
             .OnDelete(DeleteBehavior.SetNull);

            e.HasOne(t => t.Event)
             .WithMany(ev => ev.Tickets)
             .HasForeignKey(t => t.EventId)
             .OnDelete(DeleteBehavior.Cascade);

            // Ignore domain events (not persisted)
            e.Ignore(t => t.DomainEvents);
        });

        // --- TicketHistory ---
        modelBuilder.Entity<TicketHistory>(e =>
        {
            e.HasKey(h => h.Id);
            e.Property(h => h.OldStatus).HasConversion<string>().HasMaxLength(30);
            e.Property(h => h.NewStatus).HasConversion<string>().HasMaxLength(30);
            e.Property(h => h.Notes).HasMaxLength(500);

            e.HasOne(h => h.Ticket)
             .WithMany(t => t.History)
             .HasForeignKey(h => h.TicketId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(h => h.ChangedBy)
             .WithMany()
             .HasForeignKey(h => h.ChangedById)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await base.SaveChangesAsync(ct);
}
