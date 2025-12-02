namespace Each.Exceptions;

/// <summary>
/// Exception thrown when validation of a value object fails.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the name of the type that failed validation.
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// Gets the value that failed validation.
    /// </summary>
    public string? AttemptedValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="typeName">The name of the type that failed validation.</param>
    /// <param name="attemptedValue">The value that failed validation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ValidationException(string typeName, string? attemptedValue, string message)
        : base(message)
    {
        TypeName = typeName;
        AttemptedValue = attemptedValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class.
    /// </summary>
    /// <param name="typeName">The name of the type that failed validation.</param>
    /// <param name="attemptedValue">The value that failed validation.</param>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ValidationException(string typeName, string? attemptedValue, string message, Exception innerException)
        : base(message, innerException)
    {
        TypeName = typeName;
        AttemptedValue = attemptedValue;
    }
}