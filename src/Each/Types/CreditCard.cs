using Each.Abstractions;
using Each.Exceptions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed credit card number with built-in validation.
/// </summary>
/// <remarks>
/// This type validates credit card numbers using the Luhn algorithm and
/// supports major card networks (Visa, MasterCard, American Express, etc.).
/// </remarks>
public sealed partial class CreditCard : ValueObject<string>
{
    private CreditCard(string value, CreditCardType cardType) : base(NormalizeCardNumber(value))
    {
        CardType = cardType;
    }

    /// <summary>
    /// Gets the type/network of the credit card.
    /// </summary>
    public CreditCardType CardType { get; }

    /// <summary>
    /// Gets the last four digits of the credit card number.
    /// </summary>
    public string LastFourDigits => Value[^4..];

    /// <summary>
    /// Gets a masked version of the credit card number (e.g., "****-****-****-1234").
    /// </summary>
    public string Masked => $"****-****-****-{LastFourDigits}";

    /// <summary>
    /// Creates a new <see cref="CreditCard"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The credit card number string.</param>
    /// <returns>A new <see cref="CreditCard"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the credit card number is invalid.</exception>
    public static CreditCard Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out CreditCardType cardType);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(CreditCard), MaskForError(value), validationResult.ErrorMessage!);
        }
        return new CreditCard(value, cardType);
    }

    /// <summary>
    /// Attempts to create a new <see cref="CreditCard"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The credit card number string.</param>
    /// <param name="creditCard">When this method returns, contains the created credit card if successful; otherwise, null.</param>
    /// <returns>true if the credit card was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out CreditCard? creditCard)
    {
        ValidationResult validationResult = ValidateInternal(value, out CreditCardType cardType);
        if (validationResult.IsValid)
        {
            creditCard = new CreditCard(value, cardType);
            return true;
        }
        creditCard = null;
        return false;
    }

    /// <summary>
    /// Validates the specified credit card number string.
    /// </summary>
    /// <param name="value">The credit card number to validate.</param>
    /// <returns>A validation result indicating whether the credit card number is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _);
    }

    private static ValidationResult ValidateInternal(string value, out CreditCardType cardType)
    {
        cardType = CreditCardType.Unknown;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Credit card number cannot be null or empty.");
        }

        string digitsOnly = NormalizeCardNumber(value);

        if (digitsOnly.Length is < 13 or > 19)
        {
            return ValidationResult.Failure("Credit card number must be between 13 and 19 digits.");
        }

        if (!digitsOnly.All(char.IsDigit))
        {
            return ValidationResult.Failure("Credit card number must contain only digits.");
        }

        if (!IsValidLuhn(digitsOnly))
        {
            return ValidationResult.Failure("Credit card number failed Luhn validation.");
        }

        cardType = DetermineCardType(digitsOnly);

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified credit card number string is valid.
    /// </summary>
    /// <param name="value">The credit card number to check.</param>
    /// <returns>true if the credit card number is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Gets the card type for the specified credit card number without full validation.
    /// </summary>
    /// <param name="value">The credit card number.</param>
    /// <returns>The detected card type.</returns>
    public static CreditCardType GetCardType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return CreditCardType.Unknown;
        }

        string digitsOnly = NormalizeCardNumber(value);
        return DetermineCardType(digitsOnly);
    }

    private static bool IsValidLuhn(string number)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = number.Length - 1; i >= 0; i--)
        {
            int digit = number[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                {
                    digit -= 9;
                }
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    private static CreditCardType DetermineCardType(string digitsOnly)
    {
        if (digitsOnly.StartsWith('4'))
        {
            return CreditCardType.Visa;
        }

        if (digitsOnly.Length >= 2)
        {
            int firstTwo = int.Parse(digitsOnly[..2]);

            if (firstTwo is >= 51 and <= 55)
            {
                return CreditCardType.MasterCard;
            }

            if (firstTwo is 34 or 37)
            {
                return CreditCardType.AmericanExpress;
            }

            if (firstTwo is 30 or 36 or 38)
            {
                return CreditCardType.DinersClub;
            }

            if (firstTwo == 65)
            {
                return CreditCardType.Discover;
            }

            if (firstTwo == 35)
            {
                return CreditCardType.JCB;
            }
        }

        if (digitsOnly.Length >= 4)
        {
            int firstFour = int.Parse(digitsOnly[..4]);

            if (firstFour == 6011)
            {
                return CreditCardType.Discover;
            }

            if (firstFour is >= 2221 and <= 2720)
            {
                return CreditCardType.MasterCard;
            }
        }

        if (digitsOnly.Length >= 6)
        {
            int firstSix = int.Parse(digitsOnly[..6]);

            if (firstSix is >= 622126 and <= 622925)
            {
                return CreditCardType.Discover;
            }
        }

        return CreditCardType.Unknown;
    }

    private static string NormalizeCardNumber(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    private static string MaskForError(string value)
    {
        string digits = new(value.Where(char.IsDigit).ToArray());
        if (digits.Length <= 4)
        {
            return "****";
        }
        return $"****{digits[^4..]}";
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="CreditCard"/> instance.
    /// </summary>
    /// <param name="value">The credit card number string.</param>
    public static implicit operator CreditCard(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="CreditCard"/> instance to a string.
    /// </summary>
    /// <param name="creditCard">The credit card instance.</param>
    public static implicit operator string(CreditCard creditCard)
    {
        return creditCard.Value;
    }
}

/// <summary>
/// Represents the type/network of a credit card.
/// </summary>
public enum CreditCardType
{
    /// <summary>
    /// Unknown or unrecognized card type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Visa card.
    /// </summary>
    Visa,

    /// <summary>
    /// MasterCard.
    /// </summary>
    MasterCard,

    /// <summary>
    /// American Express card.
    /// </summary>
    AmericanExpress,

    /// <summary>
    /// Discover card.
    /// </summary>
    Discover,

    /// <summary>
    /// JCB card.
    /// </summary>
    JCB,

    /// <summary>
    /// Diners Club card.
    /// </summary>
    DinersClub
}