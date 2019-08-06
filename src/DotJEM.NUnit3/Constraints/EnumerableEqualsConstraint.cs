using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotJEM.NUnit3.Constraints
{
    public class EnumerableEqualsConstraint : BaseConstraint
    {
        private readonly IList<object> expected;

        public EnumerableEqualsConstraint(IEnumerable expected)
        {
            this.expected = expected?.Cast<object>().ToList();
        }

        protected override MatchResult Matches<T>(T actual)
        {
            if (ReferenceEquals(actual, expected))
                return MatchResult.Success();

            //Note: If actual is null and we have passed the first if, we know that expected is not null.
            if (actual == null)
                return MatchResult.Fail($"IEnumerable with {expected.Count()} elements.", "<null>");

            IEnumerable<object> actualEnumerable = (actual as IEnumerable)?.Cast<object>().ToList();
            if(actualEnumerable == null)
                return MatchResult.Fail($"IEnumerable with {expected.Count()} elements.", $"type of {actual.GetType()} which is not an IEnumerable.");

            //Note: If expected is null and we have passed the first if, we know that actual is not null.
            if (expected == null)
                return MatchResult.Fail("<null>", $"IEnumerable with {actualEnumerable.Count()} elements.");

            int actualCount = actualEnumerable.Count();
            int expectedCount = expected.Count();

            if (actualCount != expectedCount)
                return MatchResult.Fail($"IEnumerable with {expectedCount} elements.", $"IEnumerable with {actualCount} elements.");

            for (int i = 0; i < actualCount; i++)
            {
                object actualItem = actualEnumerable.ElementAt(i);
                object expectedItem = expected.ElementAt(i);
                if (Has.Properties.EqualTo(expectedItem).ApplyTo(actualItem).IsSuccess)
                    continue;

                return MatchResult.Fail($"Element at [{i}] should be: \"{expectedItem}\"", actualItem.ToString());
            }
            return true;
        }
    }
}