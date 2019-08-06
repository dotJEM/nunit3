using System.Collections.Generic;
using System.Reflection;
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

        protected override MatchResult Matches<T1>(T1 actual)
        {
            if (base.Matches(actual).Matches)
                return false;

            if (strict)
            {
                foreach (Property property in propertyMap.Values)
                {
                    try
                    {
                        property.Actual = property.Info.GetValue(actual, null);
                        property.Expected = property.Info.GetValue(Expected, null);
                        if (property.Apply().IsSuccess)
                            return false;
                    }
                    catch (TargetException)
                    {
                        return false;
                    }
                }
            }
            return true;
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