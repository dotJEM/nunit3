using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotJEM.NUnit3.Constraints.Objects;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Constraints
{
    public class EnumerableEqualsConstraint(IEnumerable expected) : BaseConstraint
    {
        private readonly IList<object> expectedItems = expected?.Cast<object>().ToList();

        protected override IMatchResult Matches<T>(T actual)
        {
            if (ReferenceEquals(actual, expectedItems))
                return MatchResult.Success();

            //Note: If actual is null and we have passed the first if, we know that expected is not null.
            if (actual == null)
                return MatchResult.Fail($"IEnumerable with {expectedItems.Count} elements.", "<null>");

            List<object> actualEnumerable = (actual as IEnumerable)?.Cast<object>().ToList();
            if(actualEnumerable == null)
                return MatchResult.Fail($"IEnumerable with {expectedItems.Count} elements.", $"type of '{actual.GetType()}' which is not an IEnumerable.");

            //Note: If expected is null and we have passed the first if, we know that actual is not null.
            if (expectedItems == null)
                return MatchResult.Fail("<null>", $"IEnumerable with {actualEnumerable.Count()} elements.");

            int actualCount = actualEnumerable.Count;
            int expectedCount = expectedItems.Count;

            if (actualCount != expectedCount)
                return MatchResult.Fail($"IEnumerable with {expectedCount} elements.", $"IEnumerable with {actualCount} elements.");

            CollectionMatchResult result = new ();
            for (int i = 0; i < actualCount; i++)
            {
                object actualItem = actualEnumerable[i];
                object expectedItem = expectedItems[i];
                ConstraintResult cr = Has.Properties.EqualTo(expectedItem).ApplyTo(actualItem);
                if (cr.IsSuccess)
                    continue;

                result.Failure(i, new ConstraintResultMatchResult(cr));
                //return MatchResult.Fail(
                //    expectedMessage: $"element at [{i}] to be: {expectedItem}", 
                //    actualMessage: $"{actualItem}");
            }


            return result; //MatchResult.Success();
        }
    }
}