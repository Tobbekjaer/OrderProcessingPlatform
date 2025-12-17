using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrderService.Infrastructure.Persistence;

public sealed class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
{
    public OrderDbContext CreateDbContext(string[] args)
    {
        var conn =
            Environment.GetEnvironmentVariable("ConnectionStrings__OrderDb")
            ?? "Server=localhost,14332;Database=OrderDb;User Id=sa;Password=Villamaj201$;TrustServerCertificate=True;Encrypt=False";

        var options = new DbContextOptionsBuilder<OrderDbContext>()
            .UseSqlServer(conn)
            .Options;

        return new OrderDbContext(options);
    }
}