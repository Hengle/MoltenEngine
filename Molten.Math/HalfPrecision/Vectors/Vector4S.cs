using Molten.DoublePrecision;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Molten.HalfPrecision;

///<summary>A <see cref="short"/> vector comprised of 4 components.</summary>
[StructLayout(LayoutKind.Explicit)]
[DataContract]
public partial struct Vector4S : IFormattable, ISignedVector<Vector4S, short>, IEquatable<Vector4S>
{
    ///<summary>The number of components in the current vector type.</summary>
    public static readonly int ComponentCount = 4;

	///<summary>A Vector4S with every component set to (short)1.</summary>
	public static readonly Vector4S One = new Vector4S((short)1, (short)1, (short)1, (short)1);

    static readonly string toStringFormat = "X:{0} Y:{1} Z:{2} W:{3}";

	/// <summary>The X unit <see cref="Vector4S"/>.</summary>
	public static readonly Vector4S UnitX = new Vector4S((short)1, (short)0, (short)0, (short)0);

	/// <summary>The Y unit <see cref="Vector4S"/>.</summary>
	public static readonly Vector4S UnitY = new Vector4S((short)0, (short)1, (short)0, (short)0);

	/// <summary>The Z unit <see cref="Vector4S"/>.</summary>
	public static readonly Vector4S UnitZ = new Vector4S((short)0, (short)0, (short)1, (short)0);

	/// <summary>The W unit <see cref="Vector4S"/>.</summary>
	public static readonly Vector4S UnitW = new Vector4S((short)0, (short)0, (short)0, (short)1);

	/// <summary>Represents a zero'd Vector4S.</summary>
	public static readonly Vector4S Zero = new Vector4S((short)0, (short)0, (short)0, (short)0);

	/// <summary>The X component.</summary>
	[DataMember]
	[FieldOffset(0)]
	public short X;

	/// <summary>The Y component.</summary>
	[DataMember]
	[FieldOffset(2)]
	public short Y;

	/// <summary>The Z component.</summary>
	[DataMember]
	[FieldOffset(4)]
	public short Z;

	/// <summary>The W component.</summary>
	[DataMember]
	[FieldOffset(6)]
	public short W;

	/// <summary>A fixed array mapped to the same memory space as the individual <see cref="Vector4S"/> components.</summary>
	[IgnoreDataMember]
	[FieldOffset(0)]
	public unsafe fixed short Values[4];

    /// <summary>
    /// Gets a value indicting whether this vector is zero
    /// </summary>
    public bool IsZero => X == (short)0 && Y == (short)0 && Z == (short)0 && W == (short)0;

#region Constructors
	/// <summary>
	/// Initializes a new instance of <see cref="Vector4S"/>.
	/// </summary>
	/// <param name="x">The X component.</param>
	/// <param name="y">The Y component.</param>
	/// <param name="z">The Z component.</param>
	/// <param name="w">The W component.</param>
	public Vector4S(short x, short y, short z, short w)
	{
		X = x;
		Y = y;
		Z = z;
		W = w;
	}
	/// <summary>Initializes a new instance of <see cref="Vector4S"/>.</summary>
	/// <param name="value">The value that will be assigned to all components.</param>
	public Vector4S(short value)
	{
		X = value;
		Y = value;
		Z = value;
		W = value;
	}
	/// <summary>Initializes a new instance of <see cref="Vector4S"/> from an array.</summary>
	/// <param name="values">The values to assign to the X, Y, Z, W components of the color. This must be an array with at least 4 elements.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than 4 elements.</exception>
	public unsafe Vector4S(short[] values)
	{
		if (values == null)
			throw new ArgumentNullException("values");
		if (values.Length < 4)
			throw new ArgumentOutOfRangeException("values", "There must be at least 4 input values for Vector4S.");

		fixed (short* src = values)
		{
			fixed (short* dst = Values)
				Unsafe.CopyBlock(src, dst, (sizeof(short) * 4));
		}
	}
	/// <summary>Initializes a new instance of <see cref="Vector4S"/> from a span.</summary>
	/// <param name="values">The values to assign to the X, Y, Z, W components of the color. This must be an array with at least 4 elements.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than 4 elements.</exception>
	public Vector4S(Span<short> values)
	{
		if (values == null)
			throw new ArgumentNullException("values");
		if (values.Length < 4)
			throw new ArgumentOutOfRangeException("values", "There must be at least 4 input values for Vector4S.");

		X = values[0];
		Y = values[1];
		Z = values[2];
		W = values[3];
	}
	/// <summary>Initializes a new instance of <see cref="Vector4S"/> from a an unsafe pointer.</summary>
	/// <param name="ptrValues">The values to assign to the X, Y, Z, W components of the color.
	/// <para>There must be at least 4 elements available or undefined behaviour will occur.</para></param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="ptrValues"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="ptrValues"/> contains more or less than 4 elements.</exception>
	public unsafe Vector4S(short* ptrValues)
	{
		if (ptrValues == null)
			throw new ArgumentNullException("ptrValues");

		fixed (short* dst = Values)
			Unsafe.CopyBlock(ptrValues, dst, (sizeof(short) * 4));
	}

