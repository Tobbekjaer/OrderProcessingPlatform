using InventoryService.Application.Abstractions;
using InventoryService.Application.Inventory.AdjustStock;
using InventoryService.Application.Inventory.ReserveInventory;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var conn = builder.Configuration.GetConnectionString("InventoryDb");

if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException("Connection string 'InventoryDb' not configured.");
}

builder.Services.AddDbContext<InventoryDbContext>(opt => opt.UseSqlServer(conn));

builder.Services.AddScoped<IInventoryRepository, EfInventoryRepository>();
builder.Services.AddScoped<ReserveInventoryCommandHandler>();
builder.Services.AddScoped<AdjustStockCommandHandler>();

var app = builder.Build();

// Auto-migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    db.Database.Migrate();
    await InventoryDbSeeder.SeedAsync(db, logger, CancellationToken.None);
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();