namespace App;

/// <summary>A small piece of code for the test suite to exercise. Replace with your own.</summary>
public static class Calculator
{
    public static int Add(int a, int b) => checked(a + b);

    public static double Divide(double a, double b) =>
        b == 0 ? throw new DivideByZeroException("cannot divide by zero") : a / b;

    public static async Task<int> SumAsync(IAsyncEnumerable<int> values, CancellationToken ct = default)
    {
        var total = 0;
        await foreach (var v in values.WithCancellation(ct))
        {
            total = checked(total + v);
        }
        return total;
    }
}
