using System;
using DotJEM.NUnit3.Constraints;
using DotJEM.NUnit3.Constraints.Objects;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Constraints.Objects
{
    public class ObjectPropertiesEqualsConstraintSimpleScenariosTest
    {
        [Test]
        public void ApplyTo_EquivalentSimpleObjects_Passes()
        {
            var constraint = Has.Properties.EqualTo(new SimplePerson { Name = "Jane", Age = 42 });
            ConstraintResult result = constraint.ApplyTo(new SimplePerson { Name = "Jane", Age = 42 });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentSimpleObjects_FailsWithPropertyDetails()
        {
            var constraint = Has.Properties.EqualTo(new SimplePerson { Name = "Jane", Age = 42 });
            ConstraintResult result = constraint.ApplyTo(new SimplePerson { Name = "Jane", Age = 41 });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("The property 'Age <System.Int32>' was not equals."));
        }

        [Test]
        public void ApplyTo_EquivalentNestedObjectsWithCollections_Passes()
        {
            var constraint = Has.Properties.EqualTo(new ComplexPerson
            {
                Name = "Jane",
                Address = new Address { Street = "Main Street", ZipCode = 2800 },
                LuckyNumbers = new[] { 4, 8, 15, 16, 23, 42 }
            });

            ConstraintResult result = constraint.ApplyTo(new ComplexPerson
            {
                Name = "Jane",
                Address = new Address { Street = "Main Street", ZipCode = 2800 },
                LuckyNumbers = new[] { 4, 8, 15, 16, 23, 42 }
            });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentNestedObjects_FailsWhenNestedPropertyDiffers()
        {
            var constraint = Has.Properties.EqualTo(new ComplexPerson
            {
                Name = "Jane",
                Address = new Address { Street = "Main Street", ZipCode = 2800 },
                LuckyNumbers = new[] { 4, 8, 15, 16, 23, 42 }
            });

            ConstraintResult result = constraint.ApplyTo(new ComplexPerson
            {
                Name = "Jane",
                Address = new Address { Street = "Side Street", ZipCode = 2800 },
                LuckyNumbers = new[] { 4, 8, 15, 16, 23, 42 }
            });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("The property 'Address <DotJEM.NUnit3.Tests.Constraints.Objects.Address>' was not equals."));
            Assert.That(result.ToString(), Does.Contain("The property 'Street <System.String>' was not equals."));
        }

        [Test]
        public void ApplyTo_ActualObjectMissingExpectedProperty_Fails()
        {
            var constraint = Has.Properties.EqualTo(new PersonWithAge { Name = "Jane", Age = 42 });
            ConstraintResult result = constraint.ApplyTo(new PersonWithoutAge { Name = "Jane" });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("The property 'Age <System.Int32>' was not equals."));
            Assert.That(result.ToString(), Does.Contain("[PersonWithAge] contained the property 'Age'."));
            Assert.That(result.ToString(), Does.Contain("[PersonWithoutAge] did not."));
        }

        [Test]
        public void ApplyTo_WhenExpectedAndActualAreNull_Passes()
        {
            var constraint = Has.Properties.EqualTo<string>(null);
            string actual = null;
            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }
    }

    public class ObjectPropertiesEqualsConstraintConfigurationScenariosTest
    {
        [Test]
        public void Ignore_WhenOnlyIgnoredPropertyDiffers_Passes()
        {
            var constraint = Has.Properties.EqualTo(new SimplePerson { Name = "Jane", Age = 42 })
                .Ignore(x => x.Age);

            ConstraintResult result = constraint.ApplyTo(new SimplePerson { Name = "Jane", Age = 41 });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void For_WhenUsingCustomComparison_AppliesOnlyToSelectedProperty()
        {
            var constraint = Has.Properties.EqualTo(new UserProfile { Name = "Jane", Email = "jane@example.com" })
                .For(x => x.Email)
                .Use((left, right) => string.Compare(left, right, StringComparison.OrdinalIgnoreCase));

            ConstraintResult result = constraint.ApplyTo(new UserProfile { Name = "Jane", Email = "JANE@EXAMPLE.COM" });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ForAll_WhenUsingCustomConstraint_AppliesToAllSelectedProperties()
        {
            var constraint = Has.Properties.EqualTo(new TextPair { First = "Alpha", Second = "Beta" })
                .ForAll
                .Use(value => value is string text ? Is.EqualTo(text).IgnoreCase : Is.EqualTo(value));

            ConstraintResult result = constraint.ApplyTo(new TextPair { First = "ALPHA", Second = "beta" });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void CheckTypes_WhenRootTypeDiffers_Fails()
        {
            var constraint = Has.Properties.EqualTo(new ShapeHolder { Shape = new ShapeBase { Name = "Circle" } })
                .CheckTypes();

            ConstraintResult result = constraint.ApplyTo(new DerivedShapeHolder { Shape = new ShapeBase { Name = "Circle" } });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("ExplicitTypesFlag was set"));
            Assert.That(result.ToString(), Does.Contain(nameof(ShapeHolder)));
            Assert.That(result.ToString(), Does.Contain(nameof(DerivedShapeHolder)));
        }

        [Test]
        public void CheckTypes_WhenNestedTypeDiffers_Fails()
        {
            var constraint = Has.Properties.EqualTo(new ShapeHolder { Shape = new ShapeBase { Name = "Circle" } })
                .CheckTypes();

            ConstraintResult result = constraint.ApplyTo(new ShapeHolder { Shape = new DerivedShape { Name = "Circle", Sides = 0 } });

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("The property 'Shape <DotJEM.NUnit3.Tests.Constraints.Objects.ShapeBase>' was not equals."));
            Assert.That(result.ToString(), Does.Contain("ExplicitTypesFlag was set"));
            Assert.That(result.ToString(), Does.Contain(nameof(ShapeBase)));
            Assert.That(result.ToString(), Does.Contain(nameof(DerivedShape)));
        }
    }

    public class ObjectPropertiesEqualsConstraintCircularReferenceScenariosTest
    {
        [Test]
        public void ApplyTo_EquivalentSelfReferencingObjects_Passes()
        {
            CircularNode expected = CreateSelfReferencingNode("root");
            var constraint = Has.Properties.EqualTo(expected);
            CircularNode actual = CreateSelfReferencingNode("root");

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ApplyTo_CircularGraphsWithNestedDifference_Fails()
        {
            CircularNode expected = CreateTwoNodeCycle("root", "child");
            var constraint = Has.Properties.EqualTo(expected);
            CircularNode actual = CreateTwoNodeCycle("root", "different-child");

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.ToString(), Does.Contain("The property 'Next <DotJEM.NUnit3.Tests.Constraints.Objects.CircularNode>' was not equals."));
            Assert.That(result.ToString(), Does.Contain("The property 'Name <System.String>' was not equals."));
        }

        private static CircularNode CreateSelfReferencingNode(string name)
        {
            CircularNode node = new CircularNode { Name = name };
            node.Next = node;
            return node;
        }

        private static CircularNode CreateTwoNodeCycle(string rootName, string childName)
        {
            CircularNode root = new CircularNode { Name = rootName };
            CircularNode child = new CircularNode { Name = childName };
            root.Next = child;
            child.Next = root;
            return root;
        }
    }

    public class ObjectPropertiesEqualsConstraintSharedReferenceScenariosTest
    {
        [Test]
        public void ApplyTo_SharedExpectedReference_FailsWhenActualUsesDistinctInstances()
        {
            // expected.Home and expected.Work point to the exact same Address instance
            Address sharedAddress = new Address { Street = "Main Street", ZipCode = 2800 };
            PersonWithTwoAddresses expected = new PersonWithTwoAddresses { Home = sharedAddress, Work = sharedAddress };
            var constraint = Has.Properties.EqualTo(expected);

            // actual.Home and actual.Work are different objects, even though their values match
            PersonWithTwoAddresses actual = new PersonWithTwoAddresses
            {
                Home = new Address { Street = "Main Street", ZipCode = 2800 },
                Work = new Address { Street = "Main Street", ZipCode = 2800 }
            };

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.False,
                "Work should fail because expected.Work is the same reference as expected.Home, " +
                "but actual.Work is a distinct object from actual.Home.");
        }

        [Test]
        public void ApplyTo_SharedExpectedReference_PassesWhenActualAlsoSharesTheSameReference()
        {
            Address sharedAddress = new Address { Street = "Main Street", ZipCode = 2800 };
            PersonWithTwoAddresses expected = new PersonWithTwoAddresses { Home = sharedAddress, Work = sharedAddress };
            var constraint = Has.Properties.EqualTo(expected);

            Address sharedActualAddress = new Address { Street = "Main Street", ZipCode = 2800 };
            PersonWithTwoAddresses actual = new PersonWithTwoAddresses
            {
                Home = sharedActualAddress,
                Work = sharedActualAddress
            };

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ApplyTo_DistinctExpectedReferences_PassesRegardlessOfActualSharing()
        {
            // expected uses two separate but value-equal Address instances (no shared reference)
            PersonWithTwoAddresses expected = new PersonWithTwoAddresses
            {
                Home = new Address { Street = "Main Street", ZipCode = 2800 },
                Work = new Address { Street = "Main Street", ZipCode = 2800 }
            };
            var constraint = Has.Properties.EqualTo(expected);

            // actual happens to share a single Address instance - still passes because
            // expected doesn't require reference equality between Home and Work
            Address sharedActualAddress = new Address { Street = "Main Street", ZipCode = 2800 };
            PersonWithTwoAddresses actual = new PersonWithTwoAddresses
            {
                Home = sharedActualAddress,
                Work = sharedActualAddress
            };

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void ApplyTo_SelfReferencingNodeVsNonCyclicNode_Fails()
        {
            CircularNode expected = new CircularNode { Name = "root" };
            expected.Next = expected;
            var constraint = Has.Properties.EqualTo(expected);

            // actual.Next points to a different node, not itself
            CircularNode actual = new CircularNode { Name = "root" };
            actual.Next = new CircularNode { Name = "root", Next = null };

            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.False,
                "Should fail because expected has a self-reference but actual does not.");
        }
    }

    public class ObjectPropertiesEqualsConstraintDiagnosticTest
    {
        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharper's runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(new { Test = "243", Age = 42 }, Has.Properties.EqualTo(new { Test = "222", Age = 46, Missing = true }));
        }
    }

    internal class SimplePerson
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    internal class ComplexPerson
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public int[] LuckyNumbers { get; set; }
    }

    internal class Address
    {
        public string Street { get; set; }
        public int ZipCode { get; set; }
    }

    internal class PersonWithAge
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    internal class PersonWithoutAge
    {
        public string Name { get; set; }
    }

    internal class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    internal class TextPair
    {
        public string First { get; set; }
        public string Second { get; set; }
    }

    internal class ShapeHolder
    {
        public ShapeBase Shape { get; set; }
    }

    internal class DerivedShapeHolder : ShapeHolder
    {
    }

    internal class ShapeBase
    {
        public string Name { get; set; }
    }

    internal class DerivedShape : ShapeBase
    {
        public int Sides { get; set; }
    }

    internal class CircularNode
    {
        public string Name { get; set; }
        public CircularNode Next { get; set; }
    }

    internal class PersonWithTwoAddresses
    {
        public Address Home { get; set; }
        public Address Work { get; set; }
    }
}
