using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Molten.DoublePrecision;

/// <summary>
/// Represents a color in the form of red, green, blue.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
[DataContract]
public struct Color3D : IEquatable<Color3D>, IFormattable, IVector<double>
{
    private const string toStringFormat = "R:{0} G:{1} B:{2}";

    /// <summary>
    /// Black (0D, 0D, 0D).
    /// </summary>
    public static readonly Color3D Black = new Color3D(0D, 0D, 0D);

    /// <summary>
    /// White (1D, 1D, 1D).
    /// </summary>
    public static readonly Color3D White = new Color3D(1D, 1D, 1D);

    /// <summary>
    /// Transparent (0D, 0D, 0D).
    /// </summary>
    public static readonly Color3D Zero = new Color3D(0D, 0D, 0D);

    /// <summary>
    /// Gets a value indicting whether this vector is zero
    /// </summary>
    public bool IsZero => R == 0D && G == 0D && B == 0D;

	/// <summary>The red component.</summary>
	[DataMember]
	[FieldOffset(0)]
	public double R;

	/// <summary>The green component.</summary>
	[DataMember]
	[FieldOffset(8)]
	public double G;

	/// <summary>The blue component.</summary>
	[DataMember]
	[FieldOffset(16)]
	public double B;

	/// <summary>A fixed array mapped to the same memory space as the individual <see cref="Color3D"/> components.</summary>
	[IgnoreDataMember]
	[FieldOffset(0)]
	public unsafe fixed double Values[3];

	/// <summary>
	/// Initializes a new instance of <see cref="Color3D"/>.
	/// </summary>
	/// <param name="r">The R component.</param>
	/// <param name="g">The G component.</param>
	/// <param name="b">The B component.</param>
	public Color3D(double r, double g, double b)
	{
		R = r;
		G = g;
		B = b;
	}
	/// <summary>Initializes a new instance of <see cref="Color3D"/>.</summary>
	/// <param name="value">The value that will be assigned to all components.</param>
	public Color3D(double value)
	{
		R = value;
		G = value;
		B = value;
	}
	/// <summary>Initializes a new instance of <see cref="Color3D"/> from an array.</summary>
	/// <param name="values">The values to assign to the R, G, B components of the color. This must be an array with at least 3 elements.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than 3 elements.</exception>
	public unsafe Color3D(double[] values)
	{
		if (values == null)
			throw new ArgumentNullException("values");
		if (values.Length < 3)
			throw new ArgumentOutOfRangeException("values", "There must be at least 3 input values for Color3D.");

		fixed (double* src = values)
		{
			fixed (double* dst = Values)
				Unsafe.CopyBlock(src, dst, (sizeof(double) * 3));
		}
	}
	/// <summary>Initializes a new instance of <see cref="Color3D"/> from a span.</summary>
	/// <param name="values">The values to assign to the R, G, B components of the color. This must be an array with at least 3 elements.</param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than 3 elements.</exception>
	public Color3D(Span<double> values)
	{
		if (values == null)
			throw new ArgumentNullException("values");
		if (values.Length < 3)
			throw new ArgumentOutOfRangeException("values", "There must be at least 3 input values for Color3D.");

		R = values[0];
		G = values[1];
		B = values[2];
	}
	/// <summary>Initializes a new instance of <see cref="Color3D"/> from a an unsafe pointer.</summary>
	/// <param name="ptrValues">The values to assign to the R, G, B components of the color.
	/// <para>There must be at least 3 elements available or undefined behaviour will occur.</para></param>
	/// <exception cref="ArgumentNullException">Thrown when <paramref name="ptrValues"/> is <c>null</c>.</exception>
	/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="ptrValues"/> contains more or less than 3 elements.</exception>
	public unsafe Color3D(double* ptrValues)
	{
		if (ptrValues == null)
			throw new ArgumentNullException("ptrValues");

		fixed (double* dst = Values)
			Unsafe.CopyBlock(ptrValues, dst, (sizeof(double) * 3));
	}

