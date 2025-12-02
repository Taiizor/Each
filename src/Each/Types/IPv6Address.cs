using Each.Abstractions;
using Each.Exceptions;
using System.Net;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed IPv6 address with built-in validation.
/// </summary>
/// <remarks>
/// This type ensures that only valid IPv6 addresses can be created and stored.
/// </remarks>
public sealed class IPv6Address : ValueObject<string>
{
    private readonly IPAddress _ipAddress;

    private IPv6Address(string value, IPAddress ipAddress) : base(ipAddress.ToString())
    {
        _ipAddress = ipAddress;
    }

    /// <summary>
    /// Gets a value indicating whether this is a loopback address (::1).
    /// </summary>
    public bool IsLoopback => IPAddress.IsLoopback(_ipAddress);

    /// <summary>
    /// Gets the compressed string representation of the IPv6 address.
    /// </summary>
    public string Compressed => _ipAddress.ToString();

    /// <summary>
    /// Gets the underlying <see cref="IPAddress"/> instance.
    /// </summary>
    public IPAddress ToIPAddress()
    {
        return _ipAddress;
    }

    /// <summary>
    /// Creates a new <see cref="IPv6Address"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IPv6 address string.</param>
    /// <returns>A new <see cref="IPv6Address"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the IPv6 address is invalid.</exception>
    public static IPv6Address Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out IPAddress? ipAddress);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(IPv6Address), value, validationResult.ErrorMessage!);
        }
        return new IPv6Address(value, ipAddress!);
    }

    /// <summary>
    /// Attempts to create a new <see cref="IPv6Address"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IPv6 address string.</param>
    /// <param name="ipv6">When this method returns, contains the created IPv6 address if successful; otherwise, null.</param>
    /// <returns>true if the IPv6 address was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out IPv6Address? ipv6)
    {
        ValidationResult validationResult = ValidateInternal(value, out IPAddress? ipAddress);
        if (validationResult.IsValid)
        {
            ipv6 = new IPv6Address(value, ipAddress!);
            return true;
        }
        ipv6 = null;
        return false;
    }

    /// <summary>
    /// Validates the specified IPv6 address string.
    /// </summary>
    /// <param name="value">The IPv6 address to validate.</param>
    /// <returns>A validation result indicating whether the IPv6 address is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _);
    }

    private static ValidationResult ValidateInternal(string value, out IPAddress? ipAddress)
    {
        ipAddress = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("IPv6 address cannot be null or empty.");
        }

        if (!IPAddress.TryParse(value, out ipAddress))
        {
            return ValidationResult.Failure("IPv6 address format is invalid.");
        }

        if (ipAddress.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            ipAddress = null;
            return ValidationResult.Failure("The provided address is not a valid IPv6 address.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified IPv6 address string is valid.
    /// </summary>
    /// <param name="value">The IPv6 address to check.</param>
    /// <returns>true if the IPv6 address is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Gets the loopback address (::1).
    /// </summary>
    public static IPv6Address Loopback => Create("::1");

    /// <summary>
    /// Gets the any address (::).
    /// </summary>
    public static IPv6Address Any => Create("::");

    /// <summary>
    /// Implicitly converts a string to an <see cref="IPv6Address"/> instance.
    /// </summary>
    /// <param name="value">The IPv6 address string.</param>
    public static implicit operator IPv6Address(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="IPv6Address"/> instance to a string.
    /// </summary>
    /// <param name="ipv6">The IPv6 address instance.</param>
    public static implicit operator string(IPv6Address ipv6)
    {
        return ipv6.Value;
    }

    /// <summary>
    /// Converts an <see cref="IPv6Address"/> instance to an <see cref="IPAddress"/>.
    /// </summary>
    /// <param name="ipv6">The IPv6 address instance.</param>
    public static implicit operator IPAddress(IPv6Address ipv6)
    {
        return ipv6._ipAddress;
    }
}