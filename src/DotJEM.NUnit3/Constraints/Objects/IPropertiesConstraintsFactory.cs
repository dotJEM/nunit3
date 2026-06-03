namespace DotJEM.NUnit3.Constraints.Objects
{
    public interface IPropertiesConstraintsFactory
    {
    }

    //NOTE: (jmd 2019-08-14) Just so we don't return null.
    internal class PropertiesConstraintsFactory : IPropertiesConstraintsFactory
    {
    }

    public static class PropertiesConstraintsFactoryExtensions
    {
        public static ObjectPropertiesEqualsConstraint<T> EqualTo<T>(this IPropertiesConstraintsFactory self, T expected, bool includeNonPublic = false)
        {
            return new ObjectPropertiesEqualsConstraint<T>(expected, includeNonPublic);
        }

        public static ObjectPropertiesNotEqualsConstraint<T> NotEqualTo<T>(this IPropertiesConstraintsFactory self, T expected, bool includeNonPublic = false)
        {
            return new ObjectPropertiesNotEqualsConstraint<T>(expected, includeNonPublic);
        }
    }
}