using Newtonsoft.Json.Linq;

namespace DotJEM.NUnit3.Constraints.Json
{
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


        private void QuickDiff(JToken actual, JToken expected, string path = "")
        {
            if (actual == null && expected == null)
                return;
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
    }
}