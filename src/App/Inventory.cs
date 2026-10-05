namespace App;

/// <summary>A tiny stateful service, to show a shared fixture.</summary>
public sealed class Inventory
{
    private readonly Dictionary<string, int> _stock = new(StringComparer.OrdinalIgnoreCase);

    public void Add(string sku, int quantity)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
        _stock[sku] = Count(sku) + quantity;
    }

    public bool TryRemove(string sku, int quantity)
    {
        if (Count(sku) < quantity) return false;
        _stock[sku] -= quantity;
        return true;
    }

    public int Count(string sku) => _stock.TryGetValue(sku, out var n) ? n : 0;
}
