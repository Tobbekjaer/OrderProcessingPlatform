using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InventoryService.Infrastructure.Persistence;

public sealed class InventoryDbContextFactory : IDesignTimeDbContextFactory<InventoryDbContext>
{
    public InventoryDbContext CreateDbContext(string[] args)
    {
        var conn =
            Environment.GetEnvironmentVariable("ConnectionStrings__InventoryDb")
            ?? "Server=localhost,14333;Database=InventoryDb;User Id=sa;Password=Villamaj201$;TrustServerCertificate=True;Encrypt=False";

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlServer(conn)
            .Options;

        return new InventoryDbContext(options);
    }
}