﻿using System.Runtime.InteropServices;

namespace Molten.Windows32;

public static partial class Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MemoryStatusEx
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong TotalPhysical;
        public ulong AvailablePhysical;
        public ulong TotalPageFile;
        public ulong AvailablePageFile;
        public ulong TotalVirtual;
        public ulong AvailableVirtual;
        public ulong AvailableExtendedVirtual;

        public MemoryStatusEx()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(MemoryStatusEx));
        }
    }
}
