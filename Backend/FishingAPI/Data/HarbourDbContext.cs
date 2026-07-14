using Microsoft.EntityFrameworkCore;
using FishingAPI.Models;

namespace FishingAPI.Data;

public class HarbourDbContext : DbContext
{
    public HarbourDbContext(DbContextOptions<HarbourDbContext> options)
        : base(options)
    {
    }

    public DbSet<Boat> Boats => Set<Boat>();

    public DbSet<Fish> Fish => Set<Fish>();

    public DbSet<User> Users => Set<User>();

    public DbSet<HarbourStatus> HarbourStatuses => Set<HarbourStatus>();
}