using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class PercentageTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(25.5)]
    public void Create_WithValidPercentage_ShouldSucceed(decimal value)
    {
        Percentage result = Percentage.Create(value);

        Assert.NotNull(result);
        Assert.Equal(value, result.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(-50)]
    public void Create_WithInvalidPercentage_ShouldThrow(decimal value)
    {
        Assert.Throws<ValidationException>(() => Percentage.Create(value));
    }

    [Fact]
    public void Create_WithAbove100Allowed_ShouldSucceed()
    {
        Percentage result = Percentage.Create(150, allowAbove100: true);

        Assert.Equal(150, result.Value);
    }

    [Fact]
    public void AsFraction_ShouldReturnCorrectValue()
    {
        Percentage percentage = Percentage.Create(50);

        Assert.Equal(0.5m, percentage.AsFraction);
    }

    [Fact]
    public void FromFraction_ShouldConvertCorrectly()
    {
        Percentage percentage = Percentage.FromFraction(0.75m);

        Assert.Equal(75, percentage.Value);
    }

    [Fact]
    public void Formatted_ShouldIncludePercentSign()
    {
        Percentage percentage = Percentage.Create(50);

        Assert.Equal("50%", percentage.Formatted);
    }

    [Fact]
    public void Addition_ShouldWork()
    {
        Percentage p1 = Percentage.Create(30);
        Percentage p2 = Percentage.Create(20);
        Percentage result = p1 + p2;

        Assert.Equal(50, result.Value);
    }

    [Fact]
    public void Subtraction_ShouldWork()
    {
        Percentage p1 = Percentage.Create(80);
        Percentage p2 = Percentage.Create(30);
        Percentage result = p1 - p2;

        Assert.Equal(50, result.Value);
    }

    [Fact]
    public void Multiplication_ShouldWork()
    {
        Percentage percentage = Percentage.Create(25);
        Percentage result = percentage * 2;

        Assert.Equal(50, result.Value);
    }

    [Fact]
    public void Division_ShouldWork()
    {
        Percentage percentage = Percentage.Create(50);
        Percentage result = percentage / 2;

        Assert.Equal(25, result.Value);
    }

    [Fact]
    public void Division_ByZero_ShouldThrow()
    {
        Percentage percentage = Percentage.Create(50);

        Assert.Throws<DivideByZeroException>(() => percentage / 0);
    }
}