using DotJEM.NUnit3.Legacy.Constraints;
using DotJEM.NUnit3.Legacy.Extensions;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Legacy
{
    public static class JsonHas
    {
        public static IResolveConstraint Properties(object expected)
        {
            return new HasJsonPropertiesConstraint(expected.TryJObject());
        }
    }
}