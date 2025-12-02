using Each.Abstractions;
using Each.Exceptions;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed hexadecimal color code with built-in validation.
/// </summary>
/// <remarks>
/// This type supports both 3-character (#RGB) and 6-character (#RRGGBB) hexadecimal color codes.
/// </remarks>
public sealed partial class HexColor : ValueObject<string>
{
    private HexColor(string value, byte red, byte green, byte blue) : base(NormalizeColor(value))
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    /// <summary>
    /// Gets the red component of the color (0-255).
    /// </summary>
    public byte Red { get; }

    /// <summary>
    /// Gets the green component of the color (0-255).
    /// </summary>
    public byte Green { get; }

    /// <summary>
    /// Gets the blue component of the color (0-255).
    /// </summary>
    public byte Blue { get; }

    /// <summary>
    /// Gets the color in #RRGGBB format.
    /// </summary>
    public string FullFormat => $"#{Red:X2}{Green:X2}{Blue:X2}";

    /// <summary>
    /// Gets the RGB string representation (e.g., "rgb(255, 128, 0)").
    /// </summary>
    public string RgbFormat => $"rgb({Red}, {Green}, {Blue})";

    /// <summary>
    /// Creates a new <see cref="HexColor"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The hex color string (with or without # prefix).</param>
    /// <returns>A new <see cref="HexColor"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the hex color is invalid.</exception>
    public static HexColor Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out byte red, out byte green, out byte blue);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(HexColor), value, validationResult.ErrorMessage!);
        }
        return new HexColor(value, red, green, blue);
    }

    /// <summary>
    /// Creates a new <see cref="HexColor"/> instance from RGB values.
    /// </summary>
    /// <param name="red">The red component (0-255).</param>
    /// <param name="green">The green component (0-255).</param>
    /// <param name="blue">The blue component (0-255).</param>
    /// <returns>A new <see cref="HexColor"/> instance.</returns>
    public static HexColor FromRgb(byte red, byte green, byte blue)
    {
        string hex = $"#{red:X2}{green:X2}{blue:X2}";
        return new HexColor(hex, red, green, blue);
    }

    /// <summary>
    /// Attempts to create a new <see cref="HexColor"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The hex color string.</param>
    /// <param name="color">When this method returns, contains the created color if successful; otherwise, null.</param>
    /// <returns>true if the color was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out HexColor? color)
    {
        ValidationResult validationResult = ValidateInternal(value, out byte red, out byte green, out byte blue);
        if (validationResult.IsValid)
        {
            color = new HexColor(value, red, green, blue);
            return true;
        }
        color = null;
        return false;
    }

    /// <summary>
    /// Validates the specified hex color string.
    /// </summary>
    /// <param name="value">The color to validate.</param>
    /// <returns>A validation result indicating whether the color is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _, out _, out _);
    }

    private static ValidationResult ValidateInternal(string value, out byte red, out byte green, out byte blue)
    {
        red = 0;
        green = 0;
        blue = 0;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Hex color cannot be null or empty.");
        }

        string normalized = value.TrimStart('#').ToUpperInvariant();

        if (normalized.Length == 3)
        {
            // Expand shorthand (#RGB -> #RRGGBB)
            normalized = $"{normalized[0]}{normalized[0]}{normalized[1]}{normalized[1]}{normalized[2]}{normalized[2]}";
        }

        if (normalized.Length != 6)
        {
            return ValidationResult.Failure("Hex color must be 3 or 6 characters (excluding # prefix).");
        }

        if (!HexColorRegex().IsMatch(normalized))
        {
            return ValidationResult.Failure("Hex color must contain only hexadecimal characters (0-9, A-F).");
        }

        red = Convert.ToByte(normalized[..2], 16);
        green = Convert.ToByte(normalized.Substring(2, 2), 16);
        blue = Convert.ToByte(normalized.Substring(4, 2), 16);

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified hex color string is valid.
    /// </summary>
    /// <param name="value">The color to check.</param>
    /// <returns>true if the color is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    private static string NormalizeColor(string value)
    {
        string normalized = value.TrimStart('#').ToUpperInvariant();
        if (normalized.Length == 3)
        {
            normalized = $"{normalized[0]}{normalized[0]}{normalized[1]}{normalized[1]}{normalized[2]}{normalized[2]}";
        }
        return $"#{normalized}";
    }

    // Common color presets
    /// <summary>Gets the color white (#FFFFFF).</summary>
    public static HexColor White => FromRgb(255, 255, 255);

    /// <summary>Gets the color black (#000000).</summary>
    public static HexColor Black => FromRgb(0, 0, 0);

    /// <summary>Gets the color red (#FF0000).</summary>
    public static HexColor ColorRed => FromRgb(255, 0, 0);

    /// <summary>Gets the color green (#00FF00).</summary>
    public static HexColor ColorGreen => FromRgb(0, 255, 0);

    /// <summary>Gets the color blue (#0000FF).</summary>
    public static HexColor ColorBlue => FromRgb(0, 0, 255);

    /// <summary>Gets the color yellow (#FFFF00).</summary>
    public static HexColor Yellow => FromRgb(255, 255, 0);

    /// <summary>Gets the color cyan (#00FFFF).</summary>
    public static HexColor Cyan => FromRgb(0, 255, 255);

    /// <summary>Gets the color magenta (#FF00FF).</summary>
    public static HexColor Magenta => FromRgb(255, 0, 255);

    /// <summary>
    /// Implicitly converts a string to a <see cref="HexColor"/> instance.
    /// </summary>
    /// <param name="value">The hex color string.</param>
    public static implicit operator HexColor(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="HexColor"/> instance to a string.
    /// </summary>
    /// <param name="color">The color instance.</param>
    public static implicit operator string(HexColor color)
    {
        return color.Value;
    }

    [GeneratedRegex(@"^[0-9A-F]{6}$", RegexOptions.Compiled)]
    private static partial Regex HexColorRegex();
}