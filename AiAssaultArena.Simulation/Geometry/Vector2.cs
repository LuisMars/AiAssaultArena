// MIT License - Copyright (C) The Mono.Xna Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace AiAssaultArena.Simulation.Math;

/// <summary>
/// Describes a 2D-vector.
/// </summary>
[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct Vector2 : IEquatable<Vector2>
{
    private static readonly Vector2 zeroVector = new Vector2(0f, 0f);
    private static readonly Vector2 unitVector = new Vector2(1f, 1f);
    private static readonly Vector2 unitXVector = new Vector2(1f, 0f);
    private static readonly Vector2 unitYVector = new Vector2(0f, 1f);

    /// <summary>
    /// The x coordinate of this <see cref="Vector2"/>.
    /// </summary>
    [DataMember]
    public float X;

    /// <summary>
    /// The y coordinate of this <see cref="Vector2"/>.
    /// </summary>
    [DataMember]
    public float Y;

    /// <summary>
    /// Returns a <see cref="Vector2"/> with components 0, 0.
    /// </summary>
    public static Vector2 Zero
    {
        get { return zeroVector; }
    }

    /// <summary>
    /// Returns a <see cref="Vector2"/> with components 1, 1.
    /// </summary>
    public static Vector2 One
    {
        get { return unitVector; }
    }

    /// <summary>
    /// Returns a <see cref="Vector2"/> with components 1, 0.
    /// </summary>
    public static Vector2 UnitX
    {
        get { return unitXVector; }
    }

    /// <summary>
    /// Returns a <see cref="Vector2"/> with components 0, 1.
    /// </summary>
    public static Vector2 UnitY
    {
        get { return unitYVector; }
    }

    internal readonly string DebugDisplayString
    {
        get
        {
            return string.Concat(
                X.ToString(), "  ",
                Y.ToString()
            );
        }
    }

    /// <summary>
    /// Constructs a 2d vector with X and Y from two values.
    /// </summary>
    /// <param name="x">The x coordinate in 2d-space.</param>
    /// <param name="y">The y coordinate in 2d-space.</param>
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Constructs a 2d vector with X and Y set to the same value.
    /// </summary>
    /// <param name="value">The x and y coordinates in 2d-space.</param>
    public Vector2(float value)
    {
        X = value;
        Y = value;
    }

    /// <summary>
    /// Converts a <see cref="System.Numerics.Vector2"/> to a <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">The converted value.</param>
    public static implicit operator Vector2(System.Numerics.Vector2 value)
    {
        return new Vector2(value.X, value.Y);
    }

    /// <summary>
    /// Inverts values in the specified <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/> on the right of the sub sign.</param>
    /// <returns>Result of the inversion.</returns>
    public static Vector2 operator -(Vector2 value)
    {
        value.X = -value.X;
        value.Y = -value.Y;
        return value;
    }

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the add sign.</param>
    /// <param name="value2">Source <see cref="Vector2"/> on the right of the add sign.</param>
    /// <returns>Sum of the vectors.</returns>
    public static Vector2 operator +(Vector2 value1, Vector2 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }

    /// <summary>
    /// Subtracts a <see cref="Vector2"/> from a <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the sub sign.</param>
    /// <param name="value2">Source <see cref="Vector2"/> on the right of the sub sign.</param>
    /// <returns>Result of the vector subtraction.</returns>
    public static Vector2 operator -(Vector2 value1, Vector2 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }

    /// <summary>
    /// Multiplies the components of two vectors by each other.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the mul sign.</param>
    /// <param name="value2">Source <see cref="Vector2"/> on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication.</returns>
    public static Vector2 operator *(Vector2 value1, Vector2 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        return value1;
    }

    /// <summary>
    /// Multiplies the components of vector by a scalar.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/> on the left of the mul sign.</param>
    /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication with a scalar.</returns>
    public static Vector2 operator *(Vector2 value, float scaleFactor)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        return value;
    }

    /// <summary>
    /// Multiplies the components of vector by a scalar.
    /// </summary>
    /// <param name="scaleFactor">Scalar value on the left of the mul sign.</param>
    /// <param name="value">Source <see cref="Vector2"/> on the right of the mul sign.</param>
    /// <returns>Result of the vector multiplication with a scalar.</returns>
    public static Vector2 operator *(float scaleFactor, Vector2 value)
    {
        value.X *= scaleFactor;
        value.Y *= scaleFactor;
        return value;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the div sign.</param>
    /// <param name="value2">Divisor <see cref="Vector2"/> on the right of the div sign.</param>
    /// <returns>The result of dividing the vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator /(Vector2 value1, Vector2 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        return value1;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by a scalar.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/> on the left of the div sign.</param>
    /// <param name="divider">Divisor scalar on the right of the div sign.</param>
    /// <returns>The result of dividing a vector by a scalar.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 operator /(Vector2 value1, float divider)
    {
        float factor = 1 / divider;
        value1.X *= factor;
        value1.Y *= factor;
        return value1;
    }

    /// <summary>
    /// Compares whether two <see cref="Vector2"/> instances are equal.
    /// </summary>
    /// <param name="value1"><see cref="Vector2"/> instance on the left of the equal sign.</param>
    /// <param name="value2"><see cref="Vector2"/> instance on the right of the equal sign.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public static bool operator ==(Vector2 value1, Vector2 value2)
    {
        return value1.X == value2.X && value1.Y == value2.Y;
    }

    /// <summary>
    /// Compares whether two <see cref="Vector2"/> instances are not equal.
    /// </summary>
    /// <param name="value1"><see cref="Vector2"/> instance on the left of the not equal sign.</param>
    /// <param name="value2"><see cref="Vector2"/> instance on the right of the not equal sign.</param>
    /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>	
    public static bool operator !=(Vector2 value1, Vector2 value2)
    {
        return value1.X != value2.X || value1.Y != value2.Y;
    }

    /// <summary>
    /// Performs vector addition on <paramref name="value1"/> and <paramref name="value2"/>.
    /// </summary>
    /// <param name="value1">The first vector to add.</param>
    /// <param name="value2">The second vector to add.</param>
    /// <returns>The result of the vector addition.</returns>
    public static Vector2 Add(Vector2 value1, Vector2 value2)
    {
        value1.X += value2.X;
        value1.Y += value2.Y;
        return value1;
    }

    /// <summary>
    /// Performs vector addition on <paramref name="value1"/> and
    /// <paramref name="value2"/>, storing the result of the
    /// addition in <paramref name="result"/>.
    /// </summary>
    /// <param name="value1">The first vector to add.</param>
    /// <param name="value2">The second vector to add.</param>
    /// <param name="result">The result of the vector addition.</param>
    public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X + value2.X;
        result.Y = value1.Y + value2.Y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains the cartesian coordinates of a vector specified in barycentric coordinates and relative to 2d-triangle.
    /// </summary>
    /// <param name="value1">The first vector of 2d-triangle.</param>
    /// <param name="value2">The second vector of 2d-triangle.</param>
    /// <param name="value3">The third vector of 2d-triangle.</param>
    /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 2d-triangle.</param>
    /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 2d-triangle.</param>
    /// <returns>The cartesian translation of barycentric coordinates.</returns>
    public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
    {
        return new Vector2(
            MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
            MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains the cartesian coordinates of a vector specified in barycentric coordinates and relative to 2d-triangle.
    /// </summary>
    /// <param name="value1">The first vector of 2d-triangle.</param>
    /// <param name="value2">The second vector of 2d-triangle.</param>
    /// <param name="value3">The third vector of 2d-triangle.</param>
    /// <param name="amount1">Barycentric scalar <c>b2</c> which represents a weighting factor towards second vector of 2d-triangle.</param>
    /// <param name="amount2">Barycentric scalar <c>b3</c> which represents a weighting factor towards third vector of 2d-triangle.</param>
    /// <param name="result">The cartesian translation of barycentric coordinates as an output parameter.</param>
    public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result)
    {
        result.X = MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2);
        result.Y = MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains CatmullRom interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">The first vector in interpolation.</param>
    /// <param name="value2">The second vector in interpolation.</param>
    /// <param name="value3">The third vector in interpolation.</param>
    /// <param name="value4">The fourth vector in interpolation.</param>
    /// <param name="amount">Weighting factor.</param>
    /// <returns>The result of CatmullRom interpolation.</returns>
    public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
    {
        return new Vector2(
            MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
            MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains CatmullRom interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">The first vector in interpolation.</param>
    /// <param name="value2">The second vector in interpolation.</param>
    /// <param name="value3">The third vector in interpolation.</param>
    /// <param name="value4">The fourth vector in interpolation.</param>
    /// <param name="amount">Weighting factor.</param>
    /// <param name="result">The result of CatmullRom interpolation as an output parameter.</param>
    public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result)
    {
        result.X = MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount);
        result.Y = MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount);
    }

    /// <summary>
    /// Round the members of this <see cref="Vector2"/> towards positive infinity.
    /// </summary>
    public void Ceiling()
    {
        X = MathF.Ceiling(X);
        Y = MathF.Ceiling(Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards positive infinity.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <returns>The rounded <see cref="Vector2"/>.</returns>
    public static Vector2 Ceiling(Vector2 value)
    {
        value.X = MathF.Ceiling(value.X);
        value.Y = MathF.Ceiling(value.Y);
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards positive infinity.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The rounded <see cref="Vector2"/>.</param>
    public static void Ceiling(ref Vector2 value, out Vector2 result)
    {
        result.X = MathF.Ceiling(value.X);
        result.Y = MathF.Ceiling(value.Y);
    }

    /// <summary>
    /// Clamps the specified value within a range.
    /// </summary>
    /// <param name="value1">The value to clamp.</param>
    /// <param name="min">The min value.</param>
    /// <param name="max">The max value.</param>
    /// <returns>The clamped value.</returns>
    public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
    {
        return new Vector2(
            MathHelper.Clamp(value1.X, min.X, max.X),
            MathHelper.Clamp(value1.Y, min.Y, max.Y));
    }

    /// <summary>
    /// Clamps the specified value within a range.
    /// </summary>
    /// <param name="value1">The value to clamp.</param>
    /// <param name="min">The min value.</param>
    /// <param name="max">The max value.</param>
    /// <param name="result">The clamped value as an output parameter.</param>
    public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
    {
        result.X = MathHelper.Clamp(value1.X, min.X, max.X);
        result.Y = MathHelper.Clamp(value1.Y, min.Y, max.Y);
    }

    /// <summary>
    /// Returns the distance between two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The distance between two vectors.</returns>
    public static float Distance(Vector2 value1, Vector2 value2)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        return MathF.Sqrt(v1 * v1 + v2 * v2);
    }

    /// <summary>
    /// Returns the distance between two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The distance between two vectors as an output parameter.</param>
    public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        result = MathF.Sqrt(v1 * v1 + v2 * v2);
    }

    /// <summary>
    /// Returns the squared distance between two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The squared distance between two vectors.</returns>
    public static float DistanceSquared(Vector2 value1, Vector2 value2)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        return v1 * v1 + v2 * v2;
    }

    /// <summary>
    /// Returns the squared distance between two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The squared distance between two vectors as an output parameter.</param>
    public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
        result = v1 * v1 + v2 * v2;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Divisor <see cref="Vector2"/>.</param>
    /// <returns>The result of dividing the vectors.</returns>
    public static Vector2 Divide(Vector2 value1, Vector2 value2)
    {
        value1.X /= value2.X;
        value1.Y /= value2.Y;
        return value1;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by the components of another <see cref="Vector2"/>.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Divisor <see cref="Vector2"/>.</param>
    /// <param name="result">The result of dividing the vectors as an output parameter.</param>
    public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X / value2.X;
        result.Y = value1.Y / value2.Y;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by a scalar.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="divider">Divisor scalar.</param>
    /// <returns>The result of dividing a vector by a scalar.</returns>
    public static Vector2 Divide(Vector2 value1, float divider)
    {
        float factor = 1 / divider;
        value1.X *= factor;
        value1.Y *= factor;
        return value1;
    }

    /// <summary>
    /// Divides the components of a <see cref="Vector2"/> by a scalar.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="divider">Divisor scalar.</param>
    /// <param name="result">The result of dividing a vector by a scalar as an output parameter.</param>
    public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
    {
        float factor = 1 / divider;
        result.X = value1.X * factor;
        result.Y = value1.Y * factor;
    }

    /// <summary>
    /// Returns a dot product of two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The dot product of two vectors.</returns>
    public static float Dot(Vector2 value1, Vector2 value2)
    {
        return value1.X * value2.X + value1.Y * value2.Y;
    }

    /// <summary>
    /// Returns a dot product of two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The dot product of two vectors as an output parameter.</param>
    public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
    {
        result = value1.X * value2.X + value1.Y * value2.Y;
    }

    /// <summary>
    /// Compares whether current instance is equal to specified <see cref="object"/>.
    /// </summary>
    /// <param name="obj">The <see cref="object"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public override bool Equals(object obj)
    {
        if (obj is Vector2)
        {
            return Equals((Vector2)obj);
        }

        return false;
    }

    /// <summary>
    /// Compares whether current instance is equal to specified <see cref="Vector2"/>.
    /// </summary>
    /// <param name="other">The <see cref="Vector2"/> to compare.</param>
    /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
    public readonly bool Equals(Vector2 other)
    {
        return X == other.X && Y == other.Y;
    }

    /// <summary>
    /// Round the members of this <see cref="Vector2"/> towards negative infinity.
    /// </summary>
    public void Floor()
    {
        X = MathF.Floor(X);
        Y = MathF.Floor(Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards negative infinity.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <returns>The rounded <see cref="Vector2"/>.</returns>
    public static Vector2 Floor(Vector2 value)
    {
        value.X = MathF.Floor(value.X);
        value.Y = MathF.Floor(value.Y);
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded towards negative infinity.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The rounded <see cref="Vector2"/>.</param>
    public static void Floor(ref Vector2 value, out Vector2 result)
    {
        result.X = MathF.Floor(value.X);
        result.Y = MathF.Floor(value.Y);
    }

    /// <summary>
    /// Gets the hash code of this <see cref="Vector2"/>.
    /// </summary>
    /// <returns>Hash code of this <see cref="Vector2"/>.</returns>
    public override readonly int GetHashCode()
    {
        unchecked
        {
            return X.GetHashCode() * 397 ^ Y.GetHashCode();
        }
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains hermite spline interpolation.
    /// </summary>
    /// <param name="value1">The first position vector.</param>
    /// <param name="tangent1">The first tangent vector.</param>
    /// <param name="value2">The second position vector.</param>
    /// <param name="tangent2">The second tangent vector.</param>
    /// <param name="amount">Weighting factor.</param>
    /// <returns>The hermite spline interpolation vector.</returns>
    public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
    {
        return new Vector2(MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount), MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains hermite spline interpolation.
    /// </summary>
    /// <param name="value1">The first position vector.</param>
    /// <param name="tangent1">The first tangent vector.</param>
    /// <param name="value2">The second position vector.</param>
    /// <param name="tangent2">The second tangent vector.</param>
    /// <param name="amount">Weighting factor.</param>
    /// <param name="result">The hermite spline interpolation vector as an output parameter.</param>
    public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result)
    {
        result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
        result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
    }

    /// <summary>
    /// Returns the length of this <see cref="Vector2"/>.
    /// </summary>
    /// <returns>The length of this <see cref="Vector2"/>.</returns>
    public readonly float Length()
    {
        return MathF.Sqrt(X * X + Y * Y);
    }

    /// <summary>
    /// Returns the squared length of this <see cref="Vector2"/>.
    /// </summary>
    /// <returns>The squared length of this <see cref="Vector2"/>.</returns>
    public readonly float LengthSquared()
    {
        return X * X + Y * Y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
    /// <returns>The result of linear interpolation of the specified vectors.</returns>
    public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
    {
        return new Vector2(
            MathHelper.Lerp(value1.X, value2.X, amount),
            MathHelper.Lerp(value1.Y, value2.Y, amount));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
    /// <param name="result">The result of linear interpolation of the specified vectors as an output parameter.</param>
    public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
    {
        result.X = MathHelper.Lerp(value1.X, value2.X, amount);
        result.Y = MathHelper.Lerp(value1.Y, value2.Y, amount);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
    /// Uses <see cref="MathHelper.LerpPrecise"/> on MathHelper for the interpolation.
    /// Less efficient but more precise compared to <see cref="Lerp(Vector2, Vector2, float)"/>.
    /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
    /// <returns>The result of linear interpolation of the specified vectors.</returns>
    public static Vector2 LerpPrecise(Vector2 value1, Vector2 value2, float amount)
    {
        return new Vector2(
            MathHelper.LerpPrecise(value1.X, value2.X, amount),
            MathHelper.LerpPrecise(value1.Y, value2.Y, amount));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains linear interpolation of the specified vectors.
    /// Uses <see cref="MathHelper.LerpPrecise"/> on MathHelper for the interpolation.
    /// Less efficient but more precise compared to <see cref="Lerp(ref Vector2, ref Vector2, float, out Vector2)"/>.
    /// See remarks section of <see cref="MathHelper.LerpPrecise"/> on MathHelper for more info.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
    /// <param name="result">The result of linear interpolation of the specified vectors as an output parameter.</param>
    public static void LerpPrecise(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
    {
        result.X = MathHelper.LerpPrecise(value1.X, value2.X, amount);
        result.Y = MathHelper.LerpPrecise(value1.Y, value2.Y, amount);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a maximal values from the two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The <see cref="Vector2"/> with maximal values from the two vectors.</returns>
    public static Vector2 Max(Vector2 value1, Vector2 value2)
    {
        return new Vector2(value1.X > value2.X ? value1.X : value2.X,
                           value1.Y > value2.Y ? value1.Y : value2.Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a maximal values from the two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The <see cref="Vector2"/> with maximal values from the two vectors as an output parameter.</param>
    public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X > value2.X ? value1.X : value2.X;
        result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a minimal values from the two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The <see cref="Vector2"/> with minimal values from the two vectors.</returns>
    public static Vector2 Min(Vector2 value1, Vector2 value2)
    {
        return new Vector2(value1.X < value2.X ? value1.X : value2.X,
                           value1.Y < value2.Y ? value1.Y : value2.Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a minimal values from the two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The <see cref="Vector2"/> with minimal values from the two vectors as an output parameter.</param>
    public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X < value2.X ? value1.X : value2.X;
        result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a multiplication of two vectors.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <returns>The result of the vector multiplication.</returns>
    public static Vector2 Multiply(Vector2 value1, Vector2 value2)
    {
        value1.X *= value2.X;
        value1.Y *= value2.Y;
        return value1;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a multiplication of two vectors.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The result of the vector multiplication as an output parameter.</param>
    public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X * value2.X;
        result.Y = value1.Y * value2.Y;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a multiplication of <see cref="Vector2"/> and a scalar.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="scaleFactor">Scalar value.</param>
    /// <returns>The result of the vector multiplication with a scalar.</returns>
    public static Vector2 Multiply(Vector2 value1, float scaleFactor)
    {
        value1.X *= scaleFactor;
        value1.Y *= scaleFactor;
        return value1;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a multiplication of <see cref="Vector2"/> and a scalar.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="scaleFactor">Scalar value.</param>
    /// <param name="result">The result of the multiplication with a scalar as an output parameter.</param>
    public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
    {
        result.X = value1.X * scaleFactor;
        result.Y = value1.Y * scaleFactor;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains the specified vector inversion.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <returns>The result of the vector inversion.</returns>
    public static Vector2 Negate(Vector2 value)
    {
        value.X = -value.X;
        value.Y = -value.Y;
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains the specified vector inversion.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The result of the vector inversion as an output parameter.</param>
    public static void Negate(ref Vector2 value, out Vector2 result)
    {
        result.X = -value.X;
        result.Y = -value.Y;
    }

    /// <summary>
    /// Turns this <see cref="Vector2"/> to a unit vector with the same direction.
    /// </summary>
    public void Normalize()
    {
        float val = 1.0f / MathF.Sqrt(X * X + Y * Y);
        X *= val;
        Y *= val;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a normalized values from another vector.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <returns>Unit vector.</returns>
    public static Vector2 Normalize(Vector2 value)
    {
        float val = 1.0f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
        value.X *= val;
        value.Y *= val;
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a normalized values from another vector.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="result">Unit vector as an output parameter.</param>
    public static void Normalize(ref Vector2 value, out Vector2 result)
    {
        float val = 1.0f / MathF.Sqrt(value.X * value.X + value.Y * value.Y);
        result.X = value.X * val;
        result.Y = value.Y * val;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains reflect vector of the given vector and normal.
    /// </summary>
    /// <param name="vector">Source <see cref="Vector2"/>.</param>
    /// <param name="normal">Reflection normal.</param>
    /// <returns>Reflected vector.</returns>
    public static Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        Vector2 result;
        float val = 2.0f * (vector.X * normal.X + vector.Y * normal.Y);
        result.X = vector.X - normal.X * val;
        result.Y = vector.Y - normal.Y * val;
        return result;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains reflect vector of the given vector and normal.
    /// </summary>
    /// <param name="vector">Source <see cref="Vector2"/>.</param>
    /// <param name="normal">Reflection normal.</param>
    /// <param name="result">Reflected vector as an output parameter.</param>
    public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
    {
        float val = 2.0f * (vector.X * normal.X + vector.Y * normal.Y);
        result.X = vector.X - normal.X * val;
        result.Y = vector.Y - normal.Y * val;
    }

    /// <summary>
    /// Round the members of this <see cref="Vector2"/> to the nearest integer value.
    /// </summary>
    public void Round()
    {
        X = MathF.Round(X);
        Y = MathF.Round(Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded to the nearest integer value.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <returns>The rounded <see cref="Vector2"/>.</returns>
    public static Vector2 Round(Vector2 value)
    {
        value.X = MathF.Round(value.X);
        value.Y = MathF.Round(value.Y);
        return value;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains members from another vector rounded to the nearest integer value.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The rounded <see cref="Vector2"/>.</param>
    public static void Round(ref Vector2 value, out Vector2 result)
    {
        result.X = MathF.Round(value.X);
        result.Y = MathF.Round(value.Y);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains cubic interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <param name="amount">Weighting value.</param>
    /// <returns>Cubic interpolation of the specified vectors.</returns>
    public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
    {
        return new Vector2(
            MathHelper.SmoothStep(value1.X, value2.X, amount),
            MathHelper.SmoothStep(value1.Y, value2.Y, amount));
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains cubic interpolation of the specified vectors.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <param name="amount">Weighting value.</param>
    /// <param name="result">Cubic interpolation of the specified vectors as an output parameter.</param>
    public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
    {
        result.X = MathHelper.SmoothStep(value1.X, value2.X, amount);
        result.Y = MathHelper.SmoothStep(value1.Y, value2.Y, amount);
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains subtraction of on <see cref="Vector2"/> from a another.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <returns>The result of the vector subtraction.</returns>
    public static Vector2 Subtract(Vector2 value1, Vector2 value2)
    {
        value1.X -= value2.X;
        value1.Y -= value2.Y;
        return value1;
    }

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains subtraction of on <see cref="Vector2"/> from a another.
    /// </summary>
    /// <param name="value1">Source <see cref="Vector2"/>.</param>
    /// <param name="value2">Source <see cref="Vector2"/>.</param>
    /// <param name="result">The result of the vector subtraction as an output parameter.</param>
    public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
    {
        result.X = value1.X - value2.X;
        result.Y = value1.Y - value2.Y;
    }

    /// <summary>
    /// Returns a <see cref="string"/> representation of this <see cref="Vector2"/> in the format:
    /// {X:[<see cref="X"/>] Y:[<see cref="Y"/>]}
    /// </summary>
    /// <returns>A <see cref="string"/> representation of this <see cref="Vector2"/>.</returns>
    public override readonly string ToString()
    {
        return "{X:" + X + " Y:" + Y + "}";
    }

    /// <summary>
    /// Gets a <see cref="Point"/> representation for this object.
    /// </summary>
    /// <returns>A <see cref="Point"/> representation for this object.</returns>
    public readonly Point ToPoint()
    {
        return new Point((int)X, (int)Y);
    }

    ///// <summary>
    ///// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
    ///// </summary>
    ///// <param name="position">Source <see cref="Vector2"/>.</param>
    ///// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    ///// <returns>Transformed <see cref="Vector2"/>.</returns>
    //public static Vector2 Transform(Vector2 position, Matrix matrix)
    //{
    //    return new Vector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
    //}

    ///// <summary>
    ///// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
    ///// </summary>
    ///// <param name="position">Source <see cref="Vector2"/>.</param>
    ///// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    ///// <param name="result">Transformed <see cref="Vector2"/> as an output parameter.</param>
    //public static void Transform(ref Vector2 position, ref Matrix matrix, out Vector2 result)
    //{
    //    var x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
    //    var y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
    //    result.X = x;
    //    result.Y = y;
    //}

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Quaternion"/>, representing the rotation.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
    /// <returns>Transformed <see cref="Vector2"/>.</returns>
    //public static Vector2 Transform(Vector2 value, Quaternion rotation)
    //{
    //    Transform(ref value, ref rotation, out value);
    //    return value;
    //}

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Quaternion"/>, representing the rotation.
    /// </summary>
    /// <param name="value">Source <see cref="Vector2"/>.</param>
    /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
    /// <param name="result">Transformed <see cref="Vector2"/> as an output parameter.</param>
    //public static void Transform(ref Vector2 value, ref Quaternion rotation, out Vector2 result)
    //{
    //    var rot1 = new Vector3(rotation.X + rotation.X, rotation.Y + rotation.Y, rotation.Z + rotation.Z);
    //    var rot2 = new Vector3(rotation.X, rotation.X, rotation.W);
    //    var rot3 = new Vector3(1, rotation.Y, rotation.Z);
    //    var rot4 = rot1 * rot2;
    //    var rot5 = rot1 * rot3;

    //    var v = new Vector2();
    //    v.X = (float)(value.X * (1.0 - rot5.Y - rot5.Z) + value.Y * (rot4.Y - (double)rot4.Z));
    //    v.Y = (float)(value.X * (rot4.Y + (double)rot4.Z) + value.Y * (1.0 - rot4.X - rot5.Z));
    //    result.X = v.X;
    //    result.Y = v.Y;
    //}

    /// <summary>
    /// Apply transformation on vectors within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="sourceIndex">The starting index of transformation in the source array.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <param name="destinationArray">Destination array.</param>
    /// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector2"/> should be written.</param>
    /// <param name="length">The number of vectors to be transformed.</param>
    //public static void Transform(
    //    Vector2[] sourceArray,
    //    int sourceIndex,
    //    ref Matrix matrix,
    //    Vector2[] destinationArray,
    //    int destinationIndex,
    //    int length)
    //{
    //    if (sourceArray == null)
    //        throw new ArgumentNullException("sourceArray");
    //    if (destinationArray == null)
    //        throw new ArgumentNullException("destinationArray");
    //    if (sourceArray.Length < sourceIndex + length)
    //        throw new ArgumentException("Source array length is lesser than sourceIndex + length");
    //    if (destinationArray.Length < destinationIndex + length)
    //        throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

    //    for (int x = 0; x < length; x++)
    //    {
    //        var position = sourceArray[sourceIndex + x];
    //        var destination = destinationArray[destinationIndex + x];
    //        destination.X = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
    //        destination.Y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
    //        destinationArray[destinationIndex + x] = destination;
    //    }
    //}

    /// <summary>
    /// Apply transformation on vectors within array of <see cref="Vector2"/> by the specified <see cref="Quaternion"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="sourceIndex">The starting index of transformation in the source array.</param>
    /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
    /// <param name="destinationArray">Destination array.</param>
    /// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector2"/> should be written.</param>
    /// <param name="length">The number of vectors to be transformed.</param>
    //public static void Transform
    //(
    //    Vector2[] sourceArray,
    //    int sourceIndex,
    //    ref Quaternion rotation,
    //    Vector2[] destinationArray,
    //    int destinationIndex,
    //    int length
    //)
    //{
    //    if (sourceArray == null)
    //        throw new ArgumentNullException("sourceArray");
    //    if (destinationArray == null)
    //        throw new ArgumentNullException("destinationArray");
    //    if (sourceArray.Length < sourceIndex + length)
    //        throw new ArgumentException("Source array length is lesser than sourceIndex + length");
    //    if (destinationArray.Length < destinationIndex + length)
    //        throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

    //    for (int x = 0; x < length; x++)
    //    {
    //        var position = sourceArray[sourceIndex + x];
    //        var destination = destinationArray[destinationIndex + x];

    //        Vector2 v;
    //        Transform(ref position, ref rotation, out v);

    //        destination.X = v.X;
    //        destination.Y = v.Y;

    //        destinationArray[destinationIndex + x] = destination;
    //    }
    //}

    /// <summary>
    /// Apply transformation on all vectors within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <param name="destinationArray">Destination array.</param>
    //public static void Transform(
    //    Vector2[] sourceArray,
    //    ref Matrix matrix,
    //    Vector2[] destinationArray)
    //{
    //    Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
    //}

    /// <summary>
    /// Apply transformation on all vectors within array of <see cref="Vector2"/> by the specified <see cref="Quaternion"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="rotation">The <see cref="Quaternion"/> which contains rotation transformation.</param>
    /// <param name="destinationArray">Destination array.</param>
    //public static void Transform
    //(
    //    Vector2[] sourceArray,
    //    ref Quaternion rotation,
    //    Vector2[] destinationArray
    //)
    //{
    //    Transform(sourceArray, 0, ref rotation, destinationArray, 0, sourceArray.Length);
    //}

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
    /// </summary>
    /// <param name="normal">Source <see cref="Vector2"/> which represents a normal vector.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <returns>Transformed normal.</returns>
    //public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
    //{
    //    return new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21, normal.X * matrix.M12 + normal.Y * matrix.M22);
    //}

    /// <summary>
    /// Creates a new <see cref="Vector2"/> that contains a transformation of the specified normal by the specified <see cref="Matrix"/>.
    /// </summary>
    /// <param name="normal">Source <see cref="Vector2"/> which represents a normal vector.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <param name="result">Transformed normal as an output parameter.</param>
    //public static void TransformNormal(ref Vector2 normal, ref Matrix matrix, out Vector2 result)
    //{
    //    var x = normal.X * matrix.M11 + normal.Y * matrix.M21;
    //    var y = normal.X * matrix.M12 + normal.Y * matrix.M22;
    //    result.X = x;
    //    result.Y = y;
    //}

    /// <summary>
    /// Apply transformation on normals within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="sourceIndex">The starting index of transformation in the source array.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <param name="destinationArray">Destination array.</param>
    /// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector2"/> should be written.</param>
    /// <param name="length">The number of normals to be transformed.</param>
    //public static void TransformNormal
    //(
    //    Vector2[] sourceArray,
    //    int sourceIndex,
    //    ref Matrix matrix,
    //    Vector2[] destinationArray,
    //    int destinationIndex,
    //    int length
    //)
    //{
    //    if (sourceArray == null)
    //        throw new ArgumentNullException("sourceArray");
    //    if (destinationArray == null)
    //        throw new ArgumentNullException("destinationArray");
    //    if (sourceArray.Length < sourceIndex + length)
    //        throw new ArgumentException("Source array length is lesser than sourceIndex + length");
    //    if (destinationArray.Length < destinationIndex + length)
    //        throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

    //    for (int i = 0; i < length; i++)
    //    {
    //        var normal = sourceArray[sourceIndex + i];

    //        destinationArray[destinationIndex + i] = new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21,
    //                                                             normal.X * matrix.M12 + normal.Y * matrix.M22);
    //    }
    //}

    /// <summary>
    /// Apply transformation on all normals within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
    /// </summary>
    /// <param name="sourceArray">Source array.</param>
    /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
    /// <param name="destinationArray">Destination array.</param>
    //public static void TransformNormal
    //    (
    //    Vector2[] sourceArray,
    //    ref Matrix matrix,
    //    Vector2[] destinationArray
    //    )
    //{
    //    if (sourceArray == null)
    //        throw new ArgumentNullException("sourceArray");
    //    if (destinationArray == null)
    //        throw new ArgumentNullException("destinationArray");
    //    if (destinationArray.Length < sourceArray.Length)
    //        throw new ArgumentException("Destination array length is lesser than source array length");

    //    for (int i = 0; i < sourceArray.Length; i++)
    //    {
    //        var normal = sourceArray[i];

    //        destinationArray[i] = new Vector2(normal.X * matrix.M11 + normal.Y * matrix.M21,
    //                                          normal.X * matrix.M12 + normal.Y * matrix.M22);
    //    }
    //}

    /// <summary>
    /// Deconstruction method for <see cref="Vector2"/>.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public readonly void Deconstruct(out float x, out float y)
    {
        x = X;
        y = Y;
    }

    /// <summary>
    /// Returns a <see cref="System.Numerics.Vector2"/>.
    /// </summary>
    public readonly System.Numerics.Vector2 ToNumerics()
    {
        return new System.Numerics.Vector2(X, Y);
    }

    public float Cross(Vector2 other)
    {
        return X * other.Y - Y * other.X;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 SetX(float x) => new Vector2(x, Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 SetY(float y) => new Vector2(X, y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Translate(float x, float y) => new Vector2(X + x, Y + y);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static Size2 ToSize(this Vector2 value) => new Size2(value.X, value.Y);

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static Size2 ToAbsoluteSize(this Vector2 value)
    //{
    //    var x = Math.Abs(value.X);
    //    var y = Math.Abs(value.Y);
    //    return new Size2(x, y);
    //}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Round(int digits, MidpointRounding mode)
    {
        var x = MathF.Round(X, digits, mode);
        var y = MathF.Round(Y, digits, mode);
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Round(int digits)
    {
        var x = MathF.Round(X, digits);
        var y = MathF.Round(Y, digits);
        return new Vector2(x, y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool EqualsWithTolerence(Vector2 otherValue, float tolerance = 0.00001f)
    {
        return MathF.Abs(X - otherValue.X) <= tolerance && (MathF.Abs(Y - otherValue.Y) <= tolerance);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Rotate(float radians)
    {
        var cos = MathF.Cos(radians);
        var sin = MathF.Sin(radians);
        return new Vector2(X * cos - Y * sin, X * sin + Y * cos);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 NormalizedCopy()
    {
        var newVector2 = new Vector2(X, Y);
        newVector2.Normalize();
        return newVector2;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 PerpendicularClockwise() => new Vector2(Y, -X);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 PerpendicularCounterClockwise() => new Vector2(-Y, X);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 Truncate(float maxLength)
    {
        if (LengthSquared() > maxLength * maxLength)
        {
            return NormalizedCopy() * maxLength;
        }

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNaN() => float.IsNaN(X) || float.IsNaN(Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ToAngle() => (float)MathF.Atan2(X, -Y);

    /// <summary>
    ///     Calculates the dot product of two vectors. If the two vectors are unit vectors, the dot product returns a floating
    ///     point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For
    ///     example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The dot product of the two vectors.</returns>
    /// <remarks>
    ///     <para>The dot product is also known as the inner product.</para>
    ///     <para>
    ///         For any two vectors, the dot product is defined as: <c>(vector1.X * vector2.X) + (vector1.Y * vector2.Y).</c>
    ///         The result of this calculation, plus or minus some margin to account for floating point error, is equal to:
    ///         <c>Length(vector1) * Length(vector2) * System.Math.Cos(theta)</c>, where <c>theta</c> is the angle between the
    ///         two vectors.
    ///     </para>
    ///     <para>
    ///         If <paramref name="vector1" /> and <paramref name="vector2" /> are unit vectors, the length of each
    ///         vector will be equal to 1. So, when <paramref name="vector1" /> and <paramref name="vector2" /> are unit
    ///         vectors, the dot product is simply equal to the cosine of the angle between the two vectors. For example, both
    ///         <c>cos</c> values in the following calcuations would be equal in value:
    ///         <c>vector1.Normalize(); vector2.Normalize(); var cos = vector1.Dot(vector2)</c>,
    ///         <c>var cos = System.Math.Cos(theta)</c>, where <c>theta</c> is angle in radians betwen the two vectors.
    ///     </para>
    ///     <para>
    ///         If <paramref name="vector1" /> and <paramref name="vector2" /> are unit vectors, without knowing the value of
    ///         <c>theta</c> or using a potentially processor-intensive trigonometric function, the value of the dot product
    ///         can tell us the
    ///         following things:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.Dot(vector2) &gt; 0</c>, the angle between the two vectors
    ///                     is less than 90 degrees.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.Dot(vector2) &lt; 0</c>, the angle between the two vectors
    ///                     is more than 90 degrees.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.Dot(vector2) == 0</c>, the angle between the two vectors
    ///                     is 90 degrees; that is, the vectors are othogonal.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.Dot(vector2) == 1</c>, the angle between the two vectors
    ///                     is 0 degrees; that is, the vectors point in the same direction and are parallel.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.Dot(vector2) == -1</c>, the angle between the two vectors
    ///                     is 180 degrees; that is, the vectors point in opposite directions and are parallel.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    ///     <note type="caution">
    ///         Because of floating point error, two orthogonal vectors may not return a dot product that is exactly zero. It
    ///         might be zero plus some amount of floating point error. In your code, you will want to determine what amount of
    ///         error is acceptable in your calculation, and take that into account when you do your comparisons.
    ///     </note>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float Dot(Vector2 vector2)
    {
        return X * vector2.X + Y * vector2.Y;
    }

    /// <summary>
    ///     Calculates the scalar projection of one vector onto another. The scalar projection returns the length of the
    ///     orthogonal projection of the first vector onto a straight line parallel to the second vector, with a negative value
    ///     if the projection has an opposite direction with respect to the second vector.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The scalar projection of <paramref name="vector1" /> onto <paramref name="vector2" />.</returns>
    /// <remarks>
    ///     <para>
    ///         The scalar projection is also known as the scalar resolute of the first vector in the direction of the second
    ///         vector.
    ///     </para>
    ///     <para>
    ///         For any two vectors, the scalar projection is defined as: <c>vector1.Dot(vector2) / Length(vector2)</c>. The
    ///         result of this calculation, plus or minus some margin to account for floating point error, is equal to:
    ///         <c>Length(vector1) * System.Math.Cos(theta)</c>, where <c>theta</c> is the angle in radians between
    ///         <paramref name="vector1" /> and <paramref name="vector2" />.
    ///     </para>
    ///     <para>
    ///         The value of the scalar projection can tell us the following things:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.ScalarProjectOnto(vector2) &gt;= 0</c>, the angle between <paramref name="vector1" />
    ///                     and <paramref name="vector2" /> is between 0 degrees (exclusive) and 90 degrees (inclusive).
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     If <c>vector1.ScalarProjectOnto(vector2) &lt; 0</c>, the angle between <paramref name="vector1" />
    ///                     and <paramref name="vector2" /> is between 90 degrees (exclusive) and 180 degrees (inclusive).
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ScalarProjectOnto(Vector2 vector2)
    {
        var dotNumerator = X * vector2.X + Y * vector2.Y;
        var lengthSquaredDenominator = vector2.X * vector2.X + vector2.Y * vector2.Y;
        return dotNumerator / MathF.Sqrt(lengthSquaredDenominator);
    }

    /// <summary>
    ///     Calculates the vector projection of one vector onto another. The vector projection returns the orthogonal
    ///     projection of the first vector onto a straight line parallel to the second vector.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The vector projection of <paramref name="vector1" /> onto <paramref name="vector2" />.</returns>
    /// <remarks>
    ///     <para>
    ///         The vector projection is also known as the vector component or vector resolute of the first vector in the
    ///         direction of the second vector.
    ///     </para>
    ///     <para>
    ///         For any two vectors, the vector projection is defined as:
    ///         <c>( vector1.Dot(vector2) / Length(vector2)^2 ) * vector2</c>.
    ///         The
    ///         result of this calculation, plus or minus some margin to account for floating point error, is equal to:
    ///         <c>( Length(vector1) * System.Math.Cos(theta) ) * vector2 / Length(vector2)</c>, where <c>theta</c> is the
    ///         angle in radians between <paramref name="vector1" /> and <paramref name="vector2" />.
    ///     </para>
    ///     <para>
    ///         This function is easier to compute than <see cref="ScalarProjectOnto" /> since it does not use a square root.
    ///         When the vector projection and the scalar projection is required, consider using this function; the scalar
    ///         projection can be obtained by taking the length of the projection vector.
    ///     </para>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ProjectOnto(Vector2 vector2)
    {
        var dotNumerator = X * vector2.X + Y * vector2.Y;
        var lengthSquaredDenominator = vector2.X * vector2.X + vector2.Y * vector2.Y;
        return dotNumerator / lengthSquaredDenominator * vector2;
    }
}
