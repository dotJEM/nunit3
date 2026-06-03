using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DotJEM.NUnit3.Util
{
    public class ReferenceComparer : IEqualityComparer<object>
    {
        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}