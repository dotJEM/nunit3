﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotJEM.NUnit3.Constraints.Objects;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Legacy.Constraints
{
    /// <summary>
    /// Constraint for checking Enumerables inside Properties asserts.
    /// </summary>
    public class EnumerableEqualsConstraint : NUnit26ConstraintShim
    {
        private readonly IEnumerable<object> expected;

        private string expectedMessage;
        private string actualMessage;

        public EnumerableEqualsConstraint(IEnumerable expected)
        {
            if (expected != null)
                this.expected = expected.Cast<object>();
        }

        public override bool Matches(object actualObject)
        {
            if (ReferenceEquals(actualObject, expected))
                return true;

            //Note: If actual is null and we have passed the first if, we know that expected is not null.
            if (actualObject == null)
            {
                expectedMessage = "Collection with " + expected.Count() + " elements.";
                actualMessage = "<null>";
                return false;
            }

            IEnumerable<object> actualEnummerable = (actualObject as IEnumerable).Cast<object>();
            actual = actualEnummerable;

            //Note: If expected is null and we have passed the first if, we know that actual is not null.
            if (expected == null)
            {
                actualMessage = "Collection with " + actualEnummerable.Count() + " elements.";
                expectedMessage = "<null>";
                return false;
            }

            int actualCount = actualEnummerable.Count();
            int expectedCount = expected.Count();

            if (actualCount != expectedCount)
            {
                expectedMessage = "Collection with " + expected.Count() + " elements.";
                actualMessage = "Collection with " + actualEnummerable.Count() + " elements.";
                return false;
            }

            for (int i = 0; i < actualCount; i++)
            {
                object actualItem = actualEnummerable.ElementAt(i);
                object expectedItem = expected.ElementAt(i);
                if (ObjectHas.Properties.EqualTo(expectedItem).ApplyTo(actualItem).IsSuccess)
                    continue;

                expectedMessage = "Element at [" + i + "] should be: \"" + expectedItem + "\"";
                actualMessage = actualItem.ToString();
                return false;
            }
            return true;
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            writer.Write(" {0} ", expectedMessage);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            writer.WriteActualValue(actualMessage);
        }
    }
}