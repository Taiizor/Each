namespace Each;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>
    /// Gets a value indicating whether the validation was successful.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Gets the error message if validation failed; otherwise, null.
    /// </summary>
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful validation result.
    /// </summary>
    /// <returns>A successful validation result.</returns>
    public static ValidationResult Success()
    {
        return new(true, null);
    }

    /// <summary>
    /// Creates a failed validation result with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the validation failure.</param>
    /// <returns>A failed validation result.</returns>
    public static ValidationResult Failure(string errorMessage)
    {
        return new(false, errorMessage);
    }

    /// <summary>
    /// Implicitly converts a validation result to a boolean value.
    /// </summary>
    /// <param name="result">The validation result.</param>
    public static implicit operator bool(ValidationResult result)
    {
        return result.IsValid;
    }
}