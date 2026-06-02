using System.Collections.Generic;
using System.Reflection;
using DotJEM.NUnit3.Constraints.Objects;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Constraints
{
    public class ObjectPropertiesNotEqualsConstraint<T> : ObjectPropertiesEqualsConstraint<T>
    {
        private bool strict;

        public ObjectPropertiesNotEqualsConstraint(T expected) : base(expected)
        {
        }

        public ObjectPropertiesNotEqualsConstraint(T expected, HashSet<object> references) : base(expected, references)
        {
        }

        protected override Constraint SetupPrimitive(object expected)
        {
            return Is.Not.EqualTo(expected);
        }

        protected override IMatchResult Matches<T1>(T1 actual)
        {
            if (base.Matches(actual).Matches)
                return MatchResult.Fail("any property of the object to not be equal", "all was equal");

            if (!strict)
                return MatchResult.Success();

            PropertiesMatchResult result =  new PropertiesMatchResult();
            foreach (Property property in propertyMap.Values)
            {
                try
                {
                    property.Actual = property.Info.GetValue(actual, null);
                    property.Expected = property.Info.GetValue(Expected, null);
                    ConstraintResult pr = property.Apply();
                    if (pr.IsSuccess)
                        result.Failure(property.Info, new ConstraintResultMatchResult(pr));
                }
                catch (TargetException)
                {
                    result.Failure(property.Info, MatchResult.Fail(
                        expectedMessage: $"[{Expected.GetType().Name}] contained the property '{property.Info.Name}'.",
                        actualMessage: $"[{actual.GetType().Name}] did not."
                    ));
                }
            }
            return result;
        }


        /// <summary>
        /// Checks all properties and if any one of them matches the constraint fails.
        /// </summary>
        public ObjectPropertiesNotEqualsConstraint<T> Strict()
        {
            strict = true;
            return this;
        }
    }
}