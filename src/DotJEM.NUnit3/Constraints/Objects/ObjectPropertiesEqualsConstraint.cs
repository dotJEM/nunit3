using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DotJEM.NUnit3.Util;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace DotJEM.NUnit3.Constraints
{
    public class ObjectPropertiesEqualsConstraint<T> : BaseConstraint
    {
        protected Constraint primitive;
        protected readonly Dictionary<string, Property> propertyMap = new Dictionary<string, Property>();

        protected T Expected { get; }
        protected bool ExplicitTypesFlag { get; set; }

        private readonly HashSet<object> references = new HashSet<object>(/*new ReferenceComparer()*/);

        public ObjectPropertiesEqualsConstraint(T expected)
        {
            Expected = expected;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            InitializeProperties();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public ObjectPropertiesEqualsConstraint(T expected, HashSet<object> references)
        {
            Expected = expected;
            this.references = references;

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            InitializeProperties();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        protected virtual void InitializeProperties()
        {
            if (ReferenceEquals(Expected, null))
            {
                primitive = SetupPrimitive(Expected);
                return;
            }

            Type type = Expected.GetType();
            PropertyInfo[] properties = type.GetProperties();

            if (properties.Length == 0)
                primitive = SetupPrimitive(Expected);

            //Note: No need for "Else", foreach automatically terminates on an empty collection.
            foreach (PropertyInfo property in properties.Where(property => property.GetIndexParameters().Length == 0))
            {
                object expectedObject = property.GetValue(Expected, null);
                if (references.Contains(expectedObject))
                    continue;

                references.Add(expectedObject);
                SetupProperty(property, property.GetValue(Expected, null));
            }
        }

        protected virtual Constraint SetupPrimitive(object expected)
        {
            return Is.EqualTo(expected);
        }

        protected void SetupProperty(Expression<Func<T, object>> property)
        {
            PropertyInfo propertyInfo = property.GetPropertyInfo();
            SetupProperty(propertyInfo, propertyInfo.GetValue(Expected, null));
        }

        protected void SetupProperty(Expression<Func<T, object>> property, Constraint constraint)
        {
            SetupProperty(property.GetPropertyInfo(), constraint);
        }

        protected void SetupProperty<T2>(Expression<Func<T, T2>> property, Func<T2, Constraint> constraintFactory)
        {
            SetupProperty(property.GetPropertyInfo(), constraintFactory);
        }

        private void SetupProperty(PropertyInfo property, Constraint constraint)
        {
            propertyMap[property.Name] = new Property { Info = property, Constraint = constraint };
        }

        private void SetupProperty(PropertyInfo property, object expected)
        {
            Type type = property.PropertyType;
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            {
                SetupProperty(property, Is.EqualTo(expected));
            }
            else if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
            {
                SetupProperty(property, new EnumerableEqualsConstraint((IEnumerable)expected));
            }
            else
            {
                SetupProperty(property, new ObjectPropertiesEqualsConstraint<object>(expected, references));
            }
        }

        private void SetupConstraint(PropertyInfo property, Constraint constraint)
        {
            if (!propertyMap.ContainsKey(property.Name))
                throw new ArgumentException("Property was not valid, check that it has not been removed using 'Ignore'.", nameof(property));

            propertyMap[property.Name].Constraint = constraint;
        }


        protected override IMatchResult Matches<T1>(T1 actual)
        {
            if (ExplicitTypesFlag && ReferenceEquals(actual.GetType(), Expected.GetType()) == false)
                return MatchResult.Fail($"was of type {Expected.GetType()} (ExplicitTypesFlag was set)", $"was of type {actual.GetType()}");

            if (primitive != null)
                return new ConstraintResultMatchResult(primitive.ApplyTo(actual));

            PropertiesMatchResult result =  new PropertiesMatchResult();
            foreach (Property property in propertyMap.Values)
            {
                try
                {
                    property.Actual = property.Info.GetValue(actual, null);
                    property.Expected = property.Info.GetValue(Expected, null);
                    ConstraintResult pr = property.Apply();
                    if (!pr.IsSuccess)
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

        protected class Property
        {
            public PropertyInfo Info { get; set; }
            public object Expected { get; set; }
            public object Actual { get; set; }
            public Constraint Constraint { get; set; }

            public ConstraintResult Apply()
            {
                return Constraint.ApplyTo(Actual);
            }
        }

        public class ObjectPropertyEqualsModifyer<TProperty>
        {
            private readonly PropertyInfo[] properties;
            private readonly ObjectPropertiesEqualsConstraint<T> parrent;

            internal ObjectPropertyEqualsModifyer(ObjectPropertiesEqualsConstraint<T> parrent, PropertyInfo[] properties)
            {
                this.properties = properties;
                this.parrent = parrent;
            }

            /// <summary>
            /// Uses the specified Comparison to compare the specified property or properties.
            /// </summary>
            public ObjectPropertiesEqualsConstraint<T> Use(Comparison<TProperty> comparison)
            {
                foreach (PropertyInfo property in properties)
                    SetupProperty(property, Is.EqualTo(property.GetValue(parrent.Expected, null)).Using(comparison));
                return parrent;
            }

            /// <summary>
            /// Uses the specified Constraint to compare the specified property or properties.
            /// </summary>
            public ObjectPropertiesEqualsConstraint<T> Use(Func<TProperty, Constraint> constraintFactory)
            {
                foreach (PropertyInfo property in properties)
                    SetupProperty(property, constraintFactory((TProperty)property.GetValue(parrent.Expected, null)));
                return parrent;
            }

            /// <summary>
            /// Uses the specified Constraint to compare the specified property or properties.
            /// </summary>
            public ObjectPropertiesEqualsConstraint<T> Use<TConstraint>(Func<TProperty, TConstraint> constraintFactory, Func<TConstraint, TConstraint> modifyer) where TConstraint : Constraint
            {
                foreach (PropertyInfo property in properties)
                {
                    SetupProperty(property, modifyer(constraintFactory((TProperty)property.GetValue(parrent.Expected, null))));
                }
                return parrent;
            }

            /// <summary>
            /// Uses the specified Constraint to compare the specified property or properties.
            /// </summary>
            public ObjectPropertiesEqualsConstraint<T> Use(Constraint constraint)
            {
                foreach (PropertyInfo property in properties)
                    SetupProperty(property, constraint);
                return parrent;
            }

            private void SetupProperty(PropertyInfo property, Constraint constraint)
            {
                parrent.SetupConstraint(property, constraint);
            }
        }
    }

    public class ConstraintResultMatchResult : IMatchResult
    {
        private readonly ConstraintResult constraintResult;

        public bool Matches => constraintResult.IsSuccess;

        public ConstraintResultMatchResult(ConstraintResult constraintResult)
        {
            this.constraintResult = constraintResult;
        }

        public void WriteTo(MessageWriter writer) => constraintResult.WriteMessageTo(writer);
    }

    public class PropertiesMatchResult : IMatchResult
    {
        private readonly List<(PropertyInfo, IMatchResult)> failures = new List<(PropertyInfo, IMatchResult)>();

        public bool Matches => !failures.Any();

        public void WriteTo(MessageWriter writer)
        {
            writer.WriteLine("Properties of the object did not match.");
            using (MessageWriter w = new TextMessageWriter())
            {
                foreach ((PropertyInfo property, IMatchResult result) in failures)
                {
                    w.WriteLine($"The property '{property.Name} <{property.PropertyType}>' was not equals.");
                    result.WriteTo(w);
                    w.WriteLine();
                }

                using (StringReader reader = new StringReader(w.ToString()))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        writer.WriteLine($"  {line}");
                }
            }

        }

        public void Failure(PropertyInfo property, IMatchResult pr)
        {
            failures.Add((property, pr));
        }
    }
}