	///<summary>Creates a new instance of <see cref="Vector4S"/>, using a <see cref="Vector2S"/> to populate the first 2 components.</summary>
	public Vector4S(Vector2S vector, short z, short w)
	{
		X = vector.X;
		Y = vector.Y;
		Z = z;
		W = w;
	}

	///<summary>Creates a new instance of <see cref="Vector4S"/>, using a <see cref="Vector3S"/> to populate the first 3 components.</summary>
	public Vector4S(Vector3S vector, short w)
	{
		X = vector.X;
		Y = vector.Y;
		Z = vector.Z;
		W = w;
	}

#endregion

#region Instance Methods
    /// <summary>
    /// Determines whether the specified <see cref = "Vector4S"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Vector4S"/> to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="Vector4S"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ref Vector4S other)
    {
        return other.X == X && other.Y == Y && other.Z == Z && other.W == W;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Vector4S"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Vector4S"/> to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="Vector4S"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Vector4S other)
    {
        return Equals(ref other);
    }

    /// <summary>
    /// Determines whether the specified <see cref="Vector4S"/> is equal to this instance.
    /// </summary>
    /// <param name="value">The <see cref="Vector4S"/> to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="Vector4S"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object value)
    {
        if (value is Vector4S v)
            return Equals(ref v);

        return false;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = X.GetHashCode();
            hashCode = (hashCode * 397) ^ Y.GetHashCode();
            hashCode = (hashCode * 397) ^ Z.GetHashCode();
            hashCode = (hashCode * 397) ^ W.GetHashCode();
            return hashCode;
        }
    }

#region Tuples
    /// <summary>
    /// Deconstructs the current vector into 4 separate component variables. This method is also used for tuple deconstruction.
    /// </summary>
    /// <param name="x">The output for the X component.</param>
    /// <param name="y">The output for the Y component.</param>
    /// <param name="z">The output for the Z component.</param>
    /// <param name="w">The output for the W component.</param>
    public void Deconstruct(out short x, out short y, out short z, out short w)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }

    /// <summary>
    /// Constructs a <see cref="Vector4S"/> from 4 component values.
    /// </summary>
    /// <param name="tuple">The 4-component tuple containing the values.</param>
    public static implicit operator Vector4S((short x, short y, short z, short w) tuple)
    {
        return new Vector4S(tuple.x, tuple.y, tuple.z, tuple.w);
    }
#endregion

    /// <summary>
    /// Calculates the squared length of the vector.
    /// </summary>
    /// <returns>The squared length of the vector.</returns>
    /// <remarks>
    /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public short LengthSquared()
    {
        return (short)((X * X) + (Y * Y) + (Z * Z) + (W * W));
    }

	/// <summary>
    /// Creates an array containing the elements of the current <see cref="Vector4S"/>.
    /// </summary>
    /// <returns>A 4-element array containing the components of the vector.</returns>
    public short[] ToArray()
    {
        return [X, Y, Z, W];
    }

	/// <summary>
    /// Reverses the direction of the current <see cref="Vector4S"/>.
    /// </summary>
    /// <returns>A <see cref="Vector4S"/> facing the opposite direction.</returns>
	public Vector4S Negate()
	{
		return new Vector4S((short)-X, (short)-Y, (short)-Z, (short)-W);
	}
		

	/// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    public void Clamp(short min, short max)
    {
		X = X < min ? min : X > max ? max : X;
		Y = Y < min ? min : Y > max ? max : Y;
		Z = Z < min ? min : Z > max ? max : Z;
		W = W < min ? min : W > max ? max : W;
    }

    /// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    public void Clamp(Vector4S min, Vector4S max)
    {
        Clamp(min, max);
    }

	/// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    public void Clamp(ref Vector4S min, ref Vector4S max)
    {
		X = X < min.X ? min.X : X > max.X ? max.X : X;
		Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
		Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
		W = W < min.W ? min.W : W > max.W ? max.W : W;
    }
#endregion

