namespace Molten.Math
{
	///<summary>A <see cref = "ushort"/> vector comprised of 4 components.</summary>
	public partial struct Half4U
	{
		///<summary>The X component.</summary>
		public ushort X;

		///<summary>The Y component.</summary>
		public ushort Y;

		///<summary>The Z component.</summary>
		public ushort Z;

		///<summary>The W component.</summary>
		public ushort W;

		///<summary>Creates a new instance of <see cref = "Half4U"/></summary>
		public Half4U(ushort x, ushort y, ushort z, ushort w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
	}
}

