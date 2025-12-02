# Each

[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-7.0+-purple.svg)](https://dotnet.microsoft.com/)

**Each** is a comprehensive C# library providing strongly-typed value objects for common data types like Email, PhoneNumber, URL, CreditCard, IBAN, and more. Each type includes built-in validation, formatting, and parsing capabilities.

## Features

- ✅ **Strongly-typed value objects** - Compile-time type safety for common data types
- ✅ **Built-in validation** - Automatic validation with detailed error messages
- ✅ **Immutable** - All types are immutable by design
- ✅ **Domain-Driven Design ready** - Perfect for DDD applications
- ✅ **Full XML documentation** - IntelliSense support for all public APIs
- ✅ **Implicit conversions** - Seamless integration with existing code
- ✅ **Zero dependencies** - No external package dependencies

## Installation

```bash
dotnet add package Each
```

## Quick Start

```csharp
using Each.Types;

// Email
Email email = "user@example.com";
Console.WriteLine(email.LocalPart);  // "user"
Console.WriteLine(email.Domain);     // "example.com"

// Phone Number
PhoneNumber phone = "+1234567890";
Console.WriteLine(phone.E164Format);  // "+1234567890"

// URL
Url url = "https://example.com/api/users";
Console.WriteLine(url.Host);      // "example.com"
Console.WriteLine(url.IsSecure);  // true

// Credit Card
CreditCard card = CreditCard.Create("4111111111111111");
Console.WriteLine(card.CardType);      // Visa
Console.WriteLine(card.Masked);        // "****-****-****-1111"

// Currency
var price = Currency.USD(99.99m);
var discount = price * 0.1m;
Console.WriteLine(discount.Formatted);  // "$10.00 USD"

// Password
var password = Password.Create("SecurePassword123!");
bool isValid = password.Verify("SecurePassword123!");  // true
```

## Supported Types

| Type | Description | Example |
|------|-------------|---------|
| `Email` | Email address with RFC 5322 validation | `user@example.com` |
| `PhoneNumber` | Phone number with E.164 support | `+1234567890` |
| `Url` | URL with protocol validation | `https://example.com` |
| `CreditCard` | Credit card with Luhn validation | `4111111111111111` |
| `Iban` | IBAN with checksum validation | `DE89370400440532013000` |
| `IPv4Address` | IPv4 address | `192.168.1.1` |
| `IPv6Address` | IPv6 address | `::1` |
| `PostalCode` | Country-specific postal codes | `90210` |
| `Currency` | Monetary values with currency codes | `$99.99 USD` |
| `Percentage` | Percentage values (0-100%) | `50%` |
| `SemanticVersion` | SemVer 2.0 compliant versions | `1.2.3-beta.1` |
| `HexColor` | Hexadecimal color codes | `#FF5733` |
| `UniqueId` | UUID/GUID wrapper | `550e8400-e29b-41d4-a716-446655440000` |
| `Password` | Secure password with hashing | Hashed value |
| `Latitude` | Geographic latitude (-90 to 90) | `40.7128` |
| `Longitude` | Geographic longitude (-180 to 180) | `-74.0060` |
| `GeoCoordinate` | Latitude/Longitude pair | `40.7128, -74.0060` |

## Usage Examples

### Email Validation

```csharp
// Create with validation
Email email = Email.Create("user@example.com");

// Try pattern (no exception)
if (Email.TryCreate("user@example.com", out var validEmail))
{
    Console.WriteLine(validEmail.Domain);
}

// Check if valid without creating
bool isValid = Email.IsValid("user@example.com");

// Get validation result with error message
ValidationResult result = Email.Validate("invalid-email");
if (!result.IsValid)
{
    Console.WriteLine(result.ErrorMessage);
}
```

### Credit Card Processing

```csharp
var card = CreditCard.Create("4111111111111111");

Console.WriteLine(card.CardType);      // Visa
Console.WriteLine(card.LastFourDigits); // "1111"
Console.WriteLine(card.Masked);         // "****-****-****-1111"

// Detect card type without full validation
var type = CreditCard.GetCardType("4111...");  // Visa
```

### Currency Operations

```csharp
var price = Currency.USD(100.00m);
var tax = Currency.USD(8.50m);
var total = price + tax;  // $108.50 USD

// Different currencies cannot be added
var euros = Currency.EUR(50.00m);
// var invalid = price + euros;  // Throws InvalidOperationException

// Factory methods for common currencies
var usd = Currency.USD(100);
var eur = Currency.EUR(100);
var gbp = Currency.GBP(100);
var jpy = Currency.JPY(1000);
var tryLira = Currency.TRY(100);
```

### Password Security

```csharp
// Create with default policy
var password = Password.Create("SecurePassword123!");

// Store hash and salt
string hash = password.Hash;
string salt = password.Salt;

// Later, verify
var storedPassword = Password.FromHash(hash, salt);
bool isValid = storedPassword.Verify("SecurePassword123!");

// Custom password policy
var options = new PasswordOptions
{
    MinLength = 12,
    RequireUppercase = true,
    RequireLowercase = true,
    RequireDigit = true,
    RequireSpecialCharacter = true
};
var strongPassword = Password.Create("VerySecure123!", options);
```

### Geographic Coordinates

```csharp
// Create coordinates
var nyc = GeoCoordinate.Create(40.7128, -74.0060);
var london = GeoCoordinate.Create(51.5074, -0.1278);

// Calculate distance
double distanceKm = nyc.DistanceTo(london, DistanceUnit.Kilometers);
double distanceMiles = nyc.DistanceTo(london, DistanceUnit.Miles);

// Format as DMS
Console.WriteLine(nyc.ToDmsString());  // 40°42'46.08"N 74°0'21.60"W
```

### Semantic Versioning

```csharp
var version = SemanticVersion.Create("1.2.3-beta.1+build.123");

Console.WriteLine(version.Major);       // 1
Console.WriteLine(version.Minor);       // 2
Console.WriteLine(version.Patch);       // 3
Console.WriteLine(version.PreRelease);  // "beta.1"
Console.WriteLine(version.IsPreRelease); // true

// Increment versions
var nextMajor = version.IncrementMajor();  // 2.0.0
var nextMinor = version.IncrementMinor();  // 1.3.0
var nextPatch = version.IncrementPatch();  // 1.2.4

// Version comparison
var v1 = SemanticVersion.Create("1.0.0");
var v2 = SemanticVersion.Create("2.0.0");
bool isNewer = v2 > v1;  // true
```

## Error Handling

All types provide three patterns for handling validation:

```csharp
// 1. Create (throws ValidationException)
try
{
    Email email = Email.Create("invalid");
}
catch (ValidationException ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.TypeName);        // "Email"
    Console.WriteLine(ex.AttemptedValue);  // "invalid"
}

// 2. TryCreate (returns bool)
if (Email.TryCreate("test@example.com", out var email))
{
    // Use email
}

// 3. Validate (returns ValidationResult)
var result = Email.Validate("invalid");
if (!result.IsValid)
{
    Console.WriteLine(result.ErrorMessage);
}
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

Built with ❤️ by [Taiizor](https://github.com/Taiizor)