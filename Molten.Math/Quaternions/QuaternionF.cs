




using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Molten.Math
{
	///<summary>Represents a four dimensional mathematical QuaternionF.</summary>
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public partial struct QuaternionF : IFormattable
	{
		/// <summary>
        /// The size of the <see cref="QuaternionF"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(QuaternionF));

		 /// <summary>
        /// A <see cref="QuaternionF"/> with all of its components set to zero.
        /// </summary>
        public static readonly QuaternionF Zero = new QuaternionF();

        /// <summary>
        /// A <see cref="QuaternionF"/> with all of its components set to one.
        /// </summary>
        public static readonly QuaternionF One = new QuaternionF(1F, 1F, 1F, 1F);

        /// <summary>
        /// The identity <see cref="QuaternionF"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly QuaternionF Identity = new QuaternionF(0F, 0F, 0F, 0F);

		///<summary>The X component of the QuaternionF.</summary>
		public float X;

        ///<summary>The Y component of the QuaternionF.</summary>
		public float Y;

        ///<summary>The Z component of the QuaternionF.</summary>
		public float Z;

        ///<summary>The W component of the QuaternionF.</summary>
		public float W;

        /// <summary>
        /// Gets a value indicting whether this  <see cref="QuaternionF"/> is normalized.
        /// </summary>
        public bool IsNormalized
        {
            get { return MathHelper.IsOne((X * X) + (Y * Y) + (Z * Z) + (W * W)); }
        }

        /// <summary>
        /// Gets the angle of the  <see cref="QuaternionF"/>.
        /// </summary>
        /// <value>The quaternion's angle.</value>
        public float Angle
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (MathHelper.IsZero(length))
                    return 0.0F;

                return (float)(2.0 * Math.Acos(MathHelper.Clamp(W, -1F, 1F)));
            }
        }

        /// <summary>
        /// Gets the axis components of the QuaternionF.
        /// </summary>
        /// <value>The axis components of the QuaternionF.</value>
        public Vector3F Axis
        {
            get
            {
                float length = (X * X) + (Y * Y) + (Z * Z);
                if (MathHelper.IsZero(length))
                    return Vector3F.UnitX;

                float inv = 1.0F  / (float)Math.Sqrt(length);
                return new Vector3F(X * inv, Y * inv, Z * inv);
            }
        }

#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionF"/> struct.
        /// </summary>
        /// <param name="value">A QuaternionF containing the values with which to initialize the components.</param>
        public QuaternionF(Vector4F value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionF"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X, Y, and Z components.</param>
        /// <param name="w">Initial value for the W component of the <see cref="QuaternionF"/>.</param>
        public QuaternionF(Vector3F value, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionF"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
        /// <param name="z">Initial value for the Z component of the <see cref="QuaternionF"/>.</param>
        /// <param name="w">Initial value for the W component of the <see cref="QuaternionF"/>.</param>
        public QuaternionF(Vector2F value, float z, float w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionF"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the <see cref="QuaternionF"/>.</param>
        /// <param name="y">Initial value for the Y component of the <see cref="QuaternionF"/>.</param>
        /// <param name="z">Initial value for the Z component of the <see cref="QuaternionF"/>.</param>
        /// <param name="w">Initial value for the W component of the <see cref="QuaternionF"/>.</param>
        public QuaternionF(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        
        /// <summary>
        /// Initializes a new instance of the  <see cref="QuaternionF"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z, and W components of the quaternion. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public QuaternionF(float[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for QuaternionF.");

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionF"/> struct from an unsafe pointer. The pointer should point to an array of four elements.
        /// </summary>
        public unsafe QuaternionF(float* ptr)
		{
            X = ptr[0];
            Y = ptr[1];
            Z = ptr[2];
            W = ptr[3];
		}
#endregion

#region Instance Methods
/// <summary>
        /// Gets a value indicating whether this instance is equivalent to the identity QuaternionF.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity QuaternionF; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get { return this.Equals(Identity); }
        }

        /// <summary>
        /// Conjugates the <see cref="QuaternionF"/> X,Y and Z components. W component is not changed.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Conjugates the <see cref="QuaternionF"/>.
        /// </summary>
        public void Abs()
        {
            X = Math.Abs(X);
            Y = Math.Abs(Y);
            Z = Math.Abs(Z);
            W = Math.Abs(W);
        }

        /// <summary>
        /// Calculates the squared length of the <see cref="QuaternionF"/>.
        /// </summary>
        /// <returns>The squared length of the <see cref="QuaternionF"/>.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="QuaternionF.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public float LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        /// <summary>
        /// Creates an array containing the elements of the <see cref="QuaternionF"/>.
        /// </summary>
        /// <returns>A four-element array containing the components of the <see cref="QuaternionF"/>.</returns>
        public float[] ToArray()
        {
            return new float[] { X, Y, Z, W };
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
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Computes the quaternion rotation between two normalized vectors.
        /// </summary>
        /// <param name="v1">First unit-length vector.</param>
        /// <param name="v2">Second unit-length vector.</param>
        /// <param name="q">Quaternion representing the rotation from v1 to v2.</param>
        public static QuaternionF GetQuaternionBetweenNormalizedVectors(ref Vector3F v1, ref Vector3F v2)
        {
            float dot;
            QuaternionF q;
            Vector3F.Dot(ref v1, ref v2, out dot);
            //For non-normal vectors, the multiplying the axes length squared would be necessary:
            //float w = dot + (float)Math.Sqrt(v1.LengthSquared() * v2.LengthSquared());
            if (dot < -0.9999F) //parallel, opposing direction
            {
                //If this occurs, the rotation required is ~180 degrees.
                //The problem is that we could choose any perpendicular axis for the rotation. It's not uniquely defined.
                //The solution is to pick an arbitrary perpendicular axis.
                //Project onto the plane which has the lowest component magnitude.
                //On that 2d plane, perform a 90 degree rotation.
                float absX = Math.Abs(v1.X);
                float absY = Math.Abs(v1.Y);
                float absZ = Math.Abs(v1.Z);
                if (absX < absY && absX < absZ)
                    q = new QuaternionF(0, -v1.Z, v1.Y, 0);
                else if (absY < absZ)
                    q = new QuaternionF(-v1.Z, 0, v1.X, 0);
                else
                    q = new QuaternionF(-v1.Y, v1.X, 0, 0);
            }
            else
            {
                Vector3F axis;
                Vector3F.Cross(ref v1, ref v2, out axis);
                q = new QuaternionF(axis.X, axis.Y, axis.Z, dot + 1);
            }
            q.Normalize();

            return q;
        }

        /// <summary>
        /// Converts the <see cref="QuaternionF"/> into a unit quaternion.
        /// </summary>
        public void Normalize()
        {
            float length = Length();
            if (!MathHelper.IsZero(length))
            {
                float inverse = 1.0F / length;
                X *= inverse;
                Y *= inverse;
                Z *= inverse;
                W *= inverse;
            }
        }

        /// <summary>
        /// Calculates the length of the QuaternionF.
        /// </summary>
        /// <returns>The length of the QuaternionF.</returns>
        /// <remarks>
        /// <see cref="QuaternionF.LengthSquared"/> may be preferred when only the relative length is needed and speed is of the essence.
        /// </remarks>
        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Conjugates and renormalizes the QuaternionF.
        /// </summary>
        public void Invert()
        {
            float lengthSq = LengthSquared();
            if (!MathHelper.IsZero(lengthSq))
            {
                lengthSq = 1.0F / lengthSq;

                X = -X * lengthSq;
                Y = -Y * lengthSq;
                Z = -Z * lengthSq;
                W = W * lengthSq;
            }
        }
#endregion

#region To-String Methods
/// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }
#endregion

#region Static Methods
        /// <summary>
        /// Multiplies two <see cref="QuaternionF"/> together in opposite order.
        /// </summary>
        /// <param name="a">First <see cref="QuaternionF"/> to multiply.</param>
        /// <param name="b">Second <see cref="QuaternionF"/> to multiply.</param>
        public static QuaternionF Concatenate(ref QuaternionF a, ref QuaternionF b)
        {
            float aX = a.X;
            float aY = a.Y;
            float aZ = a.Z;
            float aW = a.W;
            float bX = b.X;
            float bY = b.Y;
            float bZ = b.Z;
            float bW = b.W;

            return new QuaternionF()
            {
                X = aW * bX + aX * bW + aZ * bY - aY * bZ,
                Y = aW * bY + aY * bW + aX * bZ - aZ * bX,
                Z = aW * bZ + aZ * bW + aY * bX - aX * bY,
                W = aW * bW - aX * bX - aY * bY - aZ * bZ
            };
        }
#endregion

#region Operators - Multiply
        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static QuaternionF operator *(float scale, QuaternionF value)
        {
            return new QuaternionF()
            {
                X = value.X * scale,
                Y = value.Y * scale,
                Z = value.Z * scale,
                W = value.W * scale,
            };
        }

        /// <summary>
        /// Scales a quaternion by the given value.
        /// </summary>
        /// <param name="value">The quaternion to scale.</param>
        /// <param name="scale">The amount by which to scale the quaternion.</param>
        /// <returns>The scaled quaternion.</returns>
        public static QuaternionF operator *(QuaternionF value, float scale)
        {
            return new QuaternionF()
            {
                X = value.X * scale,
                Y = value.Y * scale,
                Z = value.Z * scale,
                W = value.W * scale,
            };
        }

        /// <summary>
        /// Multiplies a quaternion by another.
        /// </summary>
        /// <param name="left">The first quaternion to multiply.</param>
        /// <param name="right">The second quaternion to multiply.</param>
        /// <returns>The multiplied quaternion.</returns>
        public static QuaternionF operator *(QuaternionF left, QuaternionF right)
        {
            float lx = left.X;
            float ly = left.Y;
            float lz = left.Z;
            float lw = left.W;
            float rx = right.X;
            float ry = right.Y;
            float rz = right.Z;
            float rw = right.W;
            float a = (ly * rz - lz * ry);
            float b = (lz * rx - lx * rz);
            float c = (lx * ry - ly * rx);
            float d = (lx * rx + ly * ry + lz * rz);

            return new QuaternionF()
            {
                X = (lx * rw + rx * lw) + a,
                Y = (ly * rw + ry * lw) + b,
                Z = (lz * rw + rz * lw) + c,
                W = lw * rw - d
            };
        }
#endregion

#region Operators - Subtract
        /// <summary>
        /// Subtracts two quaternions.
        /// </summary>
        /// <param name="left">The first quaternion to subtract.</param>
        /// <param name="right">The second quaternion to subtract.</param>
        /// <returns>The difference of the two quaternions.</returns>
        public static QuaternionF operator -(QuaternionF left, QuaternionF right)
        {
            return new QuaternionF()
            {
            X = left.X - right.X,
            Y = left.Y - right.Y,
            Z = left.Z - right.Z,
            W = left.W - right.W
            };
        }

        /// <summary>
        /// Reverses the direction of a given quaternion.
        /// </summary>
        /// <param name="value">The quaternion to negate.</param>
        /// <returns>A quaternion facing in the opposite direction.</returns>
        public static QuaternionF operator -(QuaternionF value)
        {
            return new QuaternionF()
            {
                X = -value.X,
                Y = -value.Y,
                Z = -value.Z,
                W = -value.W
            };
        }
#endregion

#region Indexers
/// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for QuaternionF run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for QuaternionF run from 0 to 3, inclusive.");
                }
            }
        }
#endregion
	}
}