#region To-String
	/// <summary>
    /// Returns a <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </returns>
    public string ToString(string format)
    {
        if (format == null)
            return ToString();

        return string.Format(CultureInfo.CurrentCulture, format, X, Y, Z, W);
    }

	/// <summary>
    /// Returns a <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
        return string.Format(formatProvider, toStringFormat, X, Y, Z, W);
    }

	/// <summary>
    /// Returns a <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </returns>
    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, toStringFormat, X, Y, Z, W);
    }

	/// <summary>
    /// Returns a <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="format">The format string.</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this <see cref="Vector4S"/>.
    /// </returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format == null)
            return ToString(formatProvider);

        return string.Format(formatProvider,
            toStringFormat,
				X.ToString(format, formatProvider),
				Y.ToString(format, formatProvider),
				Z.ToString(format, formatProvider),
				W.ToString(format, formatProvider)
        );
    }
#endregion

#region Add operators
	///<summary>Performs a add operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to add.</param>
	///<param name="b">The second <see cref="Vector4S"/> to add.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Add(ref Vector4S a, ref Vector4S b, out Vector4S result)
	{
		result.X = (short)(a.X + b.X);
		result.Y = (short)(a.Y + b.Y);
		result.Z = (short)(a.Z + b.Z);
		result.W = (short)(a.W + b.W);
	}

	///<summary>Performs a add operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to add.</param>
	///<param name="b">The second <see cref="Vector4S"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator +(Vector4S a, Vector4S b)
	{
		Add(ref a, ref b, out Vector4S result);
		return result;
	}

	///<summary>Performs a add operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to add.</param>
	///<param name="b">The <see cref="short"/> to add.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Add(ref Vector4S a, short b, out Vector4S result)
	{
		result.X = (short)(a.X + b);
		result.Y = (short)(a.Y + b);
		result.Z = (short)(a.Z + b);
		result.W = (short)(a.W + b);
	}

	///<summary>Performs a add operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to add.</param>
	///<param name="b">The <see cref="short"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator +(Vector4S a, short b)
	{
		Add(ref a, b, out Vector4S result);
		return result;
	}

	///<summary>Performs a add operation on a <see cref="short"/> and a <see cref="Vector4S"/>.</summary>
	///<param name="a">The <see cref="short"/> to add.</param>
	///<param name="b">The <see cref="Vector4S"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator +(short a, Vector4S b)
	{
		Add(ref b, a, out Vector4S result);
		return result;
	}


	/// <summary>
    /// Assert a <see cref="Vector4S"/> (return it unchanged).
    /// </summary>
    /// <param name="value">The <see cref="Vector4S"/> to assert (unchanged).</param>
    /// <returns>The asserted (unchanged) <see cref="Vector4S"/>.</returns>
    public static Vector4S operator +(Vector4S value)
    {
        return value;
    }
#endregion

#region Subtract operators
	///<summary>Performs a subtract operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to subtract.</param>
	///<param name="b">The second <see cref="Vector4S"/> to subtract.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Subtract(ref Vector4S a, ref Vector4S b, out Vector4S result)
	{
		result.X = (short)(a.X - b.X);
		result.Y = (short)(a.Y - b.Y);
		result.Z = (short)(a.Z - b.Z);
		result.W = (short)(a.W - b.W);
	}

	///<summary>Performs a subtract operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to subtract.</param>
	///<param name="b">The second <see cref="Vector4S"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator -(Vector4S a, Vector4S b)
	{
		Subtract(ref a, ref b, out Vector4S result);
		return result;
	}

	///<summary>Performs a subtract operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to subtract.</param>
	///<param name="b">The <see cref="short"/> to subtract.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Subtract(ref Vector4S a, short b, out Vector4S result)
	{
		result.X = (short)(a.X - b);
		result.Y = (short)(a.Y - b);
		result.Z = (short)(a.Z - b);
		result.W = (short)(a.W - b);
	}

	///<summary>Performs a subtract operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to subtract.</param>
	///<param name="b">The <see cref="short"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator -(Vector4S a, short b)
	{
		Subtract(ref a, b, out Vector4S result);
		return result;
	}

	///<summary>Performs a subtract operation on a <see cref="short"/> and a <see cref="Vector4S"/>.</summary>
	///<param name="a">The <see cref="short"/> to subtract.</param>
	///<param name="b">The <see cref="Vector4S"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator -(short a, Vector4S b)
	{
		Subtract(ref b, a, out Vector4S result);
		return result;
	}


    /// <summary>
    /// Negate/reverse the direction of a <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="value">The <see cref="Vector4S"/> to reverse.</param>
    /// <param name="result">The output for the reversed <see cref="Vector4S"/>.</param>
    public static void Negate(ref Vector4S value, out Vector4S result)
    {
		result.X = (short)-value.X;
		result.Y = (short)-value.Y;
		result.Z = (short)-value.Z;
		result.W = (short)-value.W;
            
    }

	/// <summary>
    /// Negate/reverse the direction of a <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="value">The <see cref="Vector4S"/> to reverse.</param>
    /// <returns>The reversed <see cref="Vector4S"/>.</returns>
    public static Vector4S operator -(Vector4S value)
    {
        Negate(ref value, out value);
        return value;
    }
