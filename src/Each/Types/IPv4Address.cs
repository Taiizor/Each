using Each.Abstractions;
using Each.Exceptions;
using System.Net;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed IPv4 address with built-in validation.
/// </summary>
/// <remarks>
/// This type ensures that only valid IPv4 addresses can be created and stored.
/// </remarks>
public sealed class IPv4Address : ValueObject<string>
{
    private readonly IPAddress _ipAddress;

    private IPv4Address(string value, IPAddress ipAddress) : base(value)
    {
        _ipAddress = ipAddress;
    }

    /// <summary>
    /// Gets the individual octets of the IPv4 address.
    /// </summary>
    public byte[] Octets => _ipAddress.GetAddressBytes();

    /// <summary>
    /// Gets a value indicating whether this is a loopback address (127.x.x.x).
    /// </summary>
    public bool IsLoopback => IPAddress.IsLoopback(_ipAddress);

    /// <summary>
    /// Gets a value indicating whether this is a private address.
    /// </summary>
    public bool IsPrivate
    {
        get
        {
            byte[] bytes = Octets;
            return bytes[0] == 10 ||
                   (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                   (bytes[0] == 192 && bytes[1] == 168);
        }
    }

    /// <summary>
    /// Gets the underlying <see cref="IPAddress"/> instance.
    /// </summary>
    public IPAddress ToIPAddress()
    {
        return _ipAddress;
    }

    /// <summary>
    /// Creates a new <see cref="IPv4Address"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IPv4 address string.</param>
    /// <returns>A new <see cref="IPv4Address"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the IPv4 address is invalid.</exception>
    public static IPv4Address Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out IPAddress? ipAddress);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(IPv4Address), value, validationResult.ErrorMessage!);
        }
        return new IPv4Address(value, ipAddress!);
    }

    /// <summary>
    /// Attempts to create a new <see cref="IPv4Address"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IPv4 address string.</param>
    /// <param name="ipv4">When this method returns, contains the created IPv4 address if successful; otherwise, null.</param>
    /// <returns>true if the IPv4 address was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out IPv4Address? ipv4)
    {
        ValidationResult validationResult = ValidateInternal(value, out IPAddress? ipAddress);
        if (validationResult.IsValid)
        {
            ipv4 = new IPv4Address(value, ipAddress!);
            return true;
        }
        ipv4 = null;
        return false;
    }

    /// <summary>
    /// Validates the specified IPv4 address string.
    /// </summary>
    /// <param name="value">The IPv4 address to validate.</param>
    /// <returns>A validation result indicating whether the IPv4 address is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _);
    }

    private static ValidationResult ValidateInternal(string value, out IPAddress? ipAddress)
    {
        ipAddress = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("IPv4 address cannot be null or empty.");
        }

        if (!IPAddress.TryParse(value, out ipAddress))
        {
            return ValidationResult.Failure("IPv4 address format is invalid.");
        }

        if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            ipAddress = null;
            return ValidationResult.Failure("The provided address is not a valid IPv4 address.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified IPv4 address string is valid.
    /// </summary>
    /// <param name="value">The IPv4 address to check.</param>
    /// <returns>true if the IPv4 address is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Creates a new <see cref="IPv4Address"/> from the specified octets.
    /// </summary>
    /// <param name="octet1">The first octet.</param>
    /// <param name="octet2">The second octet.</param>
    /// <param name="octet3">The third octet.</param>
    /// <param name="octet4">The fourth octet.</param>
    /// <returns>A new <see cref="IPv4Address"/> instance.</returns>
    public static IPv4Address FromOctets(byte octet1, byte octet2, byte octet3, byte octet4)
    {
        return Create($"{octet1}.{octet2}.{octet3}.{octet4}");
    }

    /// <summary>
    /// Gets the loopback address (127.0.0.1).
    /// </summary>
    public static IPv4Address Loopback => Create("127.0.0.1");

    /// <summary>
    /// Gets the any address (0.0.0.0).
    /// </summary>
    public static IPv4Address Any => Create("0.0.0.0");

    /// <summary>
    /// Gets the broadcast address (255.255.255.255).
    /// </summary>
    public static IPv4Address Broadcast => Create("255.255.255.255");

    /// <summary>
    /// Implicitly converts a string to an <see cref="IPv4Address"/> instance.
    /// </summary>
    /// <param name="value">The IPv4 address string.</param>
    public static implicit operator IPv4Address(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="IPv4Address"/> instance to a string.
    /// </summary>
    /// <param name="ipv4">The IPv4 address instance.</param>
    public static implicit operator string(IPv4Address ipv4)
    {
        return ipv4.Value;
    }

    /// <summary>
    /// Converts an <see cref="IPv4Address"/> instance to an <see cref="IPAddress"/>.
    /// </summary>
    /// <param name="ipv4">The IPv4 address instance.</param>
    public static implicit operator IPAddress(IPv4Address ipv4)
    {
        return ipv4._ipAddress;
    }
}