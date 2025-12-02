using Each.Exceptions;
using System.Security.Cryptography;
using System.Text;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed password with built-in validation and hashing capabilities.
/// </summary>
/// <remarks>
/// This type ensures password security policies are enforced and provides 
/// secure hashing functionality. The original password is never stored.
/// </remarks>
public sealed class Password
{
    /// <summary>
    /// The default minimum length for passwords.
    /// </summary>
    public const int DefaultMinLength = 8;

    /// <summary>
    /// The default maximum length for passwords.
    /// </summary>
    public const int DefaultMaxLength = 128;
    private readonly byte[] _salt;

    private Password(string hash, byte[] salt)
    {
        Hash = hash;
        _salt = salt;
    }

    /// <summary>
    /// Gets the password hash (safe to store).
    /// </summary>
    public string Hash { get; }

    /// <summary>
    /// Gets the salt used for hashing (safe to store).
    /// </summary>
    public string Salt => Convert.ToBase64String(_salt);

    /// <summary>
    /// Creates a new <see cref="Password"/> instance from the specified plaintext password.
    /// </summary>
    /// <param name="plaintext">The plaintext password.</param>
    /// <param name="options">Optional password policy options.</param>
    /// <returns>A new <see cref="Password"/> instance with the hashed password.</returns>
    /// <exception cref="ValidationException">Thrown when the password doesn't meet requirements.</exception>
    public static Password Create(string plaintext, PasswordOptions? options = null)
    {
        options ??= PasswordOptions.Default;

        ValidationResult validationResult = Validate(plaintext, options);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Password), "***", validationResult.ErrorMessage!);
        }

        byte[] salt = GenerateSalt();
        string hash = HashPassword(plaintext, salt, options.Iterations);

        return new Password(hash, salt);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Password"/> instance from the specified plaintext password.
    /// </summary>
    /// <param name="plaintext">The plaintext password.</param>
    /// <param name="password">When this method returns, contains the created password if successful; otherwise, null.</param>
    /// <param name="options">Optional password policy options.</param>
    /// <returns>true if the password was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string plaintext, out Password? password, PasswordOptions? options = null)
    {
        options ??= PasswordOptions.Default;

        ValidationResult validationResult = Validate(plaintext, options);
        if (validationResult.IsValid)
        {
            byte[] salt = GenerateSalt();
            string hash = HashPassword(plaintext, salt, options.Iterations);
            password = new Password(hash, salt);
            return true;
        }
        password = null;
        return false;
    }

    /// <summary>
    /// Reconstructs a <see cref="Password"/> instance from a stored hash and salt.
    /// </summary>
    /// <param name="hash">The stored hash.</param>
    /// <param name="salt">The stored salt (Base64 encoded).</param>
    /// <returns>A <see cref="Password"/> instance.</returns>
    public static Password FromHash(string hash, string salt)
    {
        if (string.IsNullOrEmpty(hash))
        {
            throw new ValidationException(nameof(Password), null, "Hash cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(salt))
        {
            throw new ValidationException(nameof(Password), null, "Salt cannot be null or empty.");
        }

        return new Password(hash, Convert.FromBase64String(salt));
    }

    /// <summary>
    /// Verifies if the provided plaintext password matches this password's hash.
    /// </summary>
    /// <param name="plaintext">The plaintext password to verify.</param>
    /// <param name="iterations">The number of iterations used during hashing.</param>
    /// <returns>true if the password matches; otherwise, false.</returns>
    public bool Verify(string plaintext, int iterations = PasswordOptions.DefaultIterations)
    {
        if (string.IsNullOrEmpty(plaintext))
        {
            return false;
        }

        string hash = HashPassword(plaintext, _salt, iterations);
        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(Hash),
            Encoding.UTF8.GetBytes(hash));
    }

    /// <summary>
    /// Validates the specified plaintext password against the policy.
    /// </summary>
    /// <param name="plaintext">The password to validate.</param>
    /// <param name="options">Optional password policy options.</param>
    /// <returns>A validation result indicating whether the password is valid.</returns>
    public static ValidationResult Validate(string plaintext, PasswordOptions? options = null)
    {
        options ??= PasswordOptions.Default;

        if (string.IsNullOrWhiteSpace(plaintext))
        {
            return ValidationResult.Failure("Password cannot be null or empty.");
        }

        if (plaintext.Length < options.MinLength)
        {
            return ValidationResult.Failure($"Password must be at least {options.MinLength} characters.");
        }

        if (plaintext.Length > options.MaxLength)
        {
            return ValidationResult.Failure($"Password cannot exceed {options.MaxLength} characters.");
        }

        if (options.RequireUppercase && !plaintext.Any(char.IsUpper))
        {
            return ValidationResult.Failure("Password must contain at least one uppercase letter.");
        }

        if (options.RequireLowercase && !plaintext.Any(char.IsLower))
        {
            return ValidationResult.Failure("Password must contain at least one lowercase letter.");
        }

        if (options.RequireDigit && !plaintext.Any(char.IsDigit))
        {
            return ValidationResult.Failure("Password must contain at least one digit.");
        }

        if (options.RequireSpecialCharacter && !plaintext.Any(c => !char.IsLetterOrDigit(c)))
        {
            return ValidationResult.Failure("Password must contain at least one special character.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified plaintext password is valid.
    /// </summary>
    /// <param name="plaintext">The password to check.</param>
    /// <param name="options">Optional password policy options.</param>
    /// <returns>true if the password is valid; otherwise, false.</returns>
    public static bool IsValid(string plaintext, PasswordOptions? options = null)
    {
        return Validate(plaintext, options).IsValid;
    }

    private static byte[] GenerateSalt()
    {
        return RandomNumberGenerator.GetBytes(32);
    }

    private static string HashPassword(string password, byte[] salt, int iterations)
    {
        using Rfc2898DeriveBytes pbkdf2 = new(
            password,
            salt,
            iterations,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);
        return Convert.ToBase64String(hash);
    }
}

/// <summary>
/// Options for password validation policy.
/// </summary>
public sealed class PasswordOptions
{
    /// <summary>
    /// The default number of iterations for password hashing.
    /// </summary>
    public const int DefaultIterations = 100000;

    /// <summary>
    /// Gets or sets the minimum password length.
    /// </summary>
    public int MinLength { get; set; } = Password.DefaultMinLength;

    /// <summary>
    /// Gets or sets the maximum password length.
    /// </summary>
    public int MaxLength { get; set; } = Password.DefaultMaxLength;

    /// <summary>
    /// Gets or sets whether an uppercase letter is required.
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Gets or sets whether a lowercase letter is required.
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Gets or sets whether a digit is required.
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Gets or sets whether a special character is required.
    /// </summary>
    public bool RequireSpecialCharacter { get; set; } = true;

    /// <summary>
    /// Gets or sets the number of PBKDF2 iterations.
    /// </summary>
    public int Iterations { get; set; } = DefaultIterations;

    /// <summary>
    /// Gets the default password options.
    /// </summary>
    public static PasswordOptions Default => new();

    /// <summary>
    /// Gets password options with relaxed requirements (no special character required).
    /// </summary>
    public static PasswordOptions Relaxed => new()
    {
        RequireSpecialCharacter = false,
        MinLength = 6
    };

    /// <summary>
    /// Gets password options with strict requirements (longer minimum length).
    /// </summary>
    public static PasswordOptions Strict => new()
    {
        MinLength = 12,
        RequireSpecialCharacter = true,
        RequireDigit = true,
        RequireUppercase = true,
        RequireLowercase = true
    };
}