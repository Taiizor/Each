using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class UrlTests
{
    [Theory]
    [InlineData("https://www.example.com")]
    [InlineData("http://localhost:8080")]
    [InlineData("https://example.com/path?query=value")]
    [InlineData("ftp://files.example.com")]
    public void Create_WithValidUrl_ShouldSucceed(string url)
    {
        Url result = Url.Create(url);

        Assert.NotNull(result);
        Assert.Equal(url, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-url")]
    [InlineData("mailto:test@example.com")]  // Unsupported protocol
    public void Create_WithInvalidUrl_ShouldThrow(string url)
    {
        Assert.Throws<ValidationException>(() => Url.Create(url));
    }

    [Fact]
    public void IsSecure_WithHttps_ShouldReturnTrue()
    {
        Url url = Url.Create("https://example.com");

        Assert.True(url.IsSecure);
    }

    [Fact]
    public void IsSecure_WithHttp_ShouldReturnFalse()
    {
        Url url = Url.Create("http://example.com");

        Assert.False(url.IsSecure);
    }

    [Fact]
    public void Host_ShouldReturnCorrectValue()
    {
        Url url = Url.Create("https://www.example.com/path");

        Assert.Equal("www.example.com", url.Host);
    }

    [Fact]
    public void Path_ShouldReturnCorrectValue()
    {
        Url url = Url.Create("https://example.com/api/v1/users");

        Assert.Equal("/api/v1/users", url.Path);
    }

    [Fact]
    public void ToUri_ShouldReturnValidUri()
    {
        Url url = Url.Create("https://example.com");
        Uri uri = url.ToUri();

        Assert.NotNull(uri);
        Assert.IsType<Uri>(uri);
    }
}