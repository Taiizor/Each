using Each.Exceptions;
using Each.Types;

namespace Each.Tests;

public class IPv4AddressTests
{
    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("127.0.0.1")]
    [InlineData("255.255.255.255")]
    [InlineData("0.0.0.0")]
    public void Create_WithValidIPv4_ShouldSucceed(string ip)
    {
        IPv4Address result = IPv4Address.Create(ip);

        Assert.NotNull(result);
        Assert.Equal(ip, result.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid")]
    [InlineData("256.1.1.1")]  // Out of range
    [InlineData("::1")]  // IPv6
    public void Create_WithInvalidIPv4_ShouldThrow(string ip)
    {
        Assert.Throws<ValidationException>(() => IPv4Address.Create(ip));
    }

    [Fact]
    public void IsLoopback_WithLoopback_ShouldReturnTrue()
    {
        IPv4Address ip = IPv4Address.Create("127.0.0.1");

        Assert.True(ip.IsLoopback);
    }

    [Fact]
    public void IsPrivate_With10Network_ShouldReturnTrue()
    {
        IPv4Address ip = IPv4Address.Create("10.0.0.1");

        Assert.True(ip.IsPrivate);
    }

    [Fact]
    public void IsPrivate_With172Network_ShouldReturnTrue()
    {
        IPv4Address ip = IPv4Address.Create("172.16.0.1");

        Assert.True(ip.IsPrivate);
    }

    [Fact]
    public void IsPrivate_With192168Network_ShouldReturnTrue()
    {
        IPv4Address ip = IPv4Address.Create("192.168.1.1");

        Assert.True(ip.IsPrivate);
    }

    [Fact]
    public void IsPrivate_WithPublicIP_ShouldReturnFalse()
    {
        IPv4Address ip = IPv4Address.Create("8.8.8.8");

        Assert.False(ip.IsPrivate);
    }

    [Fact]
    public void FromOctets_ShouldCreateValidIP()
    {
        IPv4Address ip = IPv4Address.FromOctets(192, 168, 1, 1);

        Assert.Equal("192.168.1.1", ip.Value);
    }

    [Fact]
    public void Octets_ShouldReturnCorrectBytes()
    {
        IPv4Address ip = IPv4Address.Create("192.168.1.100");
        byte[] octets = ip.Octets;

        Assert.Equal(4, octets.Length);
        Assert.Equal(192, octets[0]);
        Assert.Equal(168, octets[1]);
        Assert.Equal(1, octets[2]);
        Assert.Equal(100, octets[3]);
    }
}