using Each.Abstractions;
using Each.Exceptions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed percentage value with built-in validation.
/// </summary>
/// <remarks>
/// This type represents a percentage value between 0 and 100 (or optionally allowing values above 100).
/// </remarks>
public sealed class Percentage : ValueObject<decimal>
{
    /// <summary>
    /// The minimum value for a standard percentage (0%).
    /// </summary>
    public const decimal MinValue = 0m;

    /// <summary>
    /// The maximum value for a standard percentage (100%).
    /// </summary>
    public const decimal MaxValue = 100m;

    private Percentage(decimal value) : base(value)
    {
    }

    /// <summary>
    /// Gets the percentage as a decimal fraction (0.0 to 1.0 for 0% to 100%).
    /// </summary>
    public decimal AsFraction => Value / 100m;

    /// <summary>
    /// Gets the formatted percentage string (e.g., "50%").
    /// </summary>
    public string Formatted => $"{Value:0.##}%";

    /// <summary>
    /// Creates a new <see cref="Percentage"/> instance from the specified decimal value.
    /// </summary>
    /// <param name="value">The percentage value (0-100).</param>
    /// <param name="allowAbove100">Whether to allow values above 100.</param>
    /// <returns>A new <see cref="Percentage"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the percentage is invalid.</exception>
    public static Percentage Create(decimal value, bool allowAbove100 = false)
    {
        ValidationResult validationResult = Validate(value, allowAbove100);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Percentage), value.ToString(), validationResult.ErrorMessage!);
        }
        return new Percentage(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Percentage"/> instance from the specified decimal value.
    /// </summary>
    /// <param name="value">The percentage value.</param>
    /// <param name="percentage">When this method returns, contains the created percentage if successful; otherwise, null.</param>
    /// <param name="allowAbove100">Whether to allow values above 100.</param>
    /// <returns>true if the percentage was created successfully; otherwise, false.</returns>
    public static bool TryCreate(decimal value, out Percentage? percentage, bool allowAbove100 = false)
    {
        ValidationResult validationResult = Validate(value, allowAbove100);
        if (validationResult.IsValid)
        {
            percentage = new Percentage(value);
            return true;
        }
        percentage = null;
        return false;
    }

    /// <summary>
    /// Creates a <see cref="Percentage"/> from a fraction value (0.0 to 1.0).
    /// </summary>
    /// <param name="fraction">The fraction value.</param>
    /// <param name="allowAbove100">Whether to allow values above 100%.</param>
    /// <returns>A new <see cref="Percentage"/> instance.</returns>
    public static Percentage FromFraction(decimal fraction, bool allowAbove100 = false)
    {
        return Create(fraction * 100m, allowAbove100);
    }

    /// <summary>
    /// Validates the specified percentage value.
    /// </summary>
    /// <param name="value">The percentage to validate.</param>
    /// <param name="allowAbove100">Whether to allow values above 100.</param>
    /// <returns>A validation result indicating whether the percentage is valid.</returns>
    public static ValidationResult Validate(decimal value, bool allowAbove100 = false)
    {
        if (value < MinValue)
        {
            return ValidationResult.Failure($"Percentage cannot be less than {MinValue}%.");
        }

        if (!allowAbove100 && value > MaxValue)
        {
            return ValidationResult.Failure($"Percentage cannot exceed {MaxValue}%.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified percentage value is valid.
    /// </summary>
    /// <param name="value">The percentage to check.</param>
    /// <param name="allowAbove100">Whether to allow values above 100.</param>
    /// <returns>true if the percentage is valid; otherwise, false.</returns>
    public static bool IsValid(decimal value, bool allowAbove100 = false)
    {
        return Validate(value, allowAbove100).IsValid;
    }

    /// <summary>
    /// Gets a percentage representing 0%.
    /// </summary>
    public static Percentage Zero => new(0m);

    /// <summary>
    /// Gets a percentage representing 50%.
    /// </summary>
    public static Percentage Half => new(50m);

    /// <summary>
    /// Gets a percentage representing 100%.
    /// </summary>
    public static Percentage Full => new(100m);

    /// <summary>
    /// Adds two percentages.
    /// </summary>
    public static Percentage operator +(Percentage left, Percentage right)
    {
        return new Percentage(left.Value + right.Value);
    }

    /// <summary>
    /// Subtracts one percentage from another.
    /// </summary>
    public static Percentage operator -(Percentage left, Percentage right)
    {
        return new Percentage(left.Value - right.Value);
    }

    /// <summary>
    /// Multiplies a percentage by a scalar.
    /// </summary>
    public static Percentage operator *(Percentage percentage, decimal scalar)
    {
        return new Percentage(percentage.Value * scalar);
    }

    /// <summary>
    /// Divides a percentage by a scalar.
    /// </summary>
    public static Percentage operator /(Percentage percentage, decimal scalar)
    {
        if (scalar == 0)
        {
            throw new DivideByZeroException("Cannot divide percentage by zero.");
        }
        return new Percentage(percentage.Value / scalar);
    }

    /// <summary>
    /// Implicitly converts a decimal to a <see cref="Percentage"/> instance.
    /// </summary>
    /// <param name="value">The percentage value.</param>
    public static implicit operator Percentage(decimal value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Percentage"/> instance to a decimal.
    /// </summary>
    /// <param name="percentage">The percentage instance.</param>
    public static implicit operator decimal(Percentage percentage)
    {
        return percentage.Value;
    }

    /// <summary>
    /// Returns a string representation of the percentage.
    /// </summary>
    public override string ToString()
    {
        return Formatted;
    }
}