#endregion

#region division operators
	///<summary>Performs a divide operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to divide.</param>
	///<param name="b">The second <see cref="Vector4S"/> to divide.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Divide(ref Vector4S a, ref Vector4S b, out Vector4S result)
	{
		result.X = (short)(a.X / b.X);
		result.Y = (short)(a.Y / b.Y);
		result.Z = (short)(a.Z / b.Z);
		result.W = (short)(a.W / b.W);
	}

	///<summary>Performs a divide operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to divide.</param>
	///<param name="b">The second <see cref="Vector4S"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator /(Vector4S a, Vector4S b)
	{
		Divide(ref a, ref b, out Vector4S result);
		return result;
	}

	///<summary>Performs a divide operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to divide.</param>
	///<param name="b">The <see cref="short"/> to divide.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Divide(ref Vector4S a, short b, out Vector4S result)
	{
		result.X = (short)(a.X / b);
		result.Y = (short)(a.Y / b);
		result.Z = (short)(a.Z / b);
		result.W = (short)(a.W / b);
	}

	///<summary>Performs a divide operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to divide.</param>
	///<param name="b">The <see cref="short"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator /(Vector4S a, short b)
	{
		Divide(ref a, b, out Vector4S result);
		return result;
	}

	///<summary>Performs a divide operation on a <see cref="short"/> and a <see cref="Vector4S"/>.</summary>
	///<param name="a">The <see cref="short"/> to divide.</param>
	///<param name="b">The <see cref="Vector4S"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator /(short a, Vector4S b)
	{
		Divide(ref b, a, out Vector4S result);
		return result;
	}

#endregion

#region Multiply operators
	///<summary>Performs a multiply operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to multiply.</param>
	///<param name="b">The second <see cref="Vector4S"/> to multiply.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Multiply(ref Vector4S a, ref Vector4S b, out Vector4S result)
	{
		result.X = (short)(a.X * b.X);
		result.Y = (short)(a.Y * b.Y);
		result.Z = (short)(a.Z * b.Z);
		result.W = (short)(a.W * b.W);
	}

	///<summary>Performs a multiply operation on two <see cref="Vector4S"/>.</summary>
	///<param name="a">The first <see cref="Vector4S"/> to multiply.</param>
	///<param name="b">The second <see cref="Vector4S"/> to multiply.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator *(Vector4S a, Vector4S b)
	{
		Multiply(ref a, ref b, out Vector4S result);
		return result;
	}

	///<summary>Performs a multiply operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to multiply.</param>
	///<param name="b">The <see cref="short"/> to multiply.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Multiply(ref Vector4S a, short b, out Vector4S result)
	{
		result.X = (short)(a.X * b);
		result.Y = (short)(a.Y * b);
		result.Z = (short)(a.Z * b);
		result.W = (short)(a.W * b);
	}

	///<summary>Performs a multiply operation on a <see cref="Vector4S"/> and a <see cref="short"/>.</summary>
	///<param name="a">The <see cref="Vector4S"/> to multiply.</param>
	///<param name="b">The <see cref="short"/> to multiply.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator *(Vector4S a, short b)
	{
		Multiply(ref a, b, out Vector4S result);
		return result;
	}

	///<summary>Performs a multiply operation on a <see cref="short"/> and a <see cref="Vector4S"/>.</summary>
	///<param name="a">The <see cref="short"/> to multiply.</param>
	///<param name="b">The <see cref="Vector4S"/> to multiply.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S operator *(short a, Vector4S b)
	{
		Multiply(ref b, a, out Vector4S result);
		return result;
	}

#endregion

#region Operators - Equality
    /// <summary>
    /// Tests for equality between two objects.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector4S left, Vector4S right)
    {
        return left.Equals(ref right);
    }

    /// <summary>
    /// Tests for inequality between two objects.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector4S left, Vector4S right)
    {
        return !left.Equals(ref right);
    }
#endregion

