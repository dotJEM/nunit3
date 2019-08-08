using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DotJEM.NUnit3.Constraints.Json
{
    public class JsonEqualsConstraint : BaseConstraint
    {
        private readonly JToken expected;

        public JsonEqualsConstraint(JToken expected)
        {
            this.expected = expected;
        }

        protected override IMatchResult Matches<T>(T actualValue)
        {
            if (ReferenceEquals(actualValue, expected))
                return MatchResult.Success();

            JToken actual = actualValue as JToken;
            if (actual == null)
                return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");
            
            if (JToken.DeepEquals(actual, expected))
                return MatchResult.Success();

            return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");

        }


        private IEnumerable<string> UnionKeys(IDictionary<string, JToken> update, IDictionary<string, JToken> other)
        {
            HashSet<string> keys = new HashSet<string>(update.Keys);
            keys.UnionWith(other.Keys);
            return keys;
        }
    }

    public class HasJsonPropertiesConstraint : BaseConstraint
    {
        private readonly JToken expected;
        private IJArrayStrategy arrayStrategy = new StrictJArrayStrategy();

        public HasJsonPropertiesConstraint(JToken expected)
        {
            this.expected = expected;
        }

        protected override IMatchResult Matches<T>(T actualValue)
        {
            if (ReferenceEquals(actualValue, expected))
                return MatchResult.Success();

            JToken actual = actualValue as JToken;
            if (actual == null)
                return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");


            return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");
        }







        public HasJsonPropertiesConstraint AllowArrayOutOfOrder()
            => SetArrayStrategy<OutOfOrderJArrayStrategy>();
 
        public HasJsonPropertiesConstraint StrictArrayOrder()
            => SetArrayStrategy<StrictJArrayStrategy>();

        private HasJsonPropertiesConstraint SetArrayStrategy<TStrategy>() where TStrategy : IJArrayStrategy, new()
        {
            arrayStrategy = new TStrategy();
            return this;
        }

        private interface IJArrayStrategy
        {
            
        }

        private class StrictJArrayStrategy : IJArrayStrategy
        {
        }

        private class OutOfOrderJArrayStrategy : IJArrayStrategy
        {
        }
    }
}
