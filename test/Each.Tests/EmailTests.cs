using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("user+tag@example.com")]
    [InlineData("a@b.co")]
    public void Create_WithValidEmail_ShouldSucceed(string email)
    {
        Email result = Email.Create(email);

        Assert.NotNull(result);
        Assert.Equal(email.ToLowerInvariant(), result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("@domain.com")]
    [InlineData("user@")]
    [InlineData("user@@domain.com")]
    public void Create_WithInvalidEmail_ShouldThrow(string email)
    {
        Assert.Throws<ValidationException>(() => Email.Create(email));
    }

    [Fact]
    public void TryCreate_WithValidEmail_ShouldReturnTrue()
    {
        bool result = Email.TryCreate("test@example.com", out Email? email);

        Assert.True(result);
        Assert.NotNull(email);
    }

    [Fact]
    public void TryCreate_WithInvalidEmail_ShouldReturnFalse()
    {
        bool result = Email.TryCreate("invalid", out Email? email);

        Assert.False(result);
        Assert.Null(email);
    }

    [Fact]
    public void LocalPart_ShouldReturnCorrectValue()
    {
        Email email = Email.Create("user@domain.com");

        Assert.Equal("user", email.LocalPart);
    }

    [Fact]
    public void Domain_ShouldReturnCorrectValue()
    {
        Email email = Email.Create("user@domain.com");

        Assert.Equal("domain.com", email.Domain);
    }

    [Fact]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        Email email = "test@example.com";

        Assert.NotNull(email);
    }

    [Fact]
    public void ImplicitConversion_ToString_ShouldWork()
    {
        Email email = Email.Create("test@example.com");
        string value = email;

        Assert.Equal("test@example.com", value);
    }
}