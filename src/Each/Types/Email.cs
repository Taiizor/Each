using Each.Abstractions;
using Each.Exceptions;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed email address with built-in validation.
/// </summary>
/// <remarks>
/// This type ensures that only valid email addresses can be created and stored.
/// It follows RFC 5322 email format specifications.
/// </remarks>
public sealed partial class Email : ValueObject<string>
{
    /// <summary>
    /// The maximum allowed length for an email address according to RFC 5321.
    /// </summary>
    public const int MaxLength = 254;

    /// <summary>
    /// The maximum allowed length for the local part of an email address.
    /// </summary>
    public const int MaxLocalPartLength = 64;

    /// <summary>
    /// The maximum allowed length for the domain part of an email address.
    /// </summary>
    public const int MaxDomainPartLength = 253;

    private Email(string value) : base(value.ToLowerInvariant())
    {
    }

    /// <summary>
    /// Gets the local part of the email address (the part before the @ symbol).
    /// </summary>
    public string LocalPart => Value.Split('@')[0];

    /// <summary>
    /// Gets the domain part of the email address (the part after the @ symbol).
    /// </summary>
    public string Domain => Value.Split('@')[1];

    /// <summary>
    /// Creates a new <see cref="Email"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The email address string.</param>
    /// <returns>A new <see cref="Email"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the email address is invalid.</exception>
    public static Email Create(string value)
    {
        ValidationResult validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Email), value, validationResult.ErrorMessage!);
        }
        return new Email(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Email"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The email address string.</param>
    /// <param name="email">When this method returns, contains the created email if successful; otherwise, null.</param>
    /// <returns>true if the email was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out Email? email)
    {
        ValidationResult validationResult = Validate(value);
        if (validationResult.IsValid)
        {
            email = new Email(value);
            return true;
        }
        email = null;
        return false;
    }

    /// <summary>
    /// Validates the specified email address string.
    /// </summary>
    /// <param name="value">The email address to validate.</param>
    /// <returns>A validation result indicating whether the email is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Email address cannot be null or empty.");
        }

        if (value.Length > MaxLength)
        {
            return ValidationResult.Failure($"Email address cannot exceed {MaxLength} characters.");
        }

        if (!value.Contains('@'))
        {
            return ValidationResult.Failure("Email address must contain an '@' symbol.");
        }

        string[] parts = value.Split('@');
        if (parts.Length != 2)
        {
            return ValidationResult.Failure("Email address must contain exactly one '@' symbol.");
        }

        string localPart = parts[0];
        string domainPart = parts[1];

        if (string.IsNullOrEmpty(localPart))
        {
            return ValidationResult.Failure("Email local part cannot be empty.");
        }

        if (localPart.Length > MaxLocalPartLength)
        {
            return ValidationResult.Failure($"Email local part cannot exceed {MaxLocalPartLength} characters.");
        }

        if (string.IsNullOrEmpty(domainPart))
        {
            return ValidationResult.Failure("Email domain part cannot be empty.");
        }

        if (domainPart.Length > MaxDomainPartLength)
        {
            return ValidationResult.Failure($"Email domain part cannot exceed {MaxDomainPartLength} characters.");
        }

        if (!EmailRegex().IsMatch(value))
        {
            return ValidationResult.Failure("Email address format is invalid.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified email address string is valid.
    /// </summary>
    /// <param name="value">The email address to check.</param>
    /// <returns>true if the email address is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="Email"/> instance.
    /// </summary>
    /// <param name="value">The email address string.</param>
    public static implicit operator Email(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="Email"/> instance to a string.
    /// </summary>
    /// <param name="email">The email instance.</param>
    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();
}