using IndustrialMachineMonitoring.Models;
using Microsoft.EntityFrameworkCore;

namespace IndustrialMachineMonitoring.Data;

public class MachineDbContext : DbContext
{
    public MachineDbContext(DbContextOptions<MachineDbContext> options)
        : base(options)
    {
    }

    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<SensorReading> SensorReadings => Set<SensorReading>();

    public DbSet<AlertLog> AlertLogs => Set<AlertLog>();

    public DbSet<MachineDowntime> MachineDowntimes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Machine>()
            .HasIndex(m => m.MachineCode)
            .IsUnique();

        modelBuilder.Entity<SensorReading>()
            .HasOne(sr => sr.Machine)
            .WithMany(m => m.SensorReadings)
            .HasForeignKey(sr => sr.MachineId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AlertLog>()
            .HasOne(a => a.Machine)
            .WithMany(m => m.AlertLogs)
            .HasForeignKey(a => a.MachineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}