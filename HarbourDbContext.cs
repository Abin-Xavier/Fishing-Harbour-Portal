using FishingAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Data;

public class HarbourDbContext : DbContext
{
    public HarbourDbContext(DbContextOptions<HarbourDbContext> options) : base(options) { }

    public DbSet<Boat> Boats => Set<Boat>();
    public DbSet<FishDetail> FishDetails => Set<FishDetail>();
    public DbSet<HarbourStatusEntity> HarbourStatusRows => Set<HarbourStatusEntity>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Boat>(b =>
        {
            b.ToTable("boats");
            b.HasIndex(x => x.BoatNumber).IsUnique();
        });

        modelBuilder.Entity<FishDetail>(f =>
        {
            f.ToTable("fish_details");
        });

        modelBuilder.Entity<HarbourStatusEntity>(h =>
        {
            h.ToTable("harbour_status");
        });

        modelBuilder.Entity<User>(u =>
        {
            u.ToTable("users");
            u.HasIndex(x => x.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
