using Each.Abstractions;
using Each.Exceptions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed URL with built-in validation.
/// </summary>
/// <remarks>
/// This type ensures that only valid URLs can be created and stored.
/// It supports HTTP, HTTPS, FTP, and other common protocols.
/// </remarks>
public sealed partial class Url : ValueObject<string>
{
    /// <summary>
    /// The maximum allowed length for a URL.
    /// </summary>
    public const int MaxLength = 2083;

    private readonly Uri _uri;

    private Url(string value, Uri uri) : base(value)
    {
        _uri = uri;
    }

    /// <summary>
    /// Gets the scheme/protocol of the URL (e.g., "http", "https").
    /// </summary>
    public string Scheme => _uri.Scheme;

    /// <summary>
    /// Gets the host name of the URL.
    /// </summary>
    public string Host => _uri.Host;

    /// <summary>
    /// Gets the port number of the URL.
    /// </summary>
    public int Port => _uri.Port;

    /// <summary>
    /// Gets the path component of the URL.
    /// </summary>
    public string Path => _uri.AbsolutePath;

    /// <summary>
    /// Gets the query string of the URL.
    /// </summary>
    public string Query => _uri.Query;

    /// <summary>
    /// Gets the fragment/anchor of the URL.
    /// </summary>
    public string Fragment => _uri.Fragment;

    /// <summary>
    /// Gets a value indicating whether the URL uses HTTPS.
    /// </summary>
    public bool IsSecure => _uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the underlying <see cref="System.Uri"/> instance.
    /// </summary>
    public Uri ToUri()
    {
        return _uri;
    }

    /// <summary>
    /// Creates a new <see cref="Url"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <returns>A new <see cref="Url"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the URL is invalid.</exception>
    public static Url Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out Uri? uri);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Url), value, validationResult.ErrorMessage!);
        }
        return new Url(value, uri!);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Url"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The URL string.</param>
    /// <param name="url">When this method returns, contains the created URL if successful; otherwise, null.</param>
    /// <returns>true if the URL was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out Url? url)
    {
        ValidationResult validationResult = ValidateInternal(value, out Uri? uri);
        if (validationResult.IsValid)
        {
            url = new Url(value, uri!);
            return true;
        }
        url = null;
        return false;
    }

    /// <summary>
    /// Validates the specified URL string.
    /// </summary>
    /// <param name="value">The URL to validate.</param>
    /// <returns>A validation result indicating whether the URL is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _);
    }

    private static ValidationResult ValidateInternal(string value, out Uri? uri)
    {
        uri = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("URL cannot be null or empty.");
        }

        if (value.Length > MaxLength)
        {
            return ValidationResult.Failure($"URL cannot exceed {MaxLength} characters.");
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
        {
            return ValidationResult.Failure("URL format is invalid.");
        }

        if (!uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals("ftp", StringComparison.OrdinalIgnoreCase) &&
            !uri.Scheme.Equals("ftps", StringComparison.OrdinalIgnoreCase))
        {
            return ValidationResult.Failure("URL must use a supported protocol (http, https, ftp, ftps).");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified URL string is valid.
    /// </summary>
    /// <param name="value">The URL to check.</param>
    /// <returns>true if the URL is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="Url"/> instance.
    /// </summary>
    /// <param name="value">The URL string.</param>
    public static implicit operator Url(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Url"/> instance to a string.
    /// </summary>
    /// <param name="url">The URL instance.</param>
    public static implicit operator string(Url url)
    {
        return url.Value;
    }

    /// <summary>
    /// Converts a <see cref="Url"/> instance to a <see cref="System.Uri"/>.
    /// </summary>
    /// <param name="url">The URL instance.</param>
    public static implicit operator Uri(Url url)
    {
        return url._uri;
    }
}