#region Static Methods
    /// <summary>
    /// Performs a cubic interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S SmoothStep(ref Vector4S start, ref Vector4S end, float amount)
    {
        amount = MathHelper.SmoothStep(amount);
        return Lerp(ref start, ref end, amount);
    }

    /// <summary>
    /// Performs a cubic interpolation between two vectors.
    /// </summary>
    /// <param name="start">Start vector.</param>
    /// <param name="end">End vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The cubic interpolation of the two vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S SmoothStep(Vector4S start, Vector4S end, short amount)
    {
        return SmoothStep(ref start, ref end, amount);
    }    

    /// <summary>
    /// Orthogonalizes a list of <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="destination">The list of orthogonalized <see cref="Vector4S"/>.</param>
    /// <param name="source">The list of vectors to orthogonalize.</param>
    /// <remarks>
    /// <para>Orthogonalization is the process of making all vectors orthogonal to each other. This
    /// means that any given vector in the list will be orthogonal to any other given vector in the
    /// list.</para>
    /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
    /// tend to be numerically unstable. The numeric stability decreases according to the vectors
    /// position in the list so that the first vector is the most stable and the last vector is the
    /// least stable.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
    public static void Orthogonalize(Vector4S[] destination, params Vector4S[] source)
    {
        //Uses the modified Gram-Schmidt process.
        //q1 = m1
        //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
        //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
        //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3
        //q5 = ...

        if (source == null)
            throw new ArgumentNullException("source");
        if (destination == null)
            throw new ArgumentNullException("destination");
        if (destination.Length < source.Length)
            throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

        for (int i = 0; i < source.Length; ++i)
        {
            Vector4S newvector = source[i];

            for (int r = 0; r < i; ++r)
                newvector -= (short)(Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];

            destination[i] = newvector;
        }
    }

        

    /// <summary>
    /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector4S"/>. <para />
    /// For example, a swizzle input of (1,1) on a <see cref="Vector4S"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
    /// </summary>
    /// <param name="val">The current vector.</param>
	/// <param name="xIndex">The axis index to use for the new X value.</param>
	/// <param name="yIndex">The axis index to use for the new Y value.</param>
	/// <param name="zIndex">The axis index to use for the new Z value.</param>
	/// <param name="wIndex">The axis index to use for the new W value.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector4S Swizzle(Vector4S val, int xIndex, int yIndex, int zIndex, int wIndex)
    {
        return new Vector4S()
        {
			X = (&val.X)[xIndex],
			Y = (&val.X)[yIndex],
			Z = (&val.X)[zIndex],
			W = (&val.X)[wIndex],
        };
    }

    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Vector4S Swizzle(Vector4S val, uint xIndex, uint yIndex, uint zIndex, uint wIndex)
    {
        return new Vector4S()
        {
			X = (&val.X)[xIndex],
			Y = (&val.X)[yIndex],
			Z = (&val.X)[zIndex],
			W = (&val.X)[wIndex],
        };
    }

    /// <summary>
    /// Calculates the dot product of two <see cref="Vector4S"/> vectors.
    /// </summary>
    /// <param name="left">First <see cref="Vector4S"/> source vector</param>
    /// <param name="right">Second <see cref="Vector4S"/> source vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Dot(ref Vector4S left, ref Vector4S right)
    {
		return (short)(((short)left.X * right.X) + ((short)left.Y * right.Y) + ((short)left.Z * right.Z) + ((short)left.W * right.W));
    }

	/// <summary>
    /// Calculates the dot product of two <see cref="Vector4S"/> vectors.
    /// </summary>
    /// <param name="left">First <see cref="Vector4S"/> source vector</param>
    /// <param name="right">Second <see cref="Vector4S"/> source vector.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short Dot(Vector4S left, Vector4S right)
    {
		return (short)((left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W));
    }

	/// <summary>
    /// Returns a <see cref="Vector4S"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
    /// </summary>
    /// <param name="value1">A <see cref="Vector4S"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
    /// <param name="value2">A <see cref="Vector4S"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
    /// <param name="value3">A <see cref="Vector4S"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
    /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
    /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
    public static Vector4S Barycentric(ref Vector4S value1, ref Vector4S value2, ref Vector4S value3, short amount1, short amount2)
    {
		return new Vector4S(
			(short)((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
			(short)((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y))), 
			(short)((value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z))), 
			(short)((value1.W + (amount1 * (value2.W - value1.W))) + (amount2 * (value3.W - value1.W)))
		);
    }

    /// <summary>
    /// Performs a linear interpolation between two <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="start">The start vector.</param>
    /// <param name="end">The end vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">The output for the resultant <see cref="Vector4S"/>.</param>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Lerp(ref Vector4S start, ref Vector4S end, float amount, out Vector4S result)
    {
		result.X = (short)((1F - amount) * start.X + amount * end.X);
		result.Y = (short)((1F - amount) * start.Y + amount * end.Y);
		result.Z = (short)((1F - amount) * start.Z + amount * end.Z);
		result.W = (short)((1F - amount) * start.W + amount * end.W);
    }

    /// <summary>
    /// Performs a linear interpolation between two <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="start">The start vector.</param>
    /// <param name="end">The end vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S Lerp(Vector4S start, Vector4S end, float amount)
    {
		return new Vector4S()
		{
			X = (short)((1F - amount) * start.X + amount * end.X),
			Y = (short)((1F - amount) * start.Y + amount * end.Y),
			Z = (short)((1F - amount) * start.Z + amount * end.Z),
			W = (short)((1F - amount) * start.W + amount * end.W),
		};
    }

	/// <summary>
    /// Performs a linear interpolation between two <see cref="Vector4S"/>.
    /// </summary>
    /// <param name="start">The start vector.</param>
    /// <param name="end">The end vector.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S Lerp(ref Vector4S start, ref Vector4S end, float amount)
    {
		return new Vector4S()
		{
			X = (short)((1F - amount) * start.X + amount * end.X),
			Y = (short)((1F - amount) * start.Y + amount * end.Y),
			Z = (short)((1F - amount) * start.Z + amount * end.Z),
			W = (short)((1F - amount) * start.W + amount * end.W),
		};
    }

    /// <summary>
    /// Returns a <see cref="Vector4S"/> containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <param name="result">The output for the resultant <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the smallest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Min(ref Vector4S left, ref Vector4S right, out Vector4S result)
	{
			result.X = (left.X < right.X) ? left.X : right.X;
			result.Y = (left.Y < right.Y) ? left.Y : right.Y;
			result.Z = (left.Z < right.Z) ? left.Z : right.Z;
			result.W = (left.W < right.W) ? left.W : right.W;
	}

    /// <summary>
    /// Returns a <see cref="Vector4S"/> containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the smallest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S Min(ref Vector4S left, ref Vector4S right)
	{
		Min(ref left, ref right, out Vector4S result);
        return result;
	}

	/// <summary>
    /// Returns a <see cref="Vector4S"/> containing the smallest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the smallest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S Min(Vector4S left, Vector4S right)
	{
		return new Vector4S()
		{
			X = (left.X < right.X) ? left.X : right.X,
			Y = (left.Y < right.Y) ? left.Y : right.Y,
			Z = (left.Z < right.Z) ? left.Z : right.Z,
			W = (left.W < right.W) ? left.W : right.W,
		};
	}

    /// <summary>
    /// Returns a <see cref="Vector4S"/> containing the largest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <param name="result">The output for the resultant <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the largest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Max(ref Vector4S left, ref Vector4S right, out Vector4S result)
	{
			result.X = (left.X > right.X) ? left.X : right.X;
			result.Y = (left.Y > right.Y) ? left.Y : right.Y;
			result.Z = (left.Z > right.Z) ? left.Z : right.Z;
			result.W = (left.W > right.W) ? left.W : right.W;
	}

    /// <summary>
    /// Returns a <see cref="Vector4S"/> containing the largest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the largest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S Max(ref Vector4S left, ref Vector4S right)
	{
		Max(ref left, ref right, out Vector4S result);
        return result;
	}

	/// <summary>
    /// Returns a <see cref="Vector4S"/> containing the largest components of the specified vectors.
    /// </summary>
    /// <param name="left">The first source <see cref="Vector4S"/>.</param>
    /// <param name="right">The second source <see cref="Vector4S"/>.</param>
    /// <returns>A <see cref="Vector4S"/> containing the largest components of the source vectors.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector4S Max(Vector4S left, Vector4S right)
	{
		return new Vector4S()
		{
			X = (left.X > right.X) ? left.X : right.X,
			Y = (left.Y > right.Y) ? left.Y : right.Y,
			Z = (left.Z > right.Z) ? left.Z : right.Z,
			W = (left.W > right.W) ? left.W : right.W,
		};
	}

	/// <summary>
    /// Calculates the squared distance between two <see cref="Vector4S"/> vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The squared distance between the two vectors.</returns>
    /// <remarks>Distance squared is the value before taking the square root. 
    /// Distance squared can often be used in place of distance if relative comparisons are being made. 
    /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
    /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
    /// involves two square roots, which are computationally expensive. However, using distance squared 
    /// provides the same information and avoids calculating two square roots.
    /// </remarks>
	public static short DistanceSquared(ref Vector4S value1, ref Vector4S value2)
    {
        int x = value1.X - value2.X;
        int y = value1.Y - value2.Y;
        int z = value1.Z - value2.Z;
        int w = value1.W - value2.W;

        return (short)((x * x) + (y * y) + (z * z) + (w * w));
    }

    /// <summary>
    /// Calculates the squared distance between two <see cref="Vector4S"/> vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The squared distance between the two vectors.</returns>
    /// <remarks>Distance squared is the value before taking the square root. 
    /// Distance squared can often be used in place of distance if relative comparisons are being made. 
    /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
    /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
    /// involves two square roots, which are computationally expensive. However, using distance squared 
    /// provides the same information and avoids calculating two square roots.
    /// </remarks>
	public static short DistanceSquared(Vector4S value1, Vector4S value2)
    {
        int x = value1.X - value2.X;
        int y = value1.Y - value2.Y;
        int z = value1.Z - value2.Z;
        int w = value1.W - value2.W;

        return (short)((x * x) + (y * y) + (z * z) + (w * w));
    }

	/// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="value">The <see cref="Vector4S"/> value to be clamped.</param>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S Clamp(Vector4S value, short min, short max)
    {
		return new Vector4S()
		{
			X = value.X < min ? min : value.X > max ? max : value.X,
			Y = value.Y < min ? min : value.Y > max ? max : value.Y,
			Z = value.Z < min ? min : value.Z > max ? max : value.Z,
			W = value.W < min ? min : value.W > max ? max : value.W,
		};
    }

    /// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="value">The <see cref="Vector4S"/> value to be clamped.</param>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    /// <param name="result">The output for the resultant <see cref="Vector4S"/>.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clamp(ref Vector4S value, ref Vector4S min, ref Vector4S max, out Vector4S result)
    {
			result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
			result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
			result.Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z;
			result.W = value.W < min.W ? min.W : value.W > max.W ? max.W : value.W;
    }

	/// <summary>Clamps the component values to within the given range.</summary>
    /// <param name="value">The <see cref="Vector4S"/> value to be clamped.</param>
    /// <param name="min">The minimum value of each component.</param>
    /// <param name="max">The maximum value of each component.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4S Clamp(Vector4S value, Vector4S min, Vector4S max)
    {
		return new Vector4S()
		{
			X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
			Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
			Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z,
			W = value.W < min.W ? min.W : value.W > max.W ? max.W : value.W,
		};
    }

    /// <summary>
    /// Returns the reflection of a vector off a surface that has the specified normal. 
    /// </summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">Normal of the surface.</param>
    /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
    /// whether the original vector was close enough to the surface to hit it.</remarks>
    public static Vector4S Reflect(Vector4S vector, Vector4S normal)
    {
        return Reflect(ref vector, ref normal);
    }

    /// <summary>
    /// Returns the reflection of a vector off a surface that has the specified normal. 
    /// </summary>
    /// <param name="vector">The source vector.</param>
    /// <param name="normal">Normal of the surface.</param>
    /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
    /// whether the original vector was close enough to the surface to hit it.</remarks>
    public static Vector4S Reflect(ref Vector4S vector, ref Vector4S normal)
    {
        int dot = (vector.X * normal.X) + (vector.Y * normal.Y) + (vector.Z * normal.Z) + (vector.W * normal.W);

        return new Vector4S()
        {
			X = (short)(vector.X - ((2 * dot) * normal.X)),
			Y = (short)(vector.Y - ((2 * dot) * normal.Y)),
			Z = (short)(vector.Z - ((2 * dot) * normal.Z)),
			W = (short)(vector.W - ((2 * dot) * normal.W)),
        };
    }
