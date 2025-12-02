using Each.Abstractions;
using Each.Exceptions;

namespace Each.Types;

/// <summary>
/// Represents a strongly-typed latitude coordinate with built-in validation.
/// </summary>
public sealed class Latitude : ValueObject<double>
{
    /// <summary>
    /// The minimum valid latitude value.
    /// </summary>
    public const double MinValue = -90.0;

    /// <summary>
    /// The maximum valid latitude value.
    /// </summary>
    public const double MaxValue = 90.0;

    private Latitude(double value) : base(value)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this latitude is in the Northern Hemisphere.
    /// </summary>
    public bool IsNorthernHemisphere => Value >= 0;

    /// <summary>
    /// Gets a value indicating whether this latitude is in the Southern Hemisphere.
    /// </summary>
    public bool IsSouthernHemisphere => Value < 0;

    /// <summary>
    /// Gets the degrees, minutes, seconds representation.
    /// </summary>
    public (int Degrees, int Minutes, double Seconds, char Direction) ToDms()
    {
        double absolute = Math.Abs(Value);
        int degrees = (int)absolute;
        double minutesDecimal = (absolute - degrees) * 60;
        int minutes = (int)minutesDecimal;
        double seconds = (minutesDecimal - minutes) * 60;
        char direction = Value >= 0 ? 'N' : 'S';

        return (degrees, minutes, seconds, direction);
    }

    /// <summary>
    /// Creates a new <see cref="Latitude"/> instance from the specified double value.
    /// </summary>
    /// <param name="value">The latitude value in decimal degrees.</param>
    /// <returns>A new <see cref="Latitude"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the latitude is invalid.</exception>
    public static Latitude Create(double value)
    {
        ValidationResult validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Latitude), value.ToString(), validationResult.ErrorMessage!);
        }
        return new Latitude(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Latitude"/> instance from the specified double value.
    /// </summary>
    /// <param name="value">The latitude value in decimal degrees.</param>
    /// <param name="latitude">When this method returns, contains the created latitude if successful; otherwise, null.</param>
    /// <returns>true if the latitude was created successfully; otherwise, false.</returns>
    public static bool TryCreate(double value, out Latitude? latitude)
    {
        ValidationResult validationResult = Validate(value);
        if (validationResult.IsValid)
        {
            latitude = new Latitude(value);
            return true;
        }
        latitude = null;
        return false;
    }

    /// <summary>
    /// Validates the specified latitude value.
    /// </summary>
    /// <param name="value">The latitude to validate.</param>
    /// <returns>A validation result indicating whether the latitude is valid.</returns>
    public static ValidationResult Validate(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return ValidationResult.Failure("Latitude must be a valid number.");
        }

        if (value is < MinValue or > MaxValue)
        {
            return ValidationResult.Failure($"Latitude must be between {MinValue} and {MaxValue} degrees.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified latitude value is valid.
    /// </summary>
    /// <param name="value">The latitude to check.</param>
    /// <returns>true if the latitude is valid; otherwise, false.</returns>
    public static bool IsValid(double value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Implicitly converts a double to a <see cref="Latitude"/> instance.
    /// </summary>
    public static implicit operator Latitude(double value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Latitude"/> instance to a double.
    /// </summary>
    public static implicit operator double(Latitude latitude)
    {
        return latitude.Value;
    }
}

/// <summary>
/// Represents a strongly-typed longitude coordinate with built-in validation.
/// </summary>
public sealed class Longitude : ValueObject<double>
{
    /// <summary>
    /// The minimum valid longitude value.
    /// </summary>
    public const double MinValue = -180.0;

    /// <summary>
    /// The maximum valid longitude value.
    /// </summary>
    public const double MaxValue = 180.0;

    private Longitude(double value) : base(value)
    {
    }

    /// <summary>
    /// Gets a value indicating whether this longitude is in the Eastern Hemisphere.
    /// </summary>
    public bool IsEasternHemisphere => Value >= 0;

    /// <summary>
    /// Gets a value indicating whether this longitude is in the Western Hemisphere.
    /// </summary>
    public bool IsWesternHemisphere => Value < 0;

    /// <summary>
    /// Gets the degrees, minutes, seconds representation.
    /// </summary>
    public (int Degrees, int Minutes, double Seconds, char Direction) ToDms()
    {
        double absolute = Math.Abs(Value);
        int degrees = (int)absolute;
        double minutesDecimal = (absolute - degrees) * 60;
        int minutes = (int)minutesDecimal;
        double seconds = (minutesDecimal - minutes) * 60;
        char direction = Value >= 0 ? 'E' : 'W';

        return (degrees, minutes, seconds, direction);
    }

    /// <summary>
    /// Creates a new <see cref="Longitude"/> instance from the specified double value.
    /// </summary>
    /// <param name="value">The longitude value in decimal degrees.</param>
    /// <returns>A new <see cref="Longitude"/> instance.</returns>
    /// <exception cref="ValidationException">Thrown when the longitude is invalid.</exception>
    public static Longitude Create(double value)
    {
        ValidationResult validationResult = Validate(value);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(nameof(Longitude), value.ToString(), validationResult.ErrorMessage!);
        }
        return new Longitude(value);
    }

    /// <summary>
    /// Attempts to create a new <see cref="Longitude"/> instance from the specified double value.
    /// </summary>
    /// <param name="value">The longitude value in decimal degrees.</param>
    /// <param name="longitude">When this method returns, contains the created longitude if successful; otherwise, null.</param>
    /// <returns>true if the longitude was created successfully; otherwise, false.</returns>
    public static bool TryCreate(double value, out Longitude? longitude)
    {
        ValidationResult validationResult = Validate(value);
        if (validationResult.IsValid)
        {
            longitude = new Longitude(value);
            return true;
        }
        longitude = null;
        return false;
    }

    /// <summary>
    /// Validates the specified longitude value.
    /// </summary>
    /// <param name="value">The longitude to validate.</param>
    /// <returns>A validation result indicating whether the longitude is valid.</returns>
    public static ValidationResult Validate(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return ValidationResult.Failure("Longitude must be a valid number.");
        }

        if (value is < MinValue or > MaxValue)
        {
            return ValidationResult.Failure($"Longitude must be between {MinValue} and {MaxValue} degrees.");
        }

        return ValidationResult.Success();
    }

    /// <summary>
    /// Determines whether the specified longitude value is valid.
    /// </summary>
    /// <param name="value">The longitude to check.</param>
    /// <returns>true if the longitude is valid; otherwise, false.</returns>
    public static bool IsValid(double value)
    {
        return Validate(value).IsValid;
    }

    /// <summary>
    /// Implicitly converts a double to a <see cref="Longitude"/> instance.
    /// </summary>
    public static implicit operator Longitude(double value)
    {
        return Create(value);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Longitude"/> instance to a double.
    /// </summary>
    public static implicit operator double(Longitude longitude)
    {
        return longitude.Value;
    }
}

/// <summary>
/// Represents a strongly-typed geographic coordinate (latitude and longitude pair).
/// </summary>
public sealed class GeoCoordinate
{
    /// <summary>
    /// Gets the latitude component.
    /// </summary>
    public Latitude Latitude { get; }

    /// <summary>
    /// Gets the longitude component.
    /// </summary>
    public Longitude Longitude { get; }

    private GeoCoordinate(Latitude latitude, Longitude longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>
    /// Creates a new <see cref="GeoCoordinate"/> instance.
    /// </summary>
    /// <param name="latitude">The latitude value.</param>
    /// <param name="longitude">The longitude value.</param>
    /// <returns>A new <see cref="GeoCoordinate"/> instance.</returns>
    public static GeoCoordinate Create(double latitude, double longitude)
    {
        return new GeoCoordinate(
            Latitude.Create(latitude),
            Longitude.Create(longitude));
    }

    /// <summary>
    /// Creates a new <see cref="GeoCoordinate"/> instance.
    /// </summary>
    /// <param name="latitude">The latitude.</param>
    /// <param name="longitude">The longitude.</param>
    /// <returns>A new <see cref="GeoCoordinate"/> instance.</returns>
    public static GeoCoordinate Create(Latitude latitude, Longitude longitude)
    {
        return new GeoCoordinate(latitude, longitude);
    }

    /// <summary>
    /// Attempts to create a new <see cref="GeoCoordinate"/> instance.
    /// </summary>
    /// <param name="latitude">The latitude value.</param>
    /// <param name="longitude">The longitude value.</param>
    /// <param name="coordinate">When this method returns, contains the created coordinate if successful; otherwise, null.</param>
    /// <returns>true if the coordinate was created successfully; otherwise, false.</returns>
    public static bool TryCreate(double latitude, double longitude, out GeoCoordinate? coordinate)
    {
        if (Latitude.TryCreate(latitude, out Latitude? lat) && Longitude.TryCreate(longitude, out Longitude? lng))
        {
            coordinate = new GeoCoordinate(lat!, lng!);
            return true;
        }
        coordinate = null;
        return false;
    }

    /// <summary>
    /// Calculates the distance to another coordinate using the Haversine formula.
    /// </summary>
    /// <param name="other">The other coordinate.</param>
    /// <param name="unit">The unit of measurement for the result.</param>
    /// <returns>The distance between the two coordinates.</returns>
    public double DistanceTo(GeoCoordinate other, DistanceUnit unit = DistanceUnit.Kilometers)
    {
        const double earthRadiusKm = 6371.0;

        double lat1 = ToRadians(Latitude.Value);
        double lat2 = ToRadians(other.Latitude.Value);
        double deltaLat = ToRadians(other.Latitude.Value - Latitude.Value);
        double deltaLon = ToRadians(other.Longitude.Value - Longitude.Value);

        double a = (Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2)) +
                (Math.Cos(lat1) * Math.Cos(lat2) *
                Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2));

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distanceKm = earthRadiusKm * c;

        return unit switch
        {
            DistanceUnit.Kilometers => distanceKm,
            DistanceUnit.Miles => distanceKm * 0.621371,
            DistanceUnit.NauticalMiles => distanceKm * 0.539957,
            DistanceUnit.Meters => distanceKm * 1000,
            _ => distanceKm
        };
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /// <summary>
    /// Returns a string representation of the coordinate.
    /// </summary>
    public override string ToString()
    {
        return $"{Latitude.Value}, {Longitude.Value}";
    }

    /// <summary>
    /// Returns the coordinate in DMS (degrees, minutes, seconds) format.
    /// </summary>
    public string ToDmsString()
    {
        (int Degrees, int Minutes, double Seconds, char Direction) lat = Latitude.ToDms();
        (int Degrees, int Minutes, double Seconds, char Direction) lng = Longitude.ToDms();

        return $"{lat.Degrees}°{lat.Minutes}'{lat.Seconds:F2}\"{lat.Direction} {lng.Degrees}°{lng.Minutes}'{lng.Seconds:F2}\"{lng.Direction}";
    }
}

/// <summary>
/// Represents the unit of measurement for distance calculations.
/// </summary>
public enum DistanceUnit
{
    /// <summary>Kilometers.</summary>
    Kilometers,
    /// <summary>Miles.</summary>
    Miles,
    /// <summary>Nautical miles.</summary>
    NauticalMiles,
    /// <summary>Meters.</summary>
    Meters
}