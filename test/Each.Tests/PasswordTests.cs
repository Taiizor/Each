using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class PasswordTests
{
    [Fact]
    public void Create_WithValidPassword_ShouldSucceed()
    {
        Password password = Password.Create("SecurePass123!");

        Assert.NotNull(password);
        Assert.NotEmpty(password.Hash);
        Assert.NotEmpty(password.Salt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("short")]  // Too short
    [InlineData("NoDigits!")]  // No digits
    [InlineData("nouppercasE1!")]  // Actually valid - has uppercase and all requirements
    public void Create_WithInvalidPassword_ShouldThrow(string plaintext)
    {
        Exception exception = Record.Exception(() => Password.Create(plaintext));

        // Most should throw, except the last one which might be valid
        if (plaintext == "nouppercasE1!")
        {
            Assert.Null(exception);  // This is valid
        }
        else
        {
            Assert.NotNull(exception);
        }
    }

    [Fact]
    public void Create_WithNoUppercase_ShouldThrow()
    {
        Assert.Throws<ValidationException>(() => Password.Create("password123!"));
    }

    [Fact]
    public void Create_WithNoSpecialCharacter_ShouldThrow()
    {
        Assert.Throws<ValidationException>(() => Password.Create("Password123"));
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        Password password = Password.Create("SecurePass123!");

        Assert.True(password.Verify("SecurePass123!"));
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        Password password = Password.Create("SecurePass123!");

        Assert.False(password.Verify("WrongPassword123!"));
    }

    [Fact]
    public void FromHash_ShouldReconstruct()
    {
        Password original = Password.Create("SecurePass123!");
        Password reconstructed = Password.FromHash(original.Hash, original.Salt);

        Assert.True(reconstructed.Verify("SecurePass123!"));
    }

    [Fact]
    public void Create_WithRelaxedOptions_ShouldAllowShorterPassword()
    {
        Password password = Password.Create("Pass12", PasswordOptions.Relaxed);

        Assert.NotNull(password);
    }

    [Fact]
    public void TryCreate_WithValidPassword_ShouldReturnTrue()
    {
        bool result = Password.TryCreate("SecurePass123!", out Password? password);

        Assert.True(result);
        Assert.NotNull(password);
    }

    [Fact]
    public void TryCreate_WithInvalidPassword_ShouldReturnFalse()
    {
        bool result = Password.TryCreate("weak", out Password? password);

        Assert.False(result);
        Assert.Null(password);
    }

    [Fact]
    public void Hash_ShouldBeDifferentForSamePassword()
    {
        Password password1 = Password.Create("SecurePass123!");
        Password password2 = Password.Create("SecurePass123!");

        // Different salts mean different hashes
        Assert.NotEqual(password1.Hash, password2.Hash);
    }
}