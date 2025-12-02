using Each.Abstractions;
using Each.Exceptions;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed postal code with built-in validation.
/// </summary>
/// <remarks>
/// This type supports postal codes from various countries with country-specific validation.
/// </remarks>
public sealed partial class PostalCode : ValueObject<string>
{
    /// <summary>
    /// Gets the country code associated with this postal code.
    /// </summary>
    public string CountryCode { get; }

    private PostalCode(string value, string countryCode) : base(value.ToUpperInvariant().Replace(" ", ""))
    {
        CountryCode = countryCode.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a new <see cref="PostalCode"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The postal code string.</param>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <returns>A new <see cref="PostalCode"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the postal code is invalid.</exception>
    public static PostalCode Create(string value, string countryCode)
    {
        ValidationResult validationResult = Validate(value, countryCode);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(PostalCode), value, validationResult.ErrorMessage!);
        }
        return new PostalCode(value, countryCode);
    }

    /// <summary>
    /// Attempts to create a new <see cref="PostalCode"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The postal code string.</param>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <param name="postalCode">When this method returns, contains the created postal code if successful; otherwise, null.</param>
    /// <returns>true if the postal code was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, string countryCode, out PostalCode? postalCode)
    {
        ValidationResult validationResult = Validate(value, countryCode);
        if (validationResult.IsValid)
        {
            postalCode = new PostalCode(value, countryCode);
            return true;
        }
        postalCode = null;
        return false;
    }

    /// <summary>
    /// Validates the specified postal code string.
    /// </summary>
    /// <param name="value">The postal code to validate.</param>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <returns>A validation result indicating whether the postal code is valid.</returns>
    public static ValidationResult Validate(string value, string countryCode)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Postal code cannot be null or empty.");
        }

        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return ValidationResult.Failure("Country code cannot be null or empty.");
        }

        if (countryCode.Length != 2)
        {
            return ValidationResult.Failure("Country code must be a 2-letter ISO 3166-1 alpha-2 code.");
        }

        string normalizedCountry = countryCode.ToUpperInvariant();
        string normalizedValue = value.ToUpperInvariant().Replace(" ", "");

        if (!PostalCodePatterns.TryGetValue(normalizedCountry, out string? pattern))
        {
            // If we don't have a specific pattern, use a general validation
            if (normalizedValue.Length is < 3 or > 10)
            {
                return ValidationResult.Failure("Postal code must be between 3 and 10 characters.");
            }
            return ValidationResult.Success();
        }

        if (!Regex.IsMatch(normalizedValue, pattern))
        {
            return ValidationResult.Failure($"Invalid postal code format for country {normalizedCountry}.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified postal code string is valid.
    /// </summary>
    /// <param name="value">The postal code to check.</param>
    /// <param name="countryCode">The ISO 3166-1 alpha-2 country code.</param>
    /// <returns>true if the postal code is valid; otherwise, false.</returns>
    public static bool IsValid(string value, string countryCode)
    {
        return Validate(value, countryCode).IsValid;
    }

    /// <summary>
    /// Implicitly converts a <see cref="PostalCode"/> instance to a string.
    /// </summary>
    /// <param name="postalCode">The postal code instance.</param>
    public static implicit operator string(PostalCode postalCode)
    {
        return postalCode.Value;
    }

    // Postal code patterns for various countries
    private static readonly Dictionary<string, string> PostalCodePatterns = new()
    {
        ["US"] = @"^\d{5}(-\d{4})?$",           // United States
        ["CA"] = @"^[A-Z]\d[A-Z]\d[A-Z]\d$",    // Canada
        ["GB"] = @"^[A-Z]{1,2}\d[A-Z\d]?\d[A-Z]{2}$", // United Kingdom
        ["DE"] = @"^\d{5}$",                     // Germany
        ["FR"] = @"^\d{5}$",                     // France
        ["IT"] = @"^\d{5}$",                     // Italy
        ["ES"] = @"^\d{5}$",                     // Spain
        ["NL"] = @"^\d{4}[A-Z]{2}$",            // Netherlands
        ["BE"] = @"^\d{4}$",                     // Belgium
        ["AT"] = @"^\d{4}$",                     // Austria
        ["CH"] = @"^\d{4}$",                     // Switzerland
        ["AU"] = @"^\d{4}$",                     // Australia
        ["JP"] = @"^\d{3}-?\d{4}$",             // Japan
        ["BR"] = @"^\d{5}-?\d{3}$",             // Brazil
        ["IN"] = @"^\d{6}$",                     // India
        ["CN"] = @"^\d{6}$",                     // China
        ["RU"] = @"^\d{6}$",                     // Russia
        ["PL"] = @"^\d{2}-\d{3}$",              // Poland
        ["SE"] = @"^\d{3}\s?\d{2}$",            // Sweden
        ["NO"] = @"^\d{4}$",                     // Norway
        ["DK"] = @"^\d{4}$",                     // Denmark
        ["FI"] = @"^\d{5}$",                     // Finland
        ["PT"] = @"^\d{4}-?\d{3}$",             // Portugal
        ["GR"] = @"^\d{3}\s?\d{2}$",            // Greece
        ["TR"] = @"^\d{5}$",                     // Turkey
        ["MX"] = @"^\d{5}$",                     // Mexico
        ["AR"] = @"^[A-Z]\d{4}[A-Z]{3}$",       // Argentina
        ["ZA"] = @"^\d{4}$",                     // South Africa
        ["KR"] = @"^\d{5}$",                     // South Korea
        ["SG"] = @"^\d{6}$",                     // Singapore
    };
}