    /// <summary>
    /// Initializes a new instance of the <see cref="Color3D"/> struct.
    /// </summary>
    /// <param name="value">The R, G, B components of the color.</param>
    public Color3D(Vector3D value)
    {
        R = value.X;
        G = value.Y;
        B = value.Z;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Color3D"/> struct.
    /// </summary>
    /// <param name="rgb">A packed integer containing all three color components in R, G, B order.
    /// The alpha component is ignored.</param>
    public Color3D(int packed)
    {
        
        B = ((packed >> 16) & 255) / 255.0D;
        G = ((packed >> 8) & 255) / 255.0D;
        R = (packed & 255) / 255.0D;
    }

#region Indexers
	/// <summary> Gets or sets the component at the specified index. </summary>
	/// <value>The value of the <see cref="Color3D"/> component, depending on the index.</value>
	/// <param name="index">The index of the index component to access, ranging from 0 to 2, inclusive.</param>
	/// <returns>The value of the component at the specified index value provided.</returns>
	/// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
	public unsafe double this[int index]
	{
		get
		{
			if(index < 0 || index > 2)
				throw new IndexOutOfRangeException("index for Color3D must be between 0 and 2, inclusive.");

			return Values[index];
		}
		set
		{
			if(index < 0 || index > 2)
				throw new IndexOutOfRangeException("index for Color3D must be between 0 and 2, inclusive.");

			Values[index] = value;
		}
	}

	/// <summary> Gets or sets the component at the specified index. </summary>
	/// <value>The value of the <see cref="Color3D"/> component, depending on the index.</value>
	/// <param name="index">The index of the index component to access, ranging from 0 to 2, inclusive.</param>
	/// <returns>The value of the component at the specified index value provided.</returns>
	/// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
	public unsafe double this[uint index]
	{
		get
		{
			if(index > 2)
				throw new IndexOutOfRangeException("index for Color3D must be between 0 and 2, inclusive.");

			return Values[index];
		}
		set
		{
			if(index > 2)
				throw new IndexOutOfRangeException("index for Color3D must be between 0 and 2, inclusive.");

			Values[index] = value;
		}
	}

#endregion

    /// <summary>
    /// Converts the color into a packed integer.
    /// </summary>
    /// <returns>A packed integer containing all three color components.
    /// The alpha channel is set to 255.</returns>
    public int PackRGBA()
    {
			uint r = (uint)(R * 255.0D) & 255;
			uint g = (uint)(G * 255.0D) & 255;
			uint b = (uint)(B * 255.0D) & 255;
        uint a = 255;

        uint value = r;
        value |= g << 8;
        value |= b << 16;
        value |= a << 24;

        return (int)value;
    }

    /// <summary>
    /// Converts the color into a packed integer.
    /// </summary>
    /// <returns>A packed integer containing all 3 color components.The alpha channel is set to 255.</returns>
    public int PackBGRA()
    {
			uint r = (uint)(R * 255.0D) & 255;
			uint g = (uint)(G * 255.0D) & 255;
			uint b = (uint)(B * 255.0D) & 255;
        uint a = 255;

        uint value = b;
        value |= g << 8;
        value |= r << 16;
        value |= a << 24;

        return (int)value;
    }

    /// <summary>
    /// Converts the color into a three component vector.
    /// </summary>
    /// <returns>A three component vector containing the red, green, and blue components of the color.</returns>
    public Vector3D ToVector()
    {
        return new Vector3D(R, G, B);
    }

    /// <summary>
    /// Creates an array containing the elements of the color.
    /// </summary>
    /// <returns>A three-element array containing the components of the color.</returns>
    public double[] ToArray()
    {
        return new double[] { R, G, B };
    }

	///<summary>Performs a add operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to add.</param>
	///<param name="b">The second <see cref="Color3D"/> to add.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Add(ref Color3D a, ref Color3D b, out Color3D result)
	{
		result.R = a.R + b.R;
		result.G = a.G + b.G;
		result.B = a.B + b.B;
	}

	///<summary>Performs a add operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to add.</param>
	///<param name="b">The second <see cref="Color3D"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator +(Color3D a, Color3D b)
	{
		Add(ref a, ref b, out Color3D result);
		return result;
	}

	///<summary>Performs a add operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to add.</param>
	///<param name="b">The <see cref="double"/> to add.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Add(ref Color3D a, double b, out Color3D result)
	{
		result.R = a.R + b;
		result.G = a.G + b;
		result.B = a.B + b;
	}

	///<summary>Performs a add operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to add.</param>
	///<param name="b">The <see cref="double"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator +(Color3D a, double b)
	{
		Add(ref a, b, out Color3D result);
		return result;
	}

	///<summary>Performs a add operation on a <see cref="double"/> and a <see cref="Color3D"/>.</summary>
	///<param name="a">The <see cref="double"/> to add.</param>
	///<param name="b">The <see cref="Color3D"/> to add.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator +(double a, Color3D b)
	{
		Add(ref b, a, out Color3D result);
		return result;
	}


	///<summary>Performs a subtract operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to subtract.</param>
	///<param name="b">The second <see cref="Color3D"/> to subtract.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Subtract(ref Color3D a, ref Color3D b, out Color3D result)
	{
		result.R = a.R - b.R;
		result.G = a.G - b.G;
		result.B = a.B - b.B;
	}

	///<summary>Performs a subtract operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to subtract.</param>
	///<param name="b">The second <see cref="Color3D"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator -(Color3D a, Color3D b)
	{
		Subtract(ref a, ref b, out Color3D result);
		return result;
	}

	///<summary>Performs a subtract operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to subtract.</param>
	///<param name="b">The <see cref="double"/> to subtract.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Subtract(ref Color3D a, double b, out Color3D result)
	{
		result.R = a.R - b;
		result.G = a.G - b;
		result.B = a.B - b;
	}

	///<summary>Performs a subtract operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to subtract.</param>
	///<param name="b">The <see cref="double"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator -(Color3D a, double b)
	{
		Subtract(ref a, b, out Color3D result);
		return result;
	}

	///<summary>Performs a subtract operation on a <see cref="double"/> and a <see cref="Color3D"/>.</summary>
	///<param name="a">The <see cref="double"/> to subtract.</param>
	///<param name="b">The <see cref="Color3D"/> to subtract.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator -(double a, Color3D b)
	{
		Subtract(ref b, a, out Color3D result);
		return result;
	}


	///<summary>Performs a modulate operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to modulate.</param>
	///<param name="b">The second <see cref="Color3D"/> to modulate.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Modulate(ref Color3D a, ref Color3D b, out Color3D result)
	{
		result.R = a.R * b.R;
		result.G = a.G * b.G;
		result.B = a.B * b.B;
	}

	///<summary>Performs a modulate operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to modulate.</param>
	///<param name="b">The second <see cref="Color3D"/> to modulate.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator *(Color3D a, Color3D b)
	{
		Modulate(ref a, ref b, out Color3D result);
		return result;
	}

	///<summary>Performs a modulate operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to modulate.</param>
	///<param name="b">The <see cref="double"/> to modulate.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Modulate(ref Color3D a, double b, out Color3D result)
	{
		result.R = a.R * b;
		result.G = a.G * b;
		result.B = a.B * b;
	}

	///<summary>Performs a modulate operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to modulate.</param>
	///<param name="b">The <see cref="double"/> to modulate.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator *(Color3D a, double b)
	{
		Modulate(ref a, b, out Color3D result);
		return result;
	}

	///<summary>Performs a modulate operation on a <see cref="double"/> and a <see cref="Color3D"/>.</summary>
	///<param name="a">The <see cref="double"/> to modulate.</param>
	///<param name="b">The <see cref="Color3D"/> to modulate.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator *(double a, Color3D b)
	{
		Modulate(ref b, a, out Color3D result);
		return result;
	}


	///<summary>Performs a divide operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to divide.</param>
	///<param name="b">The second <see cref="Color3D"/> to divide.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Divide(ref Color3D a, ref Color3D b, out Color3D result)
	{
		result.R = a.R / b.R;
		result.G = a.G / b.G;
		result.B = a.B / b.B;
	}

	///<summary>Performs a divide operation on two <see cref="Color3D"/>.</summary>
	///<param name="a">The first <see cref="Color3D"/> to divide.</param>
	///<param name="b">The second <see cref="Color3D"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator /(Color3D a, Color3D b)
	{
		Divide(ref a, ref b, out Color3D result);
		return result;
	}

	///<summary>Performs a divide operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to divide.</param>
	///<param name="b">The <see cref="double"/> to divide.</param>
	///<param name="result">Output for the result of the operation.</param>
	public static void Divide(ref Color3D a, double b, out Color3D result)
	{
		result.R = a.R / b;
		result.G = a.G / b;
		result.B = a.B / b;
	}

	///<summary>Performs a divide operation on a <see cref="Color3D"/> and a <see cref="double"/>.</summary>
	///<param name="a">The <see cref="Color3D"/> to divide.</param>
	///<param name="b">The <see cref="double"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator /(Color3D a, double b)
	{
		Divide(ref a, b, out Color3D result);
		return result;
	}

	///<summary>Performs a divide operation on a <see cref="double"/> and a <see cref="Color3D"/>.</summary>
	///<param name="a">The <see cref="double"/> to divide.</param>
	///<param name="b">The <see cref="Color3D"/> to divide.</param>
	///<returns>The result of the operation.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color3D operator /(double a, Color3D b)
	{
		Divide(ref b, a, out Color3D result);
		return result;
	}


    /// <summary>
    /// Calculates the squared length of the vector.
    /// </summary>
    /// <returns>The squared length of the vector.</returns>
    /// <remarks>
    /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
    /// and speed is of the essence.
    /// </remarks>
    public double LengthSquared()
    {
        return ((R * R) + (G * G) + (B * B));
    }

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="value">The color to scale.</param>
    /// <param name="scale">The amount by which to scale.</param>
    /// <param name="result">When the method completes, contains the scaled color.</param>
    public static void Scale(ref Color3D value, double scale, out Color3D result)
    {
			result.R = value.R * scale;
			result.G = value.G * scale;
			result.B = value.B * scale;
    }

    /// <summary>
    /// Scales a color.
    /// </summary>
    /// <param name="value">The color to scale.</param>
    /// <param name="scale">The amount by which to scale.</param>
    /// <returns>The scaled color.</returns>
    public static Color3D Scale(Color3D value, double scale)
    {
        return new Color3D(value.R * scale, value.G * scale, value.B * scale);
    }

    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <param name="result">When the method completes, contains the negated color.</param>
    public static void Negate(ref Color3D value, out Color3D result)
    {
			result.R = 1D - value.R;
			result.G = 1D - value.G;
			result.B = 1D - value.B;
    }

    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <returns>The negated color.</returns>
    public static Color3D Negate(Color3D value)
    {
        return new Color3D(1D - value.R, 1D - value.G, 1D - value.B);
    }

    /// <summary>
    /// Restricts a color to within the component ranges of the specified min and max colors.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="result">When the method completes, contains the clamped value.</param>
    public static void Clamp(ref Color3D value, ref Color3D min, ref Color3D max, out Color3D result)
    {
        double red = value.R;
        red = (red > max.R) ? max.R : red;
        red = (red < min.R) ? min.R : red;

        double green = value.G;
        green = (green > max.G) ? max.G : green;
        green = (green < min.G) ? min.G : green;

        double blue = value.B;
        blue = (blue > max.B) ? max.B : blue;
        blue = (blue < min.B) ? min.B : blue;

        result = new Color3D(red, green, blue);
    }

    /// <summary>
    /// Restricts the current <see cref="Color3D"/> to within the component ranges of the specified min and max colors.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public void Clamp(ref Color3D min, ref Color3D max)
    {
        R = (R > max.R) ? max.R : R;
        R = (R < min.R) ? min.R : R;
        G = (G > max.G) ? max.G : G;
        G = (G < min.G) ? min.G : G;
        B = (B > max.B) ? max.B : B;
        B = (B < min.B) ? min.B : B;
    }

    /// <summary>
    /// Restricts each color component to within the specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="result">When the method completes, contains the clamped value.</param>
    public static void Clamp(ref Color3D value, double min, double max, out Color3D result)
    {
        double red = value.R;
        red = (red > max) ? max : red;
        red = (red < min) ? min : red;

        double green = value.G;
        green = (green > max) ? max : green;
        green = (green < min) ? min : green;

        double blue = value.B;
        blue = (blue > max) ? max : blue;
        blue = (blue < min) ? min : blue;

        result = new Color3D(red, green, blue);
    }

    /// <summary>
    /// Restricts each color component to within the specified range.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color3D Clamp(ref Color3D value, double min, double max)
    {
        Clamp(ref value, min, max, out Color3D result);
        return result;
    }

    /// <summary>
    /// Restricts each component of the current <see cref="Color3D"/> to within the specified range.
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    public void Clamp(double min, double max)
    {
        R = (R > max) ? max : R;
        R = (R < min) ? min : R;
        G = (G > max) ? max : G;
        G = (G < min) ? min : G;
        B = (B > max) ? max : B;
        B = (B < min) ? min : B;
    }

    /// <summary>
    /// Performs a linear interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the linear interpolation of the two colors.</param>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
    /// </remarks>
    public static void Lerp(ref Color3D start, ref Color3D end, double amount, out Color3D result)
    {
			result.R = MathHelper.Lerp(start.R, end.R, amount);
			result.G = MathHelper.Lerp(start.G, end.G, amount);
			result.B = MathHelper.Lerp(start.B, end.B, amount);
    }

    /// <summary>
    /// Performs a linear interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The linear interpolation of the two colors.</returns>
    /// <remarks>
    /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
    /// </remarks>
    public static Color3D Lerp(Color3D start, Color3D end, double amount)
    {
        Lerp(ref start, ref end, amount, out Color3D result);
        return result;
    }

    /// <summary>
    /// Performs a cubic interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <param name="result">When the method completes, contains the cubic interpolation of the two colors.</param>
    public static void SmoothStep(ref Color3D start, ref Color3D end, double amount, out Color3D result)
    {
        amount = MathHelper.SmoothStep(amount);
        Lerp(ref start, ref end, amount, out result);
    }

    /// <summary>
    /// Performs a cubic interpolation between two colors.
    /// </summary>
    /// <param name="start">Start color.</param>
    /// <param name="end">End color.</param>
    /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
    /// <returns>The cubic interpolation of the two colors.</returns>
    public static Color3D SmoothStep(Color3D start, Color3D end, double amount)
    {
        SmoothStep(ref start, ref end, amount, out Color3D result);
        return result;
    }

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <param name="result">When the method completes, contains an new color composed of the largest components of the source colors.</param>
    public static void Max(ref Color3D left, ref Color3D right, out Color3D result)
    {
			result.R = (left.R  > right.R ) ? left.R  : right.R ;
			result.G = (left.G  > right.G ) ? left.G  : right.G ;
			result.B = (left.B  > right.B ) ? left.B  : right.B ;
    }

    /// <summary>
    /// Returns a color containing the largest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <returns>A color containing the largest components of the source colors.</returns>
    public static Color3D Max(Color3D left, Color3D right)
    {
        Max(ref left, ref right, out Color3D result);
        return result;
    }

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <param name="result">When the method completes, contains an new color composed of the smallest components of the source colors.</param>
    public static void Min(ref Color3D left, ref Color3D right, out Color3D result)
    {
			result.R = (left.R < right.R) ? left.R : right.R;
			result.G = (left.G < right.G) ? left.G : right.G;
			result.B = (left.B < right.B) ? left.B : right.B;
    }

    /// <summary>
    /// Returns a color containing the smallest components of the specified colors.
    /// </summary>
    /// <param name="left">The first source color.</param>
    /// <param name="right">The second source color.</param>
    /// <returns>A color containing the smallest components of the source colors.</returns>
    public static Color3D Min(Color3D left, Color3D right)
    {
        Min(ref left, ref right, out Color3D result);
        return result;
    }

    /// <summary>
    /// Calculates the dot product of two <see cref="Color3D"/>.
    /// </summary>
    /// <param name="c0">The first <see cref="Color3D"/>.</param>
    /// <param name="c1">The second <see cref="Color3D"/>.</param>
    /// <returns></returns>
    public static double Dot(Color3D c0,Color3D c1)
    {
        return c0.R * c1.R + c0.G * c1.G + c0.B * c1.B;
    }

    /// <summary>
    /// Calculates the dot product of two <see cref="Color3D"/>.
    /// </summary>
    /// <param name="c0">The first <see cref="Color3D"/>.</param>
    /// <param name="c1">The second <see cref="Color3D"/>.</param>
    /// <returns></returns>
    public static double Dot(ref Color3D c0, ref Color3D c1)
    {
        return c0.R * c1.R + c0.G * c1.G + c0.B * c1.B;
    }

    /// <summary>
    /// Calculates the dot product of two <see cref="Color3D"/>.
    /// </summary>
    /// <param name="c0">The first <see cref="Color3D"/>.</param>
    /// <param name="c1">The second <see cref="Color3D"/>.</param>
    /// <param name="result">The destination for the result.</param>
    /// <returns></returns>
    public static void Dot(ref Color3D c0,ref Color3D  c1, out double result)
    {
        result = c0.R * c1.R + c0.G * c1.G + c0.B * c1.B;
    }

    /// <summary>
    /// Adjusts the contrast of a color.
    /// </summary>
    /// <param name="value">The color whose contrast is to be adjusted.</param>
    /// <param name="contrast">The amount by which to adjust the contrast.</param>
    /// <param name="result">When the method completes, contains the adjusted color.</param>
    public static void AdjustContrast(ref Color3D value, double contrast, out Color3D result)
    {
			result.R = 0.5D + contrast * (value.R - 0.5D);
			result.G = 0.5D + contrast * (value.G - 0.5D);
			result.B = 0.5D + contrast * (value.B - 0.5D);
    }

    /// <summary>
    /// Adjusts the contrast of a color.
    /// </summary>
    /// <param name="value">The color whose contrast is to be adjusted.</param>
    /// <param name="contrast">The amount by which to adjust the contrast.</param>
    /// <returns>The adjusted color.</returns>
    public static Color3D AdjustContrast(Color3D value, double contrast)
    {
        return new Color3D(
			    0.5D + contrast * (value.R - 0.5D),
			    0.5D + contrast * (value.G - 0.5D),
			    0.5D + contrast * (value.B - 0.5D)
        );
    }

    /// <summary>
    /// Adjusts the saturation of a color.
    /// </summary>
    /// <param name="value">The color whose saturation is to be adjusted.</param>
    /// <param name="saturation">The amount by which to adjust the saturation.</param>
    /// <param name="result">When the method completes, contains the adjusted color.</param>
    public static void AdjustSaturation(ref Color3D value, double saturation, out Color3D result)
    {
        double grey = value.R * 0.2125D + value.G * 0.7154D + value.B * 0.0721D;
			result.R = grey + saturation * (value.R  - grey);
			result.G = grey + saturation * (value.G  - grey);
			result.B = grey + saturation * (value.B  - grey);
    }

    /// <summary>
    /// Adjusts the saturation of a color.
    /// </summary>
    /// <param name="value">The color whose saturation is to be adjusted.</param>
    /// <param name="saturation">The amount by which to adjust the saturation.</param>
    /// <returns>The adjusted color.</returns>
    public static Color3D AdjustSaturation(Color3D value, double saturation)
    {
        double grey = value.R * 0.2125D + value.G * 0.7154D + value.B * 0.0721D;

        return new Color3D(
			    grey + saturation * (value.R - grey),
			    grey + saturation * (value.G - grey),
			    grey + saturation * (value.B - grey)
        );
    }

    /// <summary>
    /// Computes the premultiplied value of the provided color.
    /// </summary>
    /// <param name="value">The non-premultiplied value.</param>
    /// <param name="alpha">The color alpha.</param>
    /// <param name="result">The premultiplied result.</param>
    public static void Premultiply(ref Color3D value, double alpha, out Color3D result)
    {
			result.R = value.R * alpha;
			result.G = value.G * alpha;
			result.B = value.B * alpha;
    }

    /// <summary>
    /// Computes the premultiplied value of the provided color.
    /// </summary>
    /// <param name="value">The non-premultiplied value.</param>
    /// <param name="alpha">The color alpha.</param>
    /// <returns>The premultiplied color.</returns>
    public static Color3D Premultiply(Color3D value, double alpha)
    {
        Premultiply(ref value, alpha, out Color3D result);
        return result;
    }

    /// <summary>
    /// Assert a color (return it unchanged).
    /// </summary>
    /// <param name="value">The color to assert (unchanged).</param>
    /// <returns>The asserted (unchanged) color.</returns>
    public static Color3D operator +(Color3D value)
    {
        return value;
    }


    /// <summary>
    /// Negates a color.
    /// </summary>
    /// <param name="value">The color to negate.</param>
    /// <returns>A negated color.</returns>
    public static Color3D operator -(Color3D value)
    {
        return new Color3D(-value.R, -value.G, -value.B);
    }

    /// <summary>
    /// Tests for equality between two objects.
    /// </summary>
    /// <param name="left">The first value to compare.</param>
    /// <param name="right">The second value to compare.</param>
    /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Color3D left, Color3D right)
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
    public static bool operator !=(Color3D left, Color3D right)
    {
        return !left.Equals(ref right);
    }

