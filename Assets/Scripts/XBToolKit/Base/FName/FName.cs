namespace XBToolKit.FName
{
    using System;

    public struct FName : IEquatable<FName>
    {
        public ulong comparisonIndex;

        // 获取原始字符串（保留首次添加的大小写）
        public string String => FNamePool.GetDisplayStr(comparisonIndex);

        #region Equality and Operators

        public bool Equals(FName other) => comparisonIndex == other.comparisonIndex;
        public override bool Equals(object obj) => obj is FName other && Equals(other);
        public override int GetHashCode() => (int)(comparisonIndex ^ (comparisonIndex >> 32)); // 混合高低位

        public static bool operator ==(FName a, FName b) => a.comparisonIndex == b.comparisonIndex;
        public static bool operator !=(FName a, FName b) => a.comparisonIndex != b.comparisonIndex;

        #endregion

        public override string ToString() => String;

        // 隐式转换
        public static implicit operator FName(string name) => FNamePool.GetFName(name);
        public static implicit operator string(FName name) => name.String;
    }
}