#endregion

#region Indexers
	/// <summary> Gets or sets the component at the specified index. </summary>
	/// <value>The value of the <see cref="Vector4S"/> component, depending on the index.</value>
	/// <param name="index">The index of the index component to access, ranging from 0 to 3, inclusive.</param>
	/// <returns>The value of the component at the specified index value provided.</returns>
	/// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
	public unsafe short this[int index]
	{
		get
		{
			if(index < 0 || index > 3)
				throw new IndexOutOfRangeException("index for Vector4S must be between 0 and 3, inclusive.");

			return Values[index];
		}
		set
		{
			if(index < 0 || index > 3)
				throw new IndexOutOfRangeException("index for Vector4S must be between 0 and 3, inclusive.");

			Values[index] = value;
		}
	}

	/// <summary> Gets or sets the component at the specified index. </summary>
	/// <value>The value of the <see cref="Vector4S"/> component, depending on the index.</value>
	/// <param name="index">The index of the index component to access, ranging from 0 to 3, inclusive.</param>
	/// <returns>The value of the component at the specified index value provided.</returns>
	/// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
	public unsafe short this[uint index]
	{
		get
		{
			if(index > 3)
				throw new IndexOutOfRangeException("index for Vector4S must be between 0 and 3, inclusive.");

			return Values[index];
		}
		set
		{
			if(index > 3)
				throw new IndexOutOfRangeException("index for Vector4S must be between 0 and 3, inclusive.");

			Values[index] = value;
		}
	}

