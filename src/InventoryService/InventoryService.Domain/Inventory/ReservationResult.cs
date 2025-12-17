namespace InventoryService.Domain.Inventory;

public sealed class ReservationResult
{
    public bool Success { get; }
    public string? Error { get; }
    public Quantity? Available { get; }

    private ReservationResult(bool success, string? error = null, Quantity? available = null)
    {
        Success = success;
        Error = error;
        Available = available;
    }

    public static ReservationResult Succeeded() =>
        new(true);

    public static ReservationResult NotEnoughStock(Quantity available) =>
        new(false, "Not enough stock", available);

    public static ReservationResult Invalid(string error) =>
        new(false, error);
}