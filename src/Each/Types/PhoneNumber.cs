using Each.Abstractions;
using Each.Exceptions;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed phone number with built-in validation.
/// </summary>
/// <remarks>
/// This type supports international phone numbers in E.164 format and various common formats.
/// </remarks>
public sealed partial class PhoneNumber : ValueObject<string>
{
    /// <summary>
    /// The minimum length for a phone number (including country code).
    /// </summary>
    public const int MinLength = 7;

    /// <summary>
    /// The maximum length for a phone number according to E.164 standard.
    /// </summary>
    public const int MaxLength = 15;

    private PhoneNumber(string value) : base(NormalizePhoneNumber(value))
    {
    }

    /// <summary>
    /// Gets the phone number in E.164 format (starting with +).
    /// </summary>
    public string E164Format => Value.StartsWith('+') ? Value : $"+{Value}";

    /// <summary>
    /// Gets the phone number digits only (without any formatting).
    /// </summary>
    public string DigitsOnly => new(Value.Where(char.IsDigit).ToArray());

    /// <summary>
    /// Creates a new <see cref="PhoneNumber"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <returns>A new <see cref="PhoneNumber"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the phone number is invalid.</exception>
    public static PhoneNumber Create(string value)
    {
        ValidationResult validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(PhoneNumber), value, validationResult.ErrorMessage!);
        }
        return new PhoneNumber(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="PhoneNumber"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    /// <param name="phoneNumber">When this method returns, contains the created phone number if successful; otherwise, null.</param>
    /// <returns>true if the phone number was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out PhoneNumber? phoneNumber)
    {
        ValidationResult validationResult = Validate(value);
        if (validationResult.IsValid)
        {
            phoneNumber = new PhoneNumber(value);
            return true;
        }
        phoneNumber = null;
        return false;
    }

    /// <summary>
    /// Validates the specified phone number string.
    /// </summary>
    /// <param name="value">The phone number to validate.</param>
    /// <returns>A validation result indicating whether the phone number is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Phone number cannot be null or empty.");
        }

        string digitsOnly = new(value.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length < MinLength)
        {
            return ValidationResult.Failure($"Phone number must contain at least {MinLength} digits.");
        }

        if (digitsOnly.Length > MaxLength)
        {
            return ValidationResult.Failure($"Phone number cannot exceed {MaxLength} digits.");
        }

        if (!PhoneNumberRegex().IsMatch(value))
        {
            return ValidationResult.Failure("Phone number format is invalid.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified phone number string is valid.
    /// </summary>
    /// <param name="value">The phone number to check.</param>
    /// <returns>true if the phone number is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="PhoneNumber"/> instance.
    /// </summary>
    /// <param name="value">The phone number string.</param>
    public static implicit operator PhoneNumber(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="PhoneNumber"/> instance to a string.
    /// </summary>
    /// <param name="phoneNumber">The phone number instance.</param>
    public static implicit operator string(PhoneNumber phoneNumber)
    {
        return phoneNumber.Value;
    }

    private static string NormalizePhoneNumber(string value)
    {
        // Remove common formatting characters but preserve + at the start
        string normalized = value.Trim();
        if (normalized.StartsWith('+'))
        {
            return "+" + new string(normalized[1..].Where(char.IsDigit).ToArray());
        }
        return new string(normalized.Where(char.IsDigit).ToArray());
    }

    [GeneratedRegex(@"^[\+]?[(]?[0-9]{1,4}[)]?[-\s\./0-9]*$", RegexOptions.Compiled)]
    private static partial Regex PhoneNumberRegex();
}