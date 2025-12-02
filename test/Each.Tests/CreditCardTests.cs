using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class CreditCardTests
{
    [Theory]
    [InlineData("4111111111111111")]  // Visa test number
    [InlineData("5500000000000004")]  // MasterCard test number
    [InlineData("340000000000009")]   // American Express test number
    [InlineData("4111-1111-1111-1111")]  // With dashes
    [InlineData("4111 1111 1111 1111")]  // With spaces
    public void Create_WithValidCreditCard_ShouldSucceed(string cardNumber)
    {
        CreditCard result = CreditCard.Create(cardNumber);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234567890")]  // Too short
    [InlineData("4111111111111112")]  // Invalid Luhn
    public void Create_WithInvalidCreditCard_ShouldThrow(string cardNumber)
    {
        Assert.Throws<ValidationException>(() => CreditCard.Create(cardNumber));
    }

    [Fact]
    public void CardType_Visa_ShouldBeDetected()
    {
        CreditCard card = CreditCard.Create("4111111111111111");

        Assert.Equal(CreditCardType.Visa, card.CardType);
    }

    [Fact]
    public void CardType_MasterCard_ShouldBeDetected()
    {
        CreditCard card = CreditCard.Create("5500000000000004");

        Assert.Equal(CreditCardType.MasterCard, card.CardType);
    }

    [Fact]
    public void CardType_AmericanExpress_ShouldBeDetected()
    {
        CreditCard card = CreditCard.Create("340000000000009");

        Assert.Equal(CreditCardType.AmericanExpress, card.CardType);
    }

    [Fact]
    public void LastFourDigits_ShouldReturnCorrectValue()
    {
        CreditCard card = CreditCard.Create("4111111111111111");

        Assert.Equal("1111", card.LastFourDigits);
    }

    [Fact]
    public void Masked_ShouldMaskAllButLastFour()
    {
        CreditCard card = CreditCard.Create("4111111111111111");

        Assert.Equal("****-****-****-1111", card.Masked);
    }
}