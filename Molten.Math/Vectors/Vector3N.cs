using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten.Math
{
	///<summary>A <see cref = "nint"/> vector comprised of three components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=sizeof(nint))]
	public partial struct Vector3N : IFormattable
	{
		///<summary>The X component.</summary>
		public nint X;

		///<summary>The Y component.</summary>
		public nint Y;

		///<summary>The Z component.</summary>
		public nint Z;


		///<summary>The size of <see cref="Vector3N"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3N));

		///<summary>A Vector3N with every component set to 1.</summary>
		public static readonly Vector3N One = new Vector3N(1, 1, 1);

		/// <summary>The X unit <see cref="Vector3N"/>.</summary>
		public static readonly Vector3N UnitX = new Vector3N(1, 0, 0);

		/// <summary>The Y unit <see cref="Vector3N"/>.</summary>
		public static readonly Vector3N UnitY = new Vector3N(0, 1, 0);

		/// <summary>The Z unit <see cref="Vector3N"/>.</summary>
		public static readonly Vector3N UnitZ = new Vector3N(0, 0, 1);

		/// <summary>Represents a zero'd Vector3N.</summary>
		public static readonly Vector3N Zero = new Vector3N(0, 0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelper.IsOne((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0 && Z == 0;
        }

#region Constructors
		///<summary>Creates a new instance of <see cref = "Vector3N"/>.</summary>
		public Vector3N(nint x, nint y, nint z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector3N"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y and Z components of the vector. This must be an array with 3 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Vector3N(nint[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 3)
                throw new ArgumentOutOfRangeException("values", "There must be 3 and only 3 input values for Vector3N.");

			X = values[0];
			Y = values[1];
			Z = values[2];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector3N"/> struct from an unsafe pointer. The pointer should point to an array of three elements.
        /// </summary>
		public unsafe Vector3N(nint* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
			Z = ptr[2];
		}
#endregion

#region Instance Functions
        /// <summary>
        /// Determines whether the specified <see cref="Vector3N"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector3N"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3N"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Vector3N other)
        {
            return MathHelper.NearEqual(other.X, X) && MathHelper.NearEqual(other.Y, Y) && MathHelper.NearEqual(other.Z, Z);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3N"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector3N"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3N"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3N other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector3N"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector3N"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector3N"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Vector3N))
                return false;

            var strongValue = (Vector3N)value;
            return Equals(ref strongValue);
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
                return hashCode;
            }
        }

        /// <summary>
        /// Calculates the length of the vector.
        /// </summary>
        /// <returns>The length of the vector.</returns>
        /// <remarks>
        /// <see cref="Vector2F.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public nint Length()
        {
            return (nint)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public nint LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        public void Normalize()
        {
            nint length = Length();
            if (!MathHelper.IsZero(length))
            {
                nint inverse = 1.0f / length;
			    X *= inverse;
			    Y *= inverse;
			    Z *= inverse;
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Vector3N"/>.
        /// </summary>
        /// <returns>A three-element array containing the components of the vector.</returns>
        public nint[] ToArray()
        {
            return new nint[] { X, Y, Z};
        }

		/// <summary>
        /// Reverses the direction of the current <see cref="Vector3N"/>.
        /// </summary>
        /// <returns>A <see cref="Vector3N"/> facing the opposite direction.</returns>
		public Vector3N Negate()
		{
			return new Vector3N(-X, -Y, -Z);
		}
		

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(nint min, nint max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
			Z = Z < min ? min : Z > max ? max : Z;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Vector3N min, Vector3N max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
			Z = Z < min.Z ? min.Z : Z > max.Z ? max.Z : Z;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector3N"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Z.ToString(format, formatProvider));
        }
#endregion

#region Add operators
		public static Vector3N operator +(Vector3N left, Vector3N right)
		{
			return new Vector3N(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
		}

		public static Vector3N operator +(Vector3N left, nint right)
		{
			return new Vector3N(left.X + right, left.Y + right, left.Z + right);
		}

		/// <summary>
        /// Assert a <see cref="Vector3N"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Vector3N"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Vector3N"/>.</returns>
        public static Vector3N operator +(Vector3N value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static Vector3N operator -(Vector3N left, Vector3N right)
		{
			return new Vector3N(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
		}

		public static Vector3N operator -(Vector3N left, nint right)
		{
			return new Vector3N(left.X - right, left.Y - right, left.Z - right);
		}

		/// <summary>
        /// Negate/reverse the direction of a <see cref="Vector3N"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector3N"/> to reverse.</param>
        /// <returns>The reversed <see cref="Vector3N"/>.</returns>
        public static Vector3N operator -(Vector3N value)
        {
            return new Vector3N(-value.X, -value.Y, -value.Z);
        }
#endregion

#region division operators
		public static Vector3N operator /(Vector3N left, Vector3N right)
		{
			return new Vector3N(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
		}

		public static Vector3N operator /(Vector3N left, nint right)
		{
			return new Vector3N(left.X / right, left.Y / right, left.Z / right);
		}
#endregion

#region Multiply operators
		public static Vector3N operator *(Vector3N left, Vector3N right)
		{
			return new Vector3N(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}

		public static Vector3N operator *(Vector3N left, nint right)
		{
			return new Vector3N(left.X * right, left.Y * right, left.Z * right);
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
        public static bool operator ==(Vector3N left, Vector3N right)
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
        public static bool operator !=(Vector3N left, Vector3N right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Vector3N"/> to a <see cref="Vector2N"/>.</summary>
        public static explicit operator Vector2N(Vector3N value)
        {
            return new Vector2N(value.X, value.Y);
        }

        ///<summary>Casts a <see cref="Vector3N"/> to a <see cref="Vector4N"/>.</summary>
        public static explicit operator Vector4N(Vector3N value)
        {
            return new Vector4N(value.X, value.Y, value.Z, 0);
        }

#endregion

#region Static Methods
        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector3N"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Vector2F"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
		/// <param name="zIndex">The axis index to use for the new Z value.</param>
        /// <returns></returns>
        public static unsafe Vector3N Swizzle(Vector3N val, int xIndex, int yIndex, int zIndex)
        {
            return new Vector3N()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
			   Z = (&val.X)[zIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Vector3N Swizzle(Vector3N val, uint xIndex, uint yIndex, uint zIndex)
        {
            return new Vector3N()
            {
			    X = (&val.X)[xIndex],
			    Y = (&val.X)[yIndex],
			    Z = (&val.X)[zIndex],
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector2F.DistanceSquared(Vector2F, Vector2F)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static nint Distance(Vector3N value1, Vector3N value2)
        {
			nint x = value1.X - value2.X;
			nint y = value1.Y - value2.Y;
			nint z = value1.Z - value2.Z;

            return (nint)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Vector3N Pow(Vector3N vec, nint power)
        {
            return new Vector3N()
            {
				X = (nint)Math.Pow(vec.X, power),
				Y = (nint)Math.Pow(vec.Y, power),
				Z = (nint)Math.Pow(vec.Z, power),
            };
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Vector3N"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector3N"/> source vector</param>
        /// <param name="right">Second <see cref="Vector3N"/> source vector.</param>
        public static nint Dot(Vector3N left, Vector3N right)
        {
			return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector3N"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector3N"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Vector3N"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector3N"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector3N Hermite(ref Vector3N value1, ref Vector3N tangent1, ref Vector3N value2, ref Vector3N tangent2, nint amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0F * cubed) - (3.0F * squared)) + 1.0F;
            float part2 = (-2.0F * cubed) + (3.0F * squared);
            float part3 = (cubed - (2.0F * squared)) + amount;
            float part4 = cubed - squared;

			return new Vector3N()
			{
				X = (nint)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (nint)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
				Z = (nint)((((value1.Z * part1) + (value2.Z * part2)) + (tangent1.Z * part3)) + (tangent2.Z * part4)),
			};
        }

		/// <summary>
        /// Returns a <see cref="Vector3N"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector3N"/> containing the 3D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector3N"/> containing the 3D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector3N"/> containing the 3D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector3N Barycentric(ref Vector3N value1, ref Vector3N value2, ref Vector3N value3, nint amount1, nint amount2)
        {
			return new Vector3N(
				(value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)), 
				(value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)), 
				(value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z))
			);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Vector3N"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector3N Lerp(ref Vector3N start, ref Vector3N end, float amount)
        {
			return new Vector3N()
			{
				X = (nint)((1F - amount) * start.X + amount * end.X),
				Y = (nint)((1F - amount) * start.Y + amount * end.Y),
				Z = (nint)((1F - amount) * start.Z + amount * end.Z),
			};
        }

		/// <summary>
        /// Returns a <see cref="Vector3N"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3N"/>.</param>
        /// <param name="right">The second source <see cref="Vector3N"/>.</param>
        /// <returns>A <see cref="Vector3N"/> containing the smallest components of the source vectors.</returns>
		public static Vector3N Min(Vector3N left, Vector3N right)
		{
			return new Vector3N()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
				Z = (left.Z < right.Z) ? left.Z : right.Z,
			};
		}

		/// <summary>
        /// Returns a <see cref="Vector3N"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector3N"/>.</param>
        /// <param name="right">The second source <see cref="Vector3N"/>.</param>
        /// <returns>A <see cref="Vector3N"/> containing the largest components of the source vectors.</returns>
		public static Vector3N Max(Vector3N left, Vector3N right)
		{
			return new Vector3N()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
				Z = (left.Z > right.Z) ? left.Z : right.Z,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector3N"/> vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector</param>
        /// <param name="result">When the method completes, contains the squared distance between the two vectors.</param>
        /// <remarks>Distance squared is the value before taking the square root. 
        /// Distance squared can often be used in place of distance if relative comparisons are being made. 
        /// For example, consider three points A, B, and C. To determine whether B or C is further from A, 
        /// compare the distance between A and B to the distance between A and C. Calculating the two distances 
        /// involves two square roots, which are computationally expensive. However, using distance squared 
        /// provides the same information and avoids calculating two square roots.
        /// </remarks>
		public static void DistanceSquared(ref Vector3N value1, ref Vector3N value2, out nint result)
        {
            nint x = value1.X - value2.X;
            nint y = value1.Y - value2.Y;
            nint z = value1.Z - value2.Z;

            result = (x * x) + (y * y) + (z * z);
        }

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector3N"/> vectors.
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
		public static nint DistanceSquared(ref Vector3N value1, ref Vector3N value2)
        {
            nint x = value1.X - value2.X;
            nint y = value1.Y - value2.Y;
            nint z = value1.Z - value2.Z;

            return (x * x) + (y * y) + (z * z);
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector3N"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector3N Clamp(Vector3N value, nint min, nint max)
        {
			return new Vector3N()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
				Z = value.Z < min ? min : value.Z > max ? max : value.Z,
			};
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector3N"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector3N Clamp(Vector3N value, Vector3N min, Vector3N max)
        {
			return new Vector3N()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
				Z = value.Z < min.Z ? min.Z : value.Z > max.Z ? max.Z : value.Z,
			};
        }
#endregion

#region Indexers
		/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y or Z component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component and so on.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 2].</exception>
        
		public nint this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
					case 2: return Z;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector3N run from 0 to 2, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
					case 2: Z = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector3N run from 0 to 2, inclusive.");
			}
		}
#endregion
	}
}