	///<summary>Casts a <see cref="Color3D"/> to a <see cref="Color3"/>.</summary>
	public static explicit operator Color3(Color3D value)
	{
		return new Color3((float)value.R, (float)value.G, (float)value.B);
	}

	///<summary>Casts a <see cref="Color3D"/> to a <see cref="Color4"/>.</summary>
	public static explicit operator Color4(Color3D value)
	{
		return new Color4((float)value.R, (float)value.G, (float)value.B, 1F);
	}

	///<summary>Casts a <see cref="Color3D"/> to a <see cref="Color4D"/>.</summary>
	public static explicit operator Color4D(Color3D value)
	{
		return new Color4D(value.R, value.G, value.B, 1D);
	}

    /// <summary>
    /// Performs an implicit conversion from <see cref="Color3D"/> to <see cref="Vector3D"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Vector3D(Color3D value)
    {
        return new Vector3D(value.R, value.G, value.B);
    }

    /// <summary>
    /// Performs an implicit conversion from Vector3D to <see cref="Color3D"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator Color3D(Vector3D value)
    {
        return new Color3D(value.X, value.Y, value.Z);
    }

    /// <summary>
    /// Performs an explicit conversion from <see cref="System.Int32"/> to <see cref="Color3D"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The result of the conversion.</returns>
    public static explicit operator Color3D(int value)
    {
        return new Color3D(value);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return ToString(CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format to apply to each channel element (double)</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(string format)
    {
        return ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(IFormatProvider formatProvider)
    {
        return string.Format(formatProvider, toStringFormat, R, G, B);
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents this instance.
    /// </summary>
    /// <param name="format">The format to apply to each channel element (double).</param>
    /// <param name="formatProvider">The format provider.</param>
    /// <returns>
    /// A <see cref="string"/> that represents this instance.
    /// </returns>
    public string ToString(string format, IFormatProvider formatProvider)
    {
        if (format == null)
            return ToString(formatProvider);

        return string.Format(formatProvider,
            toStringFormat,
				R.ToString(format, formatProvider),
				G.ToString(format, formatProvider),
				B.ToString(format, formatProvider)
        );
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
            var hashCode = R.GetHashCode();
				hashCode = (hashCode * 397) ^ G.GetHashCode();
				hashCode = (hashCode * 397) ^ B.GetHashCode();
            return hashCode;
        }
    }

    /// <summary>
    /// Determines whether the specified <see cref="Color3D"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Color3D"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="Color3D"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(ref Color3D other)
    {
        return R == other.R && G == other.G && B == other.B;
    }

    /// <summary>
    /// Determines whether the specified <see cref="Color3D"/> is equal to this instance.
    /// </summary>
    /// <param name="other">The <see cref="Color3D"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="Color3D"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Color3D other)
    {
        return Equals(ref other);
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object value)
    {
        if (!(value is Color3D))
            return false;

        var strongValue = (Color3D)value;
        return Equals(ref strongValue);
    }


    /// <summary>
    /// Returns a new <see cref="Color3D"/> with the values of the provided color's components assigned based on their index.<para/>
    /// For example, a swizzle input of (2,2,3) on a <see cref="Color3D"/> with RGBA values of 100,20,255, will return a <see cref="Color4"/> with values 20,20,255.
    /// </summary>
    /// <param name="col">The color to use as a source for values.</param>
			/// <param name="rIndex">The component index of the source color to use for the new red value. This should be a value between 0 and 2.</param>
			/// <param name="gIndex">The component index of the source color to use for the new green value. This should be a value between 0 and 2.</param>
			/// <param name="bIndex">The component index of the source color to use for the new blue value. This should be a value between 0 and 2.</param>
    /// <returns></returns>
    public static unsafe Color3D Swizzle(Color3D col, int rIndex, int gIndex, int bIndex)
    {
        return new Color3D()
        {
				R = *(&col.R + (rIndex * sizeof(double))),
				G = *(&col.G + (gIndex * sizeof(double))),
				B = *(&col.B + (bIndex * sizeof(double))),
        };
    }
}

