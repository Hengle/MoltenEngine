using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Molten
{
	///<summary>A <see cref = "int"/> vector comprised of two components.</summary>
	[StructLayout(LayoutKind.Sequential, Pack=4)]
	public partial struct Vector2I : IFormattable
	{
		///<summary>The X component.</summary>
		public int X;

		///<summary>The Y component.</summary>
		public int Y;


		///<summary>The size of <see cref="Vector2I"/>, in bytes.</summary>
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2I));

		///<summary>A Vector2I with every component set to 1.</summary>
		public static readonly Vector2I One = new Vector2I(1, 1);

		/// <summary>The X unit <see cref="Vector2I"/>.</summary>
		public static readonly Vector2I UnitX = new Vector2I(1, 0);

		/// <summary>The Y unit <see cref="Vector2I"/>.</summary>
		public static readonly Vector2I UnitY = new Vector2I(0, 1);

		/// <summary>Represents a zero'd Vector2I.</summary>
		public static readonly Vector2I Zero = new Vector2I(0, 0);

		 /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get => MathHelper.IsOne((X * X) + (Y * Y));
        }

        /// <summary>
        /// Gets a value indicting whether this vector is zero
        /// </summary>
        public bool IsZero
        {
            get => X == 0 && Y == 0;
        }

#region Constructors
		///<summary>Creates a new instance of <see cref = "Vector2I"/>.</summary>
		public Vector2I(int x, int y)
		{
			X = x;
			Y = y;
		}

        ///<summary>Creates a new instance of <see cref = "Vector2I"/>.</summary>
		public Vector2I(int value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector2I"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X and Y components of the vector. This must be an array with 2 elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than two elements.</exception>
        public Vector2I(int[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 2)
                throw new ArgumentOutOfRangeException("values", "There must be 2 and only 2 input values for Vector2I.");

			X = values[0];
			Y = values[1];
        }

		/// <summary>
        /// Initializes a new instance of the <see cref="Vector2I"/> struct from an unsafe pointer. The pointer should point to an array of two elements.
        /// </summary>
		public unsafe Vector2I(int* ptr)
		{
			X = ptr[0];
			Y = ptr[1];
		}
#endregion

#region Instance Methods
        /// <summary>
        /// Determines whether the specified <see cref="Vector2I"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector2I"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2I"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Vector2I other)
        {
            return MathHelper.NearEqual(other.X, X) && MathHelper.NearEqual(other.Y, Y);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector2I"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector2I"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2I"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector2I other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Vector2I"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="Vector2I"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Vector2I"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is Vector2I))
                return false;

            var strongValue = (Vector2I)value;
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
        public int Length()
        {
            return (int)Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        /// Calculates the squared length of the vector.
        /// </summary>
        /// <returns>The squared length of the vector.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="Vector2F.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public int LengthSquared()
        {
            return ((X * X) + (Y * Y));
        }

        /// <summary>
        /// Converts the vector into a unit vector.
        /// </summary>
        public void Normalize()
        {
            int length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
			    X = (int)(X * inverse);
			    Y = (int)(Y * inverse);
            }
        }

		/// <summary>
        /// Creates an array containing the elements of the current <see cref="Vector2I"/>.
        /// </summary>
        /// <returns>A two-element array containing the components of the vector.</returns>
        public int[] ToArray()
        {
            return new int[] { X, Y};
        }
		/// <summary>
        /// Reverses the direction of the current <see cref="Vector2I"/>.
        /// </summary>
        /// <returns>A <see cref="Vector2I"/> facing the opposite direction.</returns>
		public Vector2I Negate()
		{
			return new Vector2I(-X, -Y);
		}
		
        /// <summary>
        /// Returns a normalized unit vector of the original vector.
        /// </summary>
        public Vector2I GetNormalized()
        {
            float length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
                return new Vector2I()
                {
			        X = (int)(this.X * inverse),
			        Y = (int)(this.Y * inverse),
                };
            }
            else
            {
                return new Vector2I();
            }
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(int min, int max)
        {
			X = X < min ? min : X > max ? max : X;
			Y = Y < min ? min : Y > max ? max : Y;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public void Clamp(Vector2I min, Vector2I max)
        {
			X = X < min.X ? min.X : X > max.X ? max.X : X;
			Y = Y < min.Y ? min.Y : Y > max.Y ? max.Y : Y;
        }
#endregion

#region To-String

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", 
			X.ToString(format, CultureInfo.CurrentCulture), Y.ToString(format, CultureInfo.CurrentCulture));
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1}", X, Y);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1}", X, Y);
        }

		/// <summary>
        /// Returns a <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="format">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this <see cref="Vector2I"/>.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1}", X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
        }
