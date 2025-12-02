using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class IbanTests
{
    [Theory]
    [InlineData("DE89370400440532013000")]  // German IBAN
    [InlineData("GB82WEST12345698765432")]  // UK IBAN
    [InlineData("FR1420041010050500013M02606")]  // French IBAN
    [InlineData("DE89 3704 0044 0532 0130 00")]  // With spaces
    public void Create_WithValidIban_ShouldSucceed(string iban)
    {
        Iban result = Iban.Create(iban);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("DE8937")]  // Too short
    [InlineData("DE00000000000000000000")]  // Invalid checksum
    public void Create_WithInvalidIban_ShouldThrow(string iban)
    {
        Assert.Throws<ValidationException>(() => Iban.Create(iban));
    }

    [Fact]
    public void CountryCode_ShouldReturnCorrectValue()
    {
        Iban iban = Iban.Create("DE89370400440532013000");

        Assert.Equal("DE", iban.CountryCode);
    }

    [Fact]
    public void CheckDigits_ShouldReturnCorrectValue()
    {
        Iban iban = Iban.Create("DE89370400440532013000");

        Assert.Equal("89", iban.CheckDigits);
    }

    [Fact]
    public void Bban_ShouldReturnCorrectValue()
    {
        Iban iban = Iban.Create("DE89370400440532013000");

        Assert.Equal("370400440532013000", iban.Bban);
    }

    [Fact]
    public void Formatted_ShouldHaveSpacesEveryFourCharacters()
    {
        Iban iban = Iban.Create("DE89370400440532013000");

        Assert.Equal("DE89 3704 0044 0532 0130 00", iban.Formatted);
    }
}