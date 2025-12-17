using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using OrderService.Application.Abstractions;
using Shared.Contracts.Inventory;

namespace OrderService.Infrastructure.Inventory;

public sealed class InventoryClient : IInventoryClient
{
    private readonly HttpClient _http;
    private readonly ILogger<InventoryClient> _logger;

    public InventoryClient(HttpClient http, ILogger<InventoryClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<bool> ReserveAsync(Guid orderId, IReadOnlyCollection<ReserveInventoryLineDto> items, CancellationToken ct)
    {
        var request = new ReserveInventoryRequest(orderId, items);

        var response =
            await _http.PostAsJsonAsync("api/inventory/reservations", request, ct);

        if (response.StatusCode == HttpStatusCode.Conflict)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }
}