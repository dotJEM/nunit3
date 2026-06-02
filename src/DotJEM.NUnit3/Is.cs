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
        public static JsonEqualsConstraint EqualTo(this IJsonConstraintsFactory self, JToken expected) => new(expected);

        public static JsonEqualsConstraint EqualTo(this IJsonConstraintsFactory self, string expected)
            => self.EqualTo(JToken.Parse(expected));

        public static JsonEqualsConstraint EqualTo(this IJsonConstraintsFactory self, object expected)
            => self.EqualTo(CoerceToJToken(expected));

        private static JToken CoerceToJToken(object value)
        {
            if (value is JToken token)
                return token;

            if(value is string str)
                return JToken.Parse(str);

            return JToken.FromObject(value);
        }

    }

    internal class JsonConstraintsFactory : IJsonConstraintsFactory { }

    public interface IJsonConstraintsFactory { }

}
