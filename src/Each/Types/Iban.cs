using Each.Abstractions;
using Each.Exceptions;
using System.Text;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed International Bank Account Number (IBAN) with built-in validation.
/// </summary>
/// <remarks>
/// This type validates IBANs according to ISO 13616 standard.
/// </remarks>
public sealed partial class Iban : ValueObject<string>
{
    /// <summary>
    /// The minimum length for an IBAN.
    /// </summary>
    public const int MinLength = 15;

    /// <summary>
    /// The maximum length for an IBAN.
    /// </summary>
    public const int MaxLength = 34;

    private Iban(string value) : base(NormalizeIban(value))
    {
    }

    /// <summary>
    /// Gets the country code of the IBAN (first two characters).
    /// </summary>
    public string CountryCode => Value[..2];

    /// <summary>
    /// Gets the check digits of the IBAN (characters 3-4).
    /// </summary>
    public string CheckDigits => Value.Substring(2, 2);

    /// <summary>
    /// Gets the Basic Bank Account Number (BBAN) portion of the IBAN.
    /// </summary>
    public string Bban => Value[4..];

    /// <summary>
    /// Gets the IBAN formatted with spaces every 4 characters for readability.
    /// </summary>
    public string Formatted => FormatIban(Value);

    /// <summary>
    /// Creates a new <see cref="Iban"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IBAN string.</param>
    /// <returns>A new <see cref="Iban"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the IBAN is invalid.</exception>
    public static Iban Create(string value)
    {
        ValidationResult validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Iban), value, validationResult.ErrorMessage!);
        }
        return new Iban(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Iban"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The IBAN string.</param>
    /// <param name="iban">When this method returns, contains the created IBAN if successful; otherwise, null.</param>
    /// <returns>true if the IBAN was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out Iban? iban)
    {
        ValidationResult validationResult = Validate(value);
        if (validationResult.IsValid)
        {
            iban = new Iban(value);
            return true;
        }
        iban = null;
        return false;
    }

    /// <summary>
    /// Validates the specified IBAN string.
    /// </summary>
    /// <param name="value">The IBAN to validate.</param>
    /// <returns>A validation result indicating whether the IBAN is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("IBAN cannot be null or empty.");
        }

        string normalized = NormalizeIban(value);

        if (normalized.Length < MinLength)
        {
            return ValidationResult.Failure($"IBAN must be at least {MinLength} characters.");
        }

        if (normalized.Length > MaxLength)
        {
            return ValidationResult.Failure($"IBAN cannot exceed {MaxLength} characters.");
        }

        if (!IbanFormatRegex().IsMatch(normalized))
        {
            return ValidationResult.Failure("IBAN format is invalid. Must start with two letters followed by alphanumeric characters.");
        }

        if (!IsValidMod97(normalized))
        {
            return ValidationResult.Failure("IBAN failed checksum validation.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified IBAN string is valid.
    /// </summary>
    /// <param name="value">The IBAN to check.</param>
    /// <returns>true if the IBAN is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    private static bool IsValidMod97(string iban)
    {
        // Rearrange: move first 4 characters to end
        string rearranged = iban[4..] + iban[..4];

        // Replace letters with numbers (A=10, B=11, ..., Z=35)
        string numericString = string.Concat(rearranged.Select(c =>
            char.IsLetter(c) ? (c - 'A' + 10).ToString() : c.ToString()));

        // Calculate mod 97
        return Mod97(numericString) == 1;
    }

    private static int Mod97(string number)
    {
        int remainder = 0;
        foreach (char c in number)
        {
            int digit = c - '0';
            remainder = ((remainder * 10) + digit) % 97;
        }
        return remainder;
    }

    private static string NormalizeIban(string value)
    {
        return value.Replace(" ", "").Replace("-", "").ToUpperInvariant();
    }

    private static string FormatIban(string value)
    {
        StringBuilder formatted = new();
        for (int i = 0; i < value.Length; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                formatted.Append(' ');
            }
            formatted.Append(value[i]);
        }
        return formatted.ToString();
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="Iban"/> instance.
    /// </summary>
    /// <param name="value">The IBAN string.</param>
    public static implicit operator Iban(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts an <see cref="Iban"/> instance to a string.
    /// </summary>
    /// <param name="iban">The IBAN instance.</param>
    public static implicit operator string(Iban iban)
    {
        return iban.Value;
    }

    [GeneratedRegex(@"^[A-Z]{2}[0-9]{2}[A-Z0-9]+$", RegexOptions.Compiled)]
    private static partial Regex IbanFormatRegex();
}