using Each.Abstractions;
using Each.Exceptions;
using System.Globalization;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed currency value with currency code.
/// </summary>
/// <remarks>
/// This type ensures that monetary values are handled correctly with their associated currency.
/// Uses ISO 4217 currency codes.
/// </remarks>
public sealed class Currency : ValueObject<decimal>, IEquatable<Currency>
{
    /// <summary>
    /// Gets the ISO 4217 currency code (e.g., "USD", "EUR", "GBP").
    /// </summary>
    public string CurrencyCode { get; }

    /// <summary>
    /// Gets the currency symbol (e.g., "$", "€", "£").
    /// </summary>
    public string Symbol { get; }

    private Currency(decimal amount, string currencyCode, string symbol) : base(amount)
    {
        CurrencyCode = currencyCode;
        Symbol = symbol;
    }

    /// <summary>
    /// Gets the formatted currency string using the appropriate culture settings.
    /// </summary>
    public string Formatted => FormatCurrency(Value, CurrencyCode, Symbol);

    /// <summary>
    /// Creates a new <see cref="Currency"/> instance.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currencyCode">The ISO 4217 currency code.</param>
    /// <returns>A new <see cref="Currency"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the currency parameters are invalid.</exception>
    public static Currency Create(decimal amount, string currencyCode)
    {
        ValidationResult validationResult = ValidateInternal(currencyCode, out string? symbol);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Currency), currencyCode, validationResult.ErrorMessage!);
        }
        return new Currency(amount, currencyCode.ToUpperInvariant(), symbol!);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Currency"/> instance.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currencyCode">The ISO 4217 currency code.</param>
    /// <param name="currency">When this method returns, contains the created currency if successful; otherwise, null.</param>
    /// <returns>true if the currency was created successfully; otherwise, false.</returns>
    public static bool TryCreate(decimal amount, string currencyCode, out Currency? currency)
    {
        ValidationResult validationResult = ValidateInternal(currencyCode, out string? symbol);
        if (validationResult.IsValid)
        {
            currency = new Currency(amount, currencyCode.ToUpperInvariant(), symbol!);
            return true;
        }
        currency = null;
        return false;
    }

    /// <summary>
    /// Validates the specified currency code.
    /// </summary>
    /// <param name="currencyCode">The currency code to validate.</param>
    /// <returns>A validation result indicating whether the currency code is valid.</returns>
    public static ValidationResult Validate(string currencyCode)
    {
        return ValidateInternal(currencyCode, out _);
    }

    private static ValidationResult ValidateInternal(string currencyCode, out string? symbol)
    {
        symbol = null;

        if (string.IsNullOrWhiteSpace(currencyCode))
        {
            return ValidationResult.Failure("Currency code cannot be null or empty.");
        }

        if (currencyCode.Length != 3)
        {
            return ValidationResult.Failure("Currency code must be exactly 3 characters (ISO 4217).");
        }

        string normalized = currencyCode.ToUpperInvariant();

        if (!CurrencyInfo.TryGetValue(normalized, out (string Symbol, string Name) info))
        {
            return ValidationResult.Failure($"Unknown currency code: {currencyCode}");
        }

        symbol = info.Symbol;
        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified currency code is valid.
    /// </summary>
    /// <param name="currencyCode">The currency code to check.</param>
    /// <returns>true if the currency code is valid; otherwise, false.</returns>
    public static bool IsValid(string currencyCode)
    {
        return Validate(currencyCode).IsValid;
    }

    /// <summary>
    /// Adds two currency values of the same currency.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public static Currency operator +(Currency left, Currency right)
    {
        EnsureSameCurrency(left, right);
        return new Currency(left.Value + right.Value, left.CurrencyCode, left.Symbol);
    }

    /// <summary>
    /// Subtracts one currency value from another of the same currency.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when currencies don't match.</exception>
    public static Currency operator -(Currency left, Currency right)
    {
        EnsureSameCurrency(left, right);
        return new Currency(left.Value - right.Value, left.CurrencyCode, left.Symbol);
    }

    /// <summary>
    /// Multiplies a currency value by a scalar.
    /// </summary>
    public static Currency operator *(Currency currency, decimal scalar)
    {
        return new Currency(currency.Value * scalar, currency.CurrencyCode, currency.Symbol);
    }

    /// <summary>
    /// Divides a currency value by a scalar.
    /// </summary>
    public static Currency operator /(Currency currency, decimal scalar)
    {
        if (scalar == 0)
        {
            throw new DivideByZeroException("Cannot divide currency by zero.");
        }
        return new Currency(currency.Value / scalar, currency.CurrencyCode, currency.Symbol);
    }

    private static void EnsureSameCurrency(Currency left, Currency right)
    {
        if (left.CurrencyCode != right.CurrencyCode)
        {
            throw new InvalidOperationException($"Cannot perform operation on different currencies: {left.CurrencyCode} and {right.CurrencyCode}");
        }
    }

    /// <summary>
    /// Determines whether this currency equals another currency.
    /// </summary>
    public bool Equals(Currency? other)
    {
        if (other is null)
        {
            return false;
        }

        return Value == other.Value && CurrencyCode == other.CurrencyCode;
    }

    /// <summary>
    /// Determines whether this currency equals another object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        return obj is Currency other && Equals(other);
    }

    /// <summary>
    /// Gets the hash code for this currency.
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value, CurrencyCode);
    }

    /// <summary>
    /// Returns a string representation of the currency.
    /// </summary>
    public override string ToString()
    {
        return Formatted;
    }

    private static string FormatCurrency(decimal amount, string code, string symbol)
    {
        return $"{symbol}{amount.ToString("N2", CultureInfo.InvariantCulture)} {code}";
    }

    // Factory methods for common currencies
    /// <summary>
    /// Creates a USD currency value.
    /// </summary>
    public static Currency USD(decimal amount)
    {
        return Create(amount, "USD");
    }

    /// <summary>
    /// Creates a EUR currency value.
    /// </summary>
    public static Currency EUR(decimal amount)
    {
        return Create(amount, "EUR");
    }

    /// <summary>
    /// Creates a GBP currency value.
    /// </summary>
    public static Currency GBP(decimal amount)
    {
        return Create(amount, "GBP");
    }

    /// <summary>
    /// Creates a JPY currency value.
    /// </summary>
    public static Currency JPY(decimal amount)
    {
        return Create(amount, "JPY");
    }

    /// <summary>
    /// Creates a TRY (Turkish Lira) currency value.
    /// </summary>
    public static Currency TRY(decimal amount)
    {
        return Create(amount, "TRY");
    }

    /// <summary>
    /// Creates a CHF currency value.
    /// </summary>
    public static Currency CHF(decimal amount)
    {
        return Create(amount, "CHF");
    }

    /// <summary>
    /// Creates a CAD currency value.
    /// </summary>
    public static Currency CAD(decimal amount)
    {
        return Create(amount, "CAD");
    }

    /// <summary>
    /// Creates a AUD currency value.
    /// </summary>
    public static Currency AUD(decimal amount)
    {
        return Create(amount, "AUD");
    }

    // Currency information dictionary
    private static readonly Dictionary<string, (string Symbol, string Name)> CurrencyInfo = new()
    {
        ["USD"] = ("$", "US Dollar"),
        ["EUR"] = ("€", "Euro"),
        ["GBP"] = ("£", "British Pound"),
        ["JPY"] = ("¥", "Japanese Yen"),
        ["CHF"] = ("Fr", "Swiss Franc"),
        ["CAD"] = ("C$", "Canadian Dollar"),
        ["AUD"] = ("A$", "Australian Dollar"),
        ["NZD"] = ("NZ$", "New Zealand Dollar"),
        ["CNY"] = ("¥", "Chinese Yuan"),
        ["HKD"] = ("HK$", "Hong Kong Dollar"),
        ["SGD"] = ("S$", "Singapore Dollar"),
        ["SEK"] = ("kr", "Swedish Krona"),
        ["NOK"] = ("kr", "Norwegian Krone"),
        ["DKK"] = ("kr", "Danish Krone"),
        ["INR"] = ("₹", "Indian Rupee"),
        ["RUB"] = ("₽", "Russian Ruble"),
        ["BRL"] = ("R$", "Brazilian Real"),
        ["ZAR"] = ("R", "South African Rand"),
        ["MXN"] = ("$", "Mexican Peso"),
        ["KRW"] = ("₩", "South Korean Won"),
        ["TRY"] = ("₺", "Turkish Lira"),
        ["PLN"] = ("zł", "Polish Zloty"),
        ["THB"] = ("฿", "Thai Baht"),
        ["IDR"] = ("Rp", "Indonesian Rupiah"),
        ["MYR"] = ("RM", "Malaysian Ringgit"),
        ["PHP"] = ("₱", "Philippine Peso"),
        ["CZK"] = ("Kč", "Czech Koruna"),
        ["HUF"] = ("Ft", "Hungarian Forint"),
        ["ILS"] = ("₪", "Israeli New Shekel"),
        ["AED"] = ("د.إ", "UAE Dirham"),
        ["SAR"] = ("﷼", "Saudi Riyal"),
        ["TWD"] = ("NT$", "New Taiwan Dollar"),
        ["ARS"] = ("$", "Argentine Peso"),
        ["CLP"] = ("$", "Chilean Peso"),
        ["COP"] = ("$", "Colombian Peso"),
        ["EGP"] = ("£", "Egyptian Pound"),
        ["VND"] = ("₫", "Vietnamese Dong"),
        ["PKR"] = ("₨", "Pakistani Rupee"),
        ["BGN"] = ("лв", "Bulgarian Lev"),
        ["RON"] = ("lei", "Romanian Leu"),
        ["UAH"] = ("₴", "Ukrainian Hryvnia"),
        ["NGN"] = ("₦", "Nigerian Naira"),
        ["KES"] = ("KSh", "Kenyan Shilling"),
        ["QAR"] = ("﷼", "Qatari Rial"),
        ["KWD"] = ("د.ك", "Kuwaiti Dinar"),
        ["BHD"] = ("ب.د", "Bahraini Dinar"),
        ["OMR"] = ("﷼", "Omani Rial"),
    };
}