#endregion

#region Casts - vectors
	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="SByte2"/>.</summary>
	public static explicit operator SByte2(Vector4S value)
	{
		return new SByte2((sbyte)value.X, (sbyte)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="SByte3"/>.</summary>
	public static explicit operator SByte3(Vector4S value)
	{
		return new SByte3((sbyte)value.X, (sbyte)value.Y, (sbyte)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="SByte4"/>.</summary>
	public static explicit operator SByte4(Vector4S value)
	{
		return new SByte4((sbyte)value.X, (sbyte)value.Y, (sbyte)value.Z, (sbyte)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Byte2"/>.</summary>
	public static explicit operator Byte2(Vector4S value)
	{
		return new Byte2((byte)value.X, (byte)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Byte3"/>.</summary>
	public static explicit operator Byte3(Vector4S value)
	{
		return new Byte3((byte)value.X, (byte)value.Y, (byte)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Byte4"/>.</summary>
	public static explicit operator Byte4(Vector4S value)
	{
		return new Byte4((byte)value.X, (byte)value.Y, (byte)value.Z, (byte)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2I"/>.</summary>
	public static explicit operator Vector2I(Vector4S value)
	{
		return new Vector2I((int)value.X, (int)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3I"/>.</summary>
	public static explicit operator Vector3I(Vector4S value)
	{
		return new Vector3I((int)value.X, (int)value.Y, (int)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4I"/>.</summary>
	public static explicit operator Vector4I(Vector4S value)
	{
		return new Vector4I((int)value.X, (int)value.Y, (int)value.Z, (int)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2UI"/>.</summary>
	public static explicit operator Vector2UI(Vector4S value)
	{
		return new Vector2UI((uint)value.X, (uint)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3UI"/>.</summary>
	public static explicit operator Vector3UI(Vector4S value)
	{
		return new Vector3UI((uint)value.X, (uint)value.Y, (uint)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4UI"/>.</summary>
	public static explicit operator Vector4UI(Vector4S value)
	{
		return new Vector4UI((uint)value.X, (uint)value.Y, (uint)value.Z, (uint)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2S"/>.</summary>
	public static explicit operator Vector2S(Vector4S value)
	{
		return new Vector2S(value.X, value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3S"/>.</summary>
	public static explicit operator Vector3S(Vector4S value)
	{
		return new Vector3S(value.X, value.Y, value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2US"/>.</summary>
	public static explicit operator Vector2US(Vector4S value)
	{
		return new Vector2US((ushort)value.X, (ushort)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3US"/>.</summary>
	public static explicit operator Vector3US(Vector4S value)
	{
		return new Vector3US((ushort)value.X, (ushort)value.Y, (ushort)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4US"/>.</summary>
	public static explicit operator Vector4US(Vector4S value)
	{
		return new Vector4US((ushort)value.X, (ushort)value.Y, (ushort)value.Z, (ushort)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2L"/>.</summary>
	public static explicit operator Vector2L(Vector4S value)
	{
		return new Vector2L((long)value.X, (long)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3L"/>.</summary>
	public static explicit operator Vector3L(Vector4S value)
	{
		return new Vector3L((long)value.X, (long)value.Y, (long)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4L"/>.</summary>
	public static explicit operator Vector4L(Vector4S value)
	{
		return new Vector4L((long)value.X, (long)value.Y, (long)value.Z, (long)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2UL"/>.</summary>
	public static explicit operator Vector2UL(Vector4S value)
	{
		return new Vector2UL((ulong)value.X, (ulong)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3UL"/>.</summary>
	public static explicit operator Vector3UL(Vector4S value)
	{
		return new Vector3UL((ulong)value.X, (ulong)value.Y, (ulong)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4UL"/>.</summary>
	public static explicit operator Vector4UL(Vector4S value)
	{
		return new Vector4UL((ulong)value.X, (ulong)value.Y, (ulong)value.Z, (ulong)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2F"/>.</summary>
	public static explicit operator Vector2F(Vector4S value)
	{
		return new Vector2F((float)value.X, (float)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3F"/>.</summary>
	public static explicit operator Vector3F(Vector4S value)
	{
		return new Vector3F((float)value.X, (float)value.Y, (float)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4F"/>.</summary>
	public static explicit operator Vector4F(Vector4S value)
	{
		return new Vector4F((float)value.X, (float)value.Y, (float)value.Z, (float)value.W);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector2D"/>.</summary>
	public static explicit operator Vector2D(Vector4S value)
	{
		return new Vector2D((double)value.X, (double)value.Y);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector3D"/>.</summary>
	public static explicit operator Vector3D(Vector4S value)
	{
		return new Vector3D((double)value.X, (double)value.Y, (double)value.Z);
	}

	///<summary>Casts a <see cref="Vector4S"/> to a <see cref="Vector4D"/>.</summary>
	public static explicit operator Vector4D(Vector4S value)
	{
		return new Vector4D((double)value.X, (double)value.Y, (double)value.Z, (double)value.W);
	}

#endregion
}


