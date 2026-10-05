namespace App.Tests;

/// <summary>Built once and shared by every test in <see cref="InventoryTests"/>.</summary>
public sealed class StockedInventory
{
    public Inventory Inventory { get; } = new();

    public StockedInventory() => Inventory.Add("widget", 10);
}

public class InventoryTests(StockedInventory fixture) : IClassFixture<StockedInventory>
{
    [Fact]
    public void Lookup_is_case_insensitive() =>
        Assert.True(fixture.Inventory.Count("WIDGET") >= 1);

    [Fact]
    public void Cannot_remove_more_than_is_in_stock() =>
        Assert.False(fixture.Inventory.TryRemove("widget", 1_000));

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Blank_sku_is_rejected(string sku) =>
        Assert.Throws<ArgumentException>(() => new Inventory().Add(sku, 1));

    [Fact]
    public void Non_positive_quantity_is_rejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Inventory().Add("bolt", 0));
}
