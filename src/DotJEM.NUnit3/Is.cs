using DotJEM.NUnit3.Constraints.Json;
using Newtonsoft.Json.Linq;
using NUnitIs = NUnit.Framework.Is;
using NUnitHas = NUnit.Framework.Has;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3
{
    public class Is : NUnitIs
    {
        public static IJsonConstraintsFactory Json { get; } = new JsonConstraintsFactory();
    }

    public static class JsonConstraintsFactoryExtensions
    {
        public static JsonEqualsConstraint EqualTo(this IJsonConstraintsFactory self, JToken expected)
        {
            return new JsonEqualsConstraint(expected);
        }

    }

    internal class JsonConstraintsFactory : IJsonConstraintsFactory { }

    public interface IJsonConstraintsFactory { }

    //public static class Assert
    //{
    //    public static void That<T>(IConstraintAndValue expression)
    //    {

    //    }
    //}

    //public static class IsAsExtention
    //{
    //    public static IConstraintBuilder Is(this object value)
    //    {
    //        return new ConstraintBuilder(value);
    //    }
    //}

    //public interface IConstraintBuilder
    //{
    //}

    //class ConstraintBuilder : IConstraintBuilder
    //{
    //    public object Value { get; }

    //    public ConstraintBuilder(object value)
    //    {
    //        Value = value;
    //    }
    //}

    //public static class ConstraintBuilderStandardIs
    //{
    //    public static void EqualTo(this IConstraintBuilder self, object value)
    //    {
    //        return Is.EqualTo(value)
    //    }
    //}

    //public interface IConstraintAndValue
    //{

    //}

    //public class test
    //{
    //    public void Test()
    //    {
    //        Assert.That(42.Is().EqualTo(42));
    //    }
    //}
}