#endregion

#region Add operators
        public static void Add(ref Vector2I left, ref Vector2I right, out Vector2I result)
        {
			result.X = (left.X + right.X);
			result.Y = (left.Y + right.Y);
        }

        public static void Add(ref Vector2I left, int right, out Vector2I result)
        {
			result.X = (left.X + right);
			result.Y = (left.Y + right);
        }

		public static Vector2I operator +(Vector2I left, Vector2I right)
		{
			Add(ref left, ref right, out Vector2I result);
            return result;
		}

		public static Vector2I operator +(Vector2I left, int right)
		{
            Add(ref left, right, out Vector2I result);
            return result;
		}

        public static Vector2I operator +(int left, Vector2I right)
		{
            Add(ref right, left, out Vector2I result);
            return result;
		}

		/// <summary>
        /// Assert a <see cref="Vector2I"/> (return it unchanged).
        /// </summary>
        /// <param name="value">The <see cref="Vector2I"/> to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) <see cref="Vector2I"/>.</returns>
        public static Vector2I operator +(Vector2I value)
        {
            return value;
        }
#endregion

#region Subtract operators
		public static void Subtract(ref Vector2I left, ref Vector2I right, out Vector2I result)
        {
			result.X = (left.X - right.X);
			result.Y = (left.Y - right.Y);
        }

        public static void Subtract(ref Vector2I left, int right, out Vector2I result)
        {
			result.X = (left.X - right);
			result.Y = (left.Y - right);
        }

		public static Vector2I operator -(Vector2I left, Vector2I right)
		{
			Subtract(ref left, ref right, out Vector2I result);
            return result;
		}

		public static Vector2I operator -(Vector2I left, int right)
		{
            Subtract(ref left, right, out Vector2I result);
            return result;
		}

        public static Vector2I operator -(int left, Vector2I right)
		{
            Subtract(ref right, left, out Vector2I result);
            return result;
		}

        /// <summary>
        /// Negate/reverse the direction of a <see cref="Vector3D"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector2I"/> to reverse.</param>
        /// <param name="result">The output for the reversed <see cref="Vector2I"/>.</param>
        public static void Negate(ref Vector2I value, out Vector2I result)
        {
			result.X = -value.X;
			result.Y = -value.Y;
            
        }

		/// <summary>
        /// Negate/reverse the direction of a <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="value">The <see cref="Vector2I"/> to reverse.</param>
        /// <returns>The reversed <see cref="Vector2I"/>.</returns>
        public static Vector2I operator -(Vector2I value)
        {
            Negate(ref value, out value);
            return value;
        }
#endregion

#region division operators
		public static void Divide(ref Vector2I left, ref Vector2I right, out Vector2I result)
        {
			result.X = (left.X / right.X);
			result.Y = (left.Y / right.Y);
        }

        public static void Divide(ref Vector2I left, int right, out Vector2I result)
        {
			result.X = (left.X / right);
			result.Y = (left.Y / right);
        }

		public static Vector2I operator /(Vector2I left, Vector2I right)
		{
			Divide(ref left, ref right, out Vector2I result);
            return result;
		}

		public static Vector2I operator /(Vector2I left, int right)
		{
            Divide(ref left, right, out Vector2I result);
            return result;
		}

        public static Vector2I operator /(int left, Vector2I right)
		{
            Divide(ref right, left, out Vector2I result);
            return result;
		}
#endregion

#region Multiply operators
		public static void Multiply(ref Vector2I left, ref Vector2I right, out Vector2I result)
        {
			result.X = (left.X * right.X);
			result.Y = (left.Y * right.Y);
        }

        public static void Multiply(ref Vector2I left, int right, out Vector2I result)
        {
			result.X = (left.X * right);
			result.Y = (left.Y * right);
        }

		public static Vector2I operator *(Vector2I left, Vector2I right)
		{
			Multiply(ref left, ref right, out Vector2I result);
            return result;
		}

		public static Vector2I operator *(Vector2I left, int right)
		{
            Multiply(ref left, right, out Vector2I result);
            return result;
		}

        public static Vector2I operator *(int left, Vector2I right)
		{
            Multiply(ref right, left, out Vector2I result);
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
        public static bool operator ==(Vector2I left, Vector2I right)
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
        public static bool operator !=(Vector2I left, Vector2I right)
        {
            return !left.Equals(ref right);
        }
#endregion

#region Operators - Cast
        ///<summary>Casts a <see cref="Vector2I"/> to a <see cref="Vector3I"/>.</summary>
        public static explicit operator Vector3I(Vector2I value)
        {
            return new Vector3I(value.X, value.Y, 0);
        }

        ///<summary>Casts a <see cref="Vector2I"/> to a <see cref="Vector4I"/>.</summary>
        public static explicit operator Vector4I(Vector2I value)
        {
            return new Vector4I(value.X, value.Y, 0, 0);
        }

#endregion

#region Static Methods
        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(Vector2I left, Vector2I right, Vector2I epsilon)
        {
            return NearEqual(ref left, ref right, ref epsilon);
        }

        /// <summary>
        /// Tests whether one 3D vector is near another 3D vector.
        /// </summary>
        /// <param name="left">The left vector.</param>
        /// <param name="right">The right vector.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns><c>true</c> if left and right are near another 3D, <c>false</c> otherwise</returns>
        public static bool NearEqual(ref Vector2I left, ref Vector2I right, ref Vector2I epsilon)
        {
            return MathHelper.WithinEpsilon(left.X, right.X, epsilon.X) && MathHelper.WithinEpsilon(left.Y, right.Y, epsilon.Y);
        }

        /// <summary>
        /// Performs a cubic interpolation between two vectors.
        /// </summary>
        /// <param name="start">Start vector.</param>
        /// <param name="end">End vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        public static Vector2I SmoothStep(ref Vector2I start, ref Vector2I end, float amount)
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
        public static Vector2I SmoothStep(Vector2I start, Vector2I end, int amount)
        {
            return SmoothStep(ref start, ref end, amount);
        }    

        /// <summary>
        /// Orthogonalizes a list of <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="destination">The list of orthogonalized <see cref="Vector2I"/>.</param>
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
        public static void Orthogonalize(Vector2I[] destination, params Vector2I[] source)
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
                Vector2I newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= (Dot(destination[r], newvector) / Dot(destination[r], destination[r])) * destination[r];

                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Orthonormalizes a list of vectors.
        /// </summary>
        /// <param name="destination">The list of orthonormalized vectors.</param>
        /// <param name="source">The list of vectors to orthonormalize.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all vectors orthogonal to each
        /// other and making all vectors of unit length. This means that any given vector will
        /// be orthogonal to any other given vector in the list.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting vectors
        /// tend to be numerically unstable. The numeric stability decreases according to the vectors
        /// position in the list so that the first vector is the most stable and the last vector is the
        /// least stable.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        public static void Orthonormalize(Vector2I[] destination, params Vector2I[] source)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthogonalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|
            //q5 = ...

            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Vector2I newvector = source[i];

                for (int r = 0; r < i; ++r)
                    newvector -= Dot(destination[r], newvector) * destination[r];

                newvector.Normalize();
                destination[i] = newvector;
            }
        }

        /// <summary>
        /// Takes the value of an indexed component and assigns it to the axis of a new <see cref="Vector2I"/>. <para />
        /// For example, a swizzle input of (1,1) on a <see cref="Vector2I"/> with the values, 20 and 10, will return a vector with values 10,10, because it took the value of component index 1, for both axis."
        /// </summary>
        /// <param name="val">The current vector.</param>
		/// <param name="xIndex">The axis index to use for the new X value.</param>
		/// <param name="yIndex">The axis index to use for the new Y value.</param>
        /// <returns></returns>
        public static unsafe Vector2I Swizzle(Vector2I val, int xIndex, int yIndex)
        {
            return new Vector2I()
            {
			   X = (&val.X)[xIndex],
			   Y = (&val.X)[yIndex],
            };
        }

        /// <returns></returns>
        public static unsafe Vector2I Swizzle(Vector2I val, uint xIndex, uint yIndex)
        {
            return new Vector2I()
            {
			    X = (&val.X)[xIndex],
			    Y = (&val.X)[yIndex],
            };
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector2I.DistanceSquared(Vector2I, Vector2I)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static int Distance(ref Vector2I value1, ref Vector2I value2)
        {
			int x = (value1.X - value2.X);
			int y = (value1.Y - value2.Y);

            return (int)Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <remarks>
        /// <see cref="Vector2I.DistanceSquared(Vector2I, Vector2I)"/> may be preferred when only the relative distance is needed
        /// and speed is of the essence.
        /// </remarks>
        public static int Distance(Vector2I value1, Vector2I value2)
        {
			int x = (value1.X - value2.X);
			int y = (value1.Y - value2.Y);

            return (int)Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>Checks to see if any value (x, y, z, w) are within 0.0001 of 0.
        /// If so this method truncates that value to zero.</summary>
        /// <param name="power">The power.</param>
        /// <param name="vec">The vector.</param>
        public static Vector2I Pow(Vector2I vec, int power)
        {
            return new Vector2I()
            {
				X = (int)Math.Pow(vec.X, power),
				Y = (int)Math.Pow(vec.Y, power),
            };
        }

        /// <summary>
        /// Calculates the dot product of two <see cref="Vector2I"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector2I"/> source vector</param>
        /// <param name="right">Second <see cref="Vector2I"/> source vector.</param>
        public static int Dot(ref Vector2I left, ref Vector2I right)
        {
			return ((left.X * right.X) + (left.Y * right.Y));
        }

		/// <summary>
        /// Calculates the dot product of two <see cref="Vector2I"/> vectors.
        /// </summary>
        /// <param name="left">First <see cref="Vector2I"/> source vector</param>
        /// <param name="right">Second <see cref="Vector2I"/> source vector.</param>
        public static int Dot(Vector2I left, Vector2I right)
        {
			return ((left.X * right.X) + (left.Y * right.Y));
        }

		/// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector2I"/> vector.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector2I"/> vector.</param>
        /// <param name="value2">Second source position <see cref="Vector2I"/> vector.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector2I"/> vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector2I Hermite(ref Vector2I value1, ref Vector2I tangent1, ref Vector2I value2, ref Vector2I tangent2, int amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;
            float part1 = ((2.0F * cubed) - (3.0F * squared)) + 1.0F;
            float part2 = (-2.0F * cubed) + (3.0F * squared);
            float part3 = (cubed - (2.0F * squared)) + amount;
            float part4 = cubed - squared;

			return new Vector2I()
			{
				X = (int)((((value1.X * part1) + (value2.X * part2)) + (tangent1.X * part3)) + (tangent2.X * part4)),
				Y = (int)((((value1.Y * part1) + (value2.Y * part2)) + (tangent1.Y * part3)) + (tangent2.Y * part4)),
			};
        }

        /// <summary>
        /// Performs a Hermite spline interpolation.
        /// </summary>
        /// <param name="value1">First source position <see cref="Vector2I"/>.</param>
        /// <param name="tangent1">First source tangent <see cref="Vector2I"/>.</param>
        /// <param name="value2">Second source position <see cref="Vector2I"/>.</param>
        /// <param name="tangent2">Second source tangent <see cref="Vector2I"/>.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>The result of the Hermite spline interpolation.</returns>
        public static Vector2I Hermite(Vector2I value1, Vector2I tangent1, Vector2I value2, Vector2I tangent2, int amount)
        {
            return Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount);
        }

		/// <summary>
        /// Returns a <see cref="Vector2I"/> containing the 2D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="Vector2I"/> containing the 2D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="Vector2I"/> containing the 2D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="Vector2I"/> containing the 2D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        public static Vector2I Barycentric(ref Vector2I value1, ref Vector2I value2, ref Vector2I value3, int amount1, int amount2)
        {
			return new Vector2I(
				((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X))), 
				((value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)))
			);
        }

        /// <summary>
        /// Performs a linear interpolation between two <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref Vector2I start, ref Vector2I end, float amount, out Vector2I result)
        {
			result.X = (int)((1F - amount) * start.X + amount * end.X);
			result.Y = (int)((1F - amount) * start.Y + amount * end.Y);
        }

		/// <summary>
        /// Performs a linear interpolation between two <see cref="Vector2I"/>.
        /// </summary>
        /// <param name="start">The start vector.</param>
        /// <param name="end">The end vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static Vector2I Lerp(ref Vector2I start, ref Vector2I end, float amount)
        {
			return new Vector2I()
			{
				X = (int)((1F - amount) * start.X + amount * end.X),
				Y = (int)((1F - amount) * start.Y + amount * end.Y),
			};
        }

        /// <summary>
        /// Returns a <see cref="Vector2I"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the smallest components of the source vectors.</returns>
		public static void Min(ref Vector2I left, ref Vector2I right, out Vector2I result)
		{
				result.X = (left.X < right.X) ? left.X : right.X;
				result.Y = (left.Y < right.Y) ? left.Y : right.Y;
		}

        /// <summary>
        /// Returns a <see cref="Vector2I"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the smallest components of the source vectors.</returns>
		public static Vector2I Min(ref Vector2I left, ref Vector2I right)
		{
			Min(ref left, ref right, out Vector2I result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector2I"/> containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the smallest components of the source vectors.</returns>
		public static Vector2I Min(Vector2I left, Vector2I right)
		{
			return new Vector2I()
			{
				X = (left.X < right.X) ? left.X : right.X,
				Y = (left.Y < right.Y) ? left.Y : right.Y,
			};
		}

        /// <summary>
        /// Returns a <see cref="Vector2I"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the largest components of the source vectors.</returns>
		public static void Max(ref Vector2I left, ref Vector2I right, out Vector2I result)
		{
				result.X = (left.X > right.X) ? left.X : right.X;
				result.Y = (left.Y > right.Y) ? left.Y : right.Y;
		}

        /// <summary>
        /// Returns a <see cref="Vector2I"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the largest components of the source vectors.</returns>
		public static Vector2I Max(ref Vector2I left, ref Vector2I right)
		{
			Max(ref left, ref right, out Vector2I result);
            return result;
		}

		/// <summary>
        /// Returns a <see cref="Vector2I"/> containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">The first source <see cref="Vector2I"/>.</param>
        /// <param name="right">The second source <see cref="Vector2I"/>.</param>
        /// <returns>A <see cref="Vector2I"/> containing the largest components of the source vectors.</returns>
		public static Vector2I Max(Vector2I left, Vector2I right)
		{
			return new Vector2I()
			{
				X = (left.X > right.X) ? left.X : right.X,
				Y = (left.Y > right.Y) ? left.Y : right.Y,
			};
		}

		/// <summary>
        /// Calculates the squared distance between two <see cref="Vector2I"/> vectors.
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
		public static int DistanceSquared(ref Vector2I value1, ref Vector2I value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return ((x * x) + (y * y));
        }

        /// <summary>
        /// Calculates the squared distance between two <see cref="Vector2I"/> vectors.
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
		public static int DistanceSquared(Vector2I value1, Vector2I value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return ((x * x) + (y * y));
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2I"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector2I Clamp(Vector2I value, int min, int max)
        {
			return new Vector2I()
			{
				X = value.X < min ? min : value.X > max ? max : value.X,
				Y = value.Y < min ? min : value.Y > max ? max : value.Y,
			};
        }

        /// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2I"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector2I Clamp(ref Vector2I value, ref Vector2I min, ref Vector2I max, out Vector2I result)
        {
				result.X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X;
				result.Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y;
        }

		/// <summary>Clamps the component values to within the given range.</summary>
        /// <param name="value">The <see cref="Vector2I"/> value to be clamped.</param>
        /// <param name="min">The minimum value of each component.</param>
        /// <param name="max">The maximum value of each component.</param>
        public static Vector2I Clamp(Vector2I value, Vector2I min, Vector2I max)
        {
			return new Vector2I()
			{
				X = value.X < min.X ? min.X : value.X > max.X ? max.X : value.X,
				Y = value.Y < min.Y ? min.Y : value.Y > max.Y ? max.Y : value.Y,
			};
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector2I CatmullRom(ref Vector2I value1, ref Vector2I value2, ref Vector2I value3, ref Vector2I value4, int amount)
        {
            float squared = amount * amount;
            float cubed = amount * squared;

            return new Vector2I()
            {
				X = (int)(0.5F * ((((2F * value2.X) + 
                ((-value1.X + value3.X) * amount)) + 
                (((((2F * value1.X) - (5F * value2.X)) + (4F * value3.X)) - value4.X) * squared)) +
                ((((-value1.X + (3F * value2.X)) - (3F * value3.X)) + value4.X) * cubed))),

				Y = (int)(0.5F * ((((2F * value2.Y) + 
                ((-value1.Y + value3.Y) * amount)) + 
                (((((2F * value1.Y) - (5F * value2.Y)) + (4F * value3.Y)) - value4.Y) * squared)) +
                ((((-value1.Y + (3F * value2.Y)) - (3F * value3.Y)) + value4.Y) * cubed))),

            };
        }

        /// <summary>
        /// Performs a Catmull-Rom interpolation using the specified positions.
        /// </summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <returns>A vector that is the result of the Catmull-Rom interpolation.</returns>
        public static Vector2I CatmullRom(Vector2I value1, Vector2I value2, Vector2I value3, Vector2I value4, int amount)
        {
            return CatmullRom(ref value1, ref value2, ref value3, ref value4, amount);
        }

        /// <summary>
        /// Returns the reflection of a vector off a surface that has the specified normal. 
        /// </summary>
        /// <param name="vector">The source vector.</param>
        /// <param name="normal">Normal of the surface.</param>
        /// <remarks>Reflect only gives the direction of a reflection off a surface, it does not determine 
        /// whether the original vector was close enough to the surface to hit it.</remarks>
        public static Vector2I Reflect(ref Vector2I vector, ref Vector2I normal)
        {
            int dot = (vector.X * normal.X) + (vector.Y * normal.Y);

            return new Vector2I()
            {
				X = (int)(vector.X - ((2.0F * dot) * normal.X)),
				Y = (int)(vector.Y - ((2.0F * dot) * normal.Y)),
            };
        }

        /// <summary>
        /// Converts the <see cref="Vector2I"/> into a unit vector.
        /// </summary>
        /// <param name="value">The <see cref="Vector2I"/> to normalize.</param>
        /// <returns>The normalized <see cref="Vector2I"/>.</returns>
        public static Vector2I Normalize(Vector2I value)
        {
            value.Normalize();
            return value;
        }
#endregion

#region Indexers
		/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X or Y component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component and so on.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 1].</exception>
        
		public int this[int index]
		{
			get
			{
				switch(index)
				{
					case 0: return X;
					case 1: return Y;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector2I run from 0 to 1, inclusive.");
			}

			set
			{
				switch(index)
				{
					case 0: X = value; break;
					case 1: Y = value; break;
				}
				throw new ArgumentOutOfRangeException("index", "Indices for Vector2I run from 0 to 1, inclusive.");
			}
		}
#endregion
	}
}

