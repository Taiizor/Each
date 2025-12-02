using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class CurrencyTests
{
    [Fact]
    public void Create_WithValidCurrency_ShouldSucceed()
    {
        Currency currency = Currency.Create(100.50m, "USD");

        Assert.NotNull(currency);
        Assert.Equal(100.50m, currency.Value);
        Assert.Equal("USD", currency.CurrencyCode);
        Assert.Equal("$", currency.Symbol);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("USDD")]  // Invalid length
    [InlineData("XXX")]   // Unknown currency
    public void Create_WithInvalidCurrencyCode_ShouldThrow(string code)
    {
        Assert.Throws<ValidationException>(() => Currency.Create(100, code));
    }

    [Fact]
    public void USD_FactoryMethod_ShouldWork()
    {
        Currency currency = Currency.USD(100);

        Assert.Equal("USD", currency.CurrencyCode);
        Assert.Equal("$", currency.Symbol);
    }

    [Fact]
    public void EUR_FactoryMethod_ShouldWork()
    {
        Currency currency = Currency.EUR(100);

        Assert.Equal("EUR", currency.CurrencyCode);
        Assert.Equal("€", currency.Symbol);
    }

    [Fact]
    public void TRY_FactoryMethod_ShouldWork()
    {
        Currency currency = Currency.TRY(100);

        Assert.Equal("TRY", currency.CurrencyCode);
        Assert.Equal("₺", currency.Symbol);
    }

    [Fact]
    public void Addition_SameCurrency_ShouldWork()
    {
        Currency c1 = Currency.USD(100);
        Currency c2 = Currency.USD(50);
        Currency result = c1 + c2;

        Assert.Equal(150, result.Value);
        Assert.Equal("USD", result.CurrencyCode);
    }

    [Fact]
    public void Addition_DifferentCurrency_ShouldThrow()
    {
        Currency c1 = Currency.USD(100);
        Currency c2 = Currency.EUR(50);

        Assert.Throws<InvalidOperationException>(() => c1 + c2);
    }

    [Fact]
    public void Subtraction_SameCurrency_ShouldWork()
    {
        Currency c1 = Currency.USD(100);
        Currency c2 = Currency.USD(30);
        Currency result = c1 - c2;

        Assert.Equal(70, result.Value);
    }

    [Fact]
    public void Multiplication_ShouldWork()
    {
        Currency currency = Currency.USD(100);
        Currency result = currency * 1.5m;

        Assert.Equal(150, result.Value);
    }

    [Fact]
    public void Division_ShouldWork()
    {
        Currency currency = Currency.USD(100);
        Currency result = currency / 2;

        Assert.Equal(50, result.Value);
    }

    [Fact]
    public void Formatted_ShouldIncludeSymbolAndCode()
    {
        Currency currency = Currency.USD(100.50m);

        Assert.Contains("$", currency.Formatted);
        Assert.Contains("USD", currency.Formatted);
        Assert.Contains("100.50", currency.Formatted);
    }
}