namespace App.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_returns_the_sum() => Assert.Equal(5, Calculator.Add(2, 3));

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(-4, 4, 0)]
    [InlineData(0, 0, 0)]
    public void Add_handles_signs(int a, int b, int expected) =>
        Assert.Equal(expected, Calculator.Add(a, b));

    [Fact]
    public void Add_throws_on_overflow() =>
        Assert.Throws<OverflowException>(() => Calculator.Add(int.MaxValue, 1));

    public static TheoryData<double, double, double> Quotients => new()
    {
        { 10, 4, 2.5 },
        { -9, 3, -3 },
    };

    [Theory]
    [MemberData(nameof(Quotients))]
    public void Divide_returns_the_quotient(double a, double b, double expected) =>
        Assert.Equal(expected, Calculator.Divide(a, b), precision: 10);

    [Fact]
    public void Divide_by_zero_throws()
    {
        var ex = Assert.Throws<DivideByZeroException>(() => Calculator.Divide(1, 0));
        Assert.Contains("zero", ex.Message);
    }

    [Fact]
    public async Task SumAsync_adds_a_stream()
    {
        static async IAsyncEnumerable<int> Numbers()
        {
            for (var i = 1; i <= 4; i++)
            {
                await Task.Yield();
                yield return i;
            }
        }

        Assert.Equal(10, await Calculator.SumAsync(Numbers(), TestContext.Current.CancellationToken));
    }
}
