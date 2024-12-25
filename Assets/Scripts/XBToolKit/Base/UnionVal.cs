using System.Runtime.InteropServices;

namespace XBToolKit
{
    /// <summary>
    /// 严禁作为key
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Union32Val
    {
        [FieldOffset(0)] public int intVal;
        [FieldOffset(0)] public float floatVal;

        public static implicit operator Union32Val(int val)
        {
            return new Union32Val() { intVal = val };
        }

        public static implicit operator Union32Val(float val)
        {
            return new Union32Val() { floatVal = val };
        }
    }

    /// <summary>
    /// 严禁作为key
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct Union64Val
    {
        [FieldOffset(0)] public long longVal;
        [FieldOffset(0)] public double doubleVal;

        public static implicit operator Union64Val(long val)
        {
            return new Union64Val() { longVal = val };
        }

        public static implicit operator Union64Val(double val)
        {
            return new Union64Val() { doubleVal = val };
        }
    }
}