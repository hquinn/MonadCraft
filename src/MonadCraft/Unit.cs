using System.Diagnostics.CodeAnalysis;

namespace MonadCraft;

/// <summary>
/// Represents a type with a single value. This is often used in functional
/// programming to represent the result of a function that performs an action
/// but does not return a meaningful value (similar to 'void').
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// The single, default instance of the Unit type.
    /// </summary>
    public static readonly Unit Default = new();

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// Since Unit has only one value, this always returns true if the other object is a Unit.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is Unit;

    /// <summary>
    /// Determines whether the specified Unit is equal to the current Unit.
    /// Since Unit has only one value, this always returns true.
    /// </summary>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Returns the hash code for this instance. It is always 0.
    /// </summary>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Returns a string representation of the Unit type.
    /// </summary>
    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;

    public static bool operator !=(Unit left, Unit right) => false;
}