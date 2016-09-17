using System.Runtime.InteropServices;

namespace Wildfire
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CVehicleWaterCannonEntity
    {
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 3)]
        public float[] Position;
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 0x24)]
        private byte[] Padding;
    }

     [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct CVehicleWaterCannonPool
    {
        [FieldOffset(0xC)]
        int ActiveTime;
        [FieldOffset(0x30)]
        [MarshalAs(UnmanagedType.LPArray, SizeConst = 32)]
        CVehicleWaterCannonEntity[] Entities;
    };
}
