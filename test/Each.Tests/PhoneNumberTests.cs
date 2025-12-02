using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+44 20 7123 4567")]
    [InlineData("1-800-555-1234")]
    [InlineData("(555) 123-4567")]
    public void Create_WithValidPhoneNumber_ShouldSucceed(string phoneNumber)
    {
        PhoneNumber result = PhoneNumber.Create(phoneNumber);

        Assert.NotNull(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]  // Too short
    [InlineData("abc")]
    public void Create_WithInvalidPhoneNumber_ShouldThrow(string phoneNumber)
    {
        Assert.Throws<ValidationException>(() => PhoneNumber.Create(phoneNumber));
    }

    [Fact]
    public void TryCreate_WithValidPhoneNumber_ShouldReturnTrue()
    {
        bool result = PhoneNumber.TryCreate("+1234567890", out PhoneNumber? phoneNumber);

        Assert.True(result);
        Assert.NotNull(phoneNumber);
    }

    [Fact]
    public void E164Format_ShouldAddPlusSign()
    {
        PhoneNumber phoneNumber = PhoneNumber.Create("1234567890");

        Assert.StartsWith("+", phoneNumber.E164Format);
    }

    [Fact]
    public void DigitsOnly_ShouldRemoveFormatting()
    {
        PhoneNumber phoneNumber = PhoneNumber.Create("+12345678901");

        Assert.Equal("12345678901", phoneNumber.DigitsOnly);
    }
}