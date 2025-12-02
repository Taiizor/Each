using Each.Abstractions;
using Each.Exceptions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed UUID/GUID with built-in validation.
/// </summary>
/// <remarks>
/// This type provides additional functionality over <see cref="Guid"/> including
/// various format options and version detection.
/// </remarks>
public sealed partial class UniqueId : ValueObject<Guid>
{
    private UniqueId(Guid value) : base(value)
    {
    }

    /// <summary>
    /// Gets the UUID as a standard formatted string (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).
    /// </summary>
    public string Standard => Value.ToString("D");

    /// <summary>
    /// Gets the UUID without hyphens.
    /// </summary>
    public string WithoutHyphens => Value.ToString("N");

    /// <summary>
    /// Gets the UUID in braces format ({xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}).
    /// </summary>
    public string WithBraces => Value.ToString("B");

    /// <summary>
    /// Gets the UUID in parentheses format ((xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)).
    /// </summary>
    public string WithParentheses => Value.ToString("P");

    /// <summary>
    /// Gets the UUID version (1-5, or 0 if unknown).
    /// </summary>
    public int Version
    {
        get
        {
            byte[] bytes = Value.ToByteArray();
            return (bytes[7] >> 4) & 0x0F;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this is an empty UUID.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Creates a new <see cref="UniqueId"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The UUID string.</param>
    /// <returns>A new <see cref="UniqueId"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the UUID is invalid.</exception>
    public static UniqueId Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out Guid guid);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(UniqueId), value, validationResult.ErrorMessage!);
        }
        return new UniqueId(guid);
    }

    /// <summary>
    /// Creates a new <see cref="UniqueId"/> instance from the specified <see cref="Guid"/>.
    /// </summary>
    /// <param name="guid">The GUID value.</param>
    /// <returns>A new <see cref="UniqueId"/> instance.</returns>
    public static UniqueId Create(Guid guid)
    {
        return new UniqueId(guid);
    }

    /// <summary>
    /// Generates a new random <see cref="UniqueId"/>.
    /// </summary>
    /// <returns>A new randomly generated <see cref="UniqueId"/>.</returns>
    public static UniqueId NewUniqueId()
    {
        return new UniqueId(Guid.NewGuid());
    }

    /// <summary>
    /// Attempts to create a new <see cref="UniqueId"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The UUID string.</param>
    /// <param name="uniqueId">When this method returns, contains the created UUID if successful; otherwise, null.</param>
    /// <returns>true if the UUID was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out UniqueId? uniqueId)
    {
        ValidationResult validationResult = ValidateInternal(value, out Guid guid);
        if (validationResult.IsValid)
        {
            uniqueId = new UniqueId(guid);
            return true;
        }
        uniqueId = null;
        return false;
    }

    /// <summary>
    /// Validates the specified UUID string.
    /// </summary>
    /// <param name="value">The UUID to validate.</param>
    /// <returns>A validation result indicating whether the UUID is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _);
    }

    private static ValidationResult ValidateInternal(string value, out Guid guid)
    {
        guid = Guid.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("UUID cannot be null or empty.");
        }

        if (!Guid.TryParse(value, out guid))
        {
            return ValidationResult.Failure("Invalid UUID format.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified UUID string is valid.
    /// </summary>
    /// <param name="value">The UUID to check.</param>
    /// <returns>true if the UUID is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Gets an empty UUID.
    /// </summary>
    public static UniqueId Empty => new(Guid.Empty);

    /// <summary>
    /// Returns the string representation of this UUID.
    /// </summary>
    public override string ToString()
    {
        return Standard;
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="UniqueId"/> instance.
    /// </summary>
    /// <param name="value">The UUID string.</param>
    public static implicit operator UniqueId(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="UniqueId"/> instance to a string.
    /// </summary>
    /// <param name="uniqueId">The UUID instance.</param>
    public static implicit operator string(UniqueId uniqueId)
    {
        return uniqueId.Standard;
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="UniqueId"/> instance.
    /// </summary>
    /// <param name="guid">The GUID value.</param>
    public static implicit operator UniqueId(Guid guid)
    {
        return Create(guid);
    }

    /// <summary>
    /// Implicitly converts a <see cref="UniqueId"/> instance to a <see cref="Guid"/>.
    /// </summary>
    /// <param name="uniqueId">The UUID instance.</param>
    public static implicit operator Guid(UniqueId uniqueId)
    {
        return uniqueId.Value;
    }
}