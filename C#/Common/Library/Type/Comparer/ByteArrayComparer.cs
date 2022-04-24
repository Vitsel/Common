using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Common.Library.Type.Comparer
{
    public class ByteArrayComparer : EqualityComparer<byte[]>
    {
        public override bool Equals(byte[] x, byte[] y)
        {
            if (x == null && y == null)
                return true;

            if (ReferenceEquals(x, y))
                return true;

            if (x.Length != y.Length)
                return false;

            return x.SequenceEqual(y);
        }

        public override int GetHashCode(byte[] obj)
        {
            return new BigInteger(obj).GetHashCode();
        }
    }
}