using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace XBToolKit
{
    public static class FNamePool
    {
        private static readonly ConcurrentDictionary<ulong, string> s_NamePool = new();

        // FNV-1a 64位哈希算法
        private static ulong ComputeHash(string input)
        {
            const ulong fnvPrime = 1099511628211UL;
            const ulong fnvOffsetBasis = 14695981039346656037UL;
            ulong hash = fnvOffsetBasis;
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            foreach (byte b in bytes)
            {
                hash ^= b;
                hash *= fnvPrime;
            }
            return hash;
        }

        public static string GetDisplayStr(ulong comparisonIndex)
        {
            return s_NamePool.GetValueOrDefault(comparisonIndex);
        }

        public static FName GetFName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new FName() { comparisonIndex = 0 };
            }

            var comparisonIndex = ComputeHash(input);
            if (s_NamePool.TryGetValue(comparisonIndex, out var str))
            {
#if UNITY_EDITOR
                if (input != str)
                {
                    throw new Exception($"Hash collision: {str} and {input} have the same hash {comparisonIndex}");
                }
#endif
            }
            else
            {
                s_NamePool.TryAdd(comparisonIndex, input);
            }
            return new FName() { comparisonIndex = comparisonIndex };
        }
    }
}