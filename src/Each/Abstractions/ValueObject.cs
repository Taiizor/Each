namespace Each.Abstractions;

/// <summary>
/// Base class for value objects that provides equality comparison based on the object's value.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public abstract class ValueObject<T> : IEquatable<ValueObject<T>>, IComparable<ValueObject<T>> where T : IComparable<T>
{
    /// <summary>
    /// Gets the underlying value of this value object.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObject{T}"/> class.
    /// </summary>
    /// <param name="value">The underlying value.</param>
    protected ValueObject(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="other">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public bool Equals(ValueObject<T>? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((ValueObject<T>)obj);
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>A value that indicates the relative order of the objects being compared.</returns>
    public int CompareTo(ValueObject<T>? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Determines whether two specified value objects have the same value.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if the value of left is the same as the value of right; otherwise, false.</returns>
    public static bool operator ==(ValueObject<T>? left, ValueObject<T>? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two specified value objects have different values.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if the value of left is different from the value of right; otherwise, false.</returns>
    public static bool operator !=(ValueObject<T>? left, ValueObject<T>? right)
    {
        return !(left == right);
    }

    /// <summary>
    /// Determines whether the first value object is less than the second.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if left is less than right; otherwise, false.</returns>
    public static bool operator <(ValueObject<T>? left, ValueObject<T>? right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Determines whether the first value object is less than or equal to the second.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if left is less than or equal to right; otherwise, false.</returns>
    public static bool operator <=(ValueObject<T>? left, ValueObject<T>? right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Determines whether the first value object is greater than the second.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if left is greater than right; otherwise, false.</returns>
    public static bool operator >(ValueObject<T>? left, ValueObject<T>? right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Determines whether the first value object is greater than or equal to the second.
    /// </summary>
    /// <param name="left">The first value object to compare.</param>
    /// <param name="right">The second value object to compare.</param>
    /// <returns>true if left is greater than or equal to right; otherwise, false.</returns>
    public static bool operator >=(ValueObject<T>? left, ValueObject<T>? right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}