using System.Runtime.InteropServices;

namespace Molten.Math
{
	///<summary>A <see cref = "float"/> vector comprised of 4 components.</summary>
	public partial struct Vector4F
	{
		/// <summary>
        /// Saturates this instance in the range [0,1]
        /// </summary>
        public void Saturate()
        {
			X = X < 0F ? 0F : X > 1F ? 1F : X;
			Y = Y < 0F ? 0F : Y > 1F ? 1F : Y;
			Z = Z < 0F ? 0F : Z > 1F ? 1F : Z;
			W = W < 0F ? 0F : W > 1F ? 1F : W;
        }

		/// <summary>Rounds all components down to the nearest unit.</summary>
        public void Floor()
        {
			X = (float)Math.Floor(X);
			Y = (float)Math.Floor(Y);
			Z = (float)Math.Floor(Z);
			W = (float)Math.Floor(W);
        }

        /// <summary>Rounds all components up to the nearest unit.</summary>
        public void Ceiling()
        {
			X = (float)Math.Ceiling(X);
			Y = (float)Math.Ceiling(Y);
			Z = (float)Math.Ceiling(Z);
			W = (float)Math.Ceiling(W);
        }
	}
}

