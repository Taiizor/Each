using Each.Abstractions;
using Each.Exceptions;
using System.Text.RegularExpressions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed Semantic Version with built-in validation.
/// </summary>
/// <remarks>
/// This type follows the Semantic Versioning 2.0.0 specification (https://semver.org/).
/// </remarks>
public sealed partial class SemanticVersion : ValueObject<string>, IComparable<SemanticVersion>
{
    /// <summary>
    /// Gets the major version number.
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// Gets the minor version number.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    /// Gets the patch version number.
    /// </summary>
    public int Patch { get; }

    /// <summary>
    /// Gets the pre-release label (e.g., "alpha", "beta.1").
    /// </summary>
    public string? PreRelease { get; }

    /// <summary>
    /// Gets the build metadata.
    /// </summary>
    public string? BuildMetadata { get; }

    /// <summary>
    /// Gets a value indicating whether this is a pre-release version.
    /// </summary>
    public bool IsPreRelease => !string.IsNullOrEmpty(PreRelease);

    private SemanticVersion(string value, int major, int minor, int patch, string? preRelease, string? buildMetadata)
        : base(value)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
        PreRelease = preRelease;
        BuildMetadata = buildMetadata;
    }

    /// <summary>
    /// Creates a new <see cref="SemanticVersion"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The semantic version string.</param>
    /// <returns>A new <see cref="SemanticVersion"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the semantic version is invalid.</exception>
    public static SemanticVersion Create(string value)
    {
        ValidationResult validationResult = ValidateInternal(value, out int major, out int minor, out int patch, out string? preRelease, out string? buildMetadata);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(SemanticVersion), value, validationResult.ErrorMessage!);
        }
        return new SemanticVersion(value, major, minor, patch, preRelease, buildMetadata);
    }

    /// <summary>
    /// Creates a new <see cref="SemanticVersion"/> instance from individual components.
    /// </summary>
    /// <param name="major">The major version number.</param>
    /// <param name="minor">The minor version number.</param>
    /// <param name="patch">The patch version number.</param>
    /// <param name="preRelease">The pre-release label (optional).</param>
    /// <param name="buildMetadata">The build metadata (optional).</param>
    /// <returns>A new <see cref="SemanticVersion"/> instance.</returns>
    public static SemanticVersion Create(int major, int minor, int patch, string? preRelease = null, string? buildMetadata = null)
    {
        if (major < 0 || minor < 0 || patch < 0)
        {
            throw new ValidationException(nameof(SemanticVersion), $"{major}.{minor}.{patch}", "Version numbers cannot be negative.");
        }

        string versionString = $"{major}.{minor}.{patch}";
        if (!string.IsNullOrEmpty(preRelease))
        {
            versionString += $"-{preRelease}";
        }
        if (!string.IsNullOrEmpty(buildMetadata))
        {
            versionString += $"+{buildMetadata}";
        }

        return new SemanticVersion(versionString, major, minor, patch, preRelease, buildMetadata);
    }

    /// <summary>
    /// Attempts to create a new <see cref="SemanticVersion"/> instance from the specified string value.
    /// </summary>
    /// <param name="value">The semantic version string.</param>
    /// <param name="version">When this method returns, contains the created version if successful; otherwise, null.</param>
    /// <returns>true if the version was created successfully; otherwise, false.</returns>
    public static bool TryCreate(string value, out SemanticVersion? version)
    {
        ValidationResult validationResult = ValidateInternal(value, out int major, out int minor, out int patch, out string? preRelease, out string? buildMetadata);
        if (validationResult.IsValid)
        {
            version = new SemanticVersion(value, major, minor, patch, preRelease, buildMetadata);
            return true;
        }
        version = null;
        return false;
    }

    /// <summary>
    /// Validates the specified semantic version string.
    /// </summary>
    /// <param name="value">The version to validate.</param>
    /// <returns>A validation result indicating whether the version is valid.</returns>
    public static ValidationResult Validate(string value)
    {
        return ValidateInternal(value, out _, out _, out _, out _, out _);
    }

    private static ValidationResult ValidateInternal(string value, out int major, out int minor, out int patch, out string? preRelease, out string? buildMetadata)
    {
        major = 0;
        minor = 0;
        patch = 0;
        preRelease = null;
        buildMetadata = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Failure("Semantic version cannot be null or empty.");
        }

        Match match = SemanticVersionRegex().Match(value);
        if (!match.Success)
        {
            return ValidationResult.Failure("Invalid semantic version format.");
        }

        major = int.Parse(match.Groups["major"].Value);
        minor = int.Parse(match.Groups["minor"].Value);
        patch = int.Parse(match.Groups["patch"].Value);
        preRelease = match.Groups["prerelease"].Success ? match.Groups["prerelease"].Value : null;
        buildMetadata = match.Groups["buildmetadata"].Success ? match.Groups["buildmetadata"].Value : null;

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified semantic version string is valid.
    /// </summary>
    /// <param name="value">The version to check.</param>
    /// <returns>true if the version is valid; otherwise, false.</returns>
    public static bool IsValid(string value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Creates a new version with the major version incremented.
    /// </summary>
    public SemanticVersion IncrementMajor()
    {
        return Create(Major + 1, 0, 0);
    }

    /// <summary>
    /// Creates a new version with the minor version incremented.
    /// </summary>
    public SemanticVersion IncrementMinor()
    {
        return Create(Major, Minor + 1, 0);
    }

    /// <summary>
    /// Creates a new version with the patch version incremented.
    /// </summary>
    public SemanticVersion IncrementPatch()
    {
        return Create(Major, Minor, Patch + 1);
    }

    /// <summary>
    /// Compares this version with another version.
    /// </summary>
    public int CompareTo(SemanticVersion? other)
    {
        if (other is null)
        {
            return 1;
        }

        int result = Major.CompareTo(other.Major);
        if (result != 0)
        {
            return result;
        }

        result = Minor.CompareTo(other.Minor);
        if (result != 0)
        {
            return result;
        }

        result = Patch.CompareTo(other.Patch);
        if (result != 0)
        {
            return result;
        }

        // Pre-release versions have lower precedence
        if (IsPreRelease && !other.IsPreRelease)
        {
            return -1;
        }

        if (!IsPreRelease && other.IsPreRelease)
        {
            return 1;
        }

        return string.Compare(PreRelease, other.PreRelease, StringComparison.Ordinal);
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="SemanticVersion"/> instance.
    /// </summary>
    /// <param name="value">The version string.</param>
    public static implicit operator SemanticVersion(string value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="SemanticVersion"/> instance to a string.
    /// </summary>
    /// <param name="version">The version instance.</param>
    public static implicit operator string(SemanticVersion version)
    {
        return version.Value;
    }

    [GeneratedRegex(@"^(?<major>0|[1-9]\d*)\.(?<minor>0|[1-9]\d*)\.(?<patch>0|[1-9]\d*)(?:-(?<prerelease>(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?<buildmetadata>[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$", RegexOptions.Compiled)]
    private static partial Regex SemanticVersionRegex();
}