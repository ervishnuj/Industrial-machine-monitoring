using IndustrialMachineMonitoring.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IndustrialMachineMonitoring.Data;

public class MachineDbContextFactory
    : IDesignTimeDbContextFactory<MachineDbContext>
{
    public MachineDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddUserSecrets<MachineDbContextFactory>()
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("MachineDb");

        var optionsBuilder =
            new DbContextOptionsBuilder<MachineDbContext>();

        optionsBuilder.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        );

        return new MachineDbContext(optionsBuilder.Options);
    }
}