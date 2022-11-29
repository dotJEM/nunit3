using DotJEM.NUnit3.Legacy.Constraints;
using DotJEM.NUnit3.Legacy.Extensions;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Legacy
{
    public static class JsonIs
    {
        public static IResolveConstraint EqualTo(object expected)
        {
            return new JsonEqualsConstraint(expected.TryJToken());
        }
    }
}