using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotJEM.NUnit3.Constraints;
using DotJEM.NUnit3.Constraints.Objects;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Constraints.Objects
{
    public class ObjectPropertiesEqualsConstraintTest
    {
        [Test]
        public void ApplyTo_EquivalentCollections_Passes()
        {
            var constraint = Has.Properties.EqualTo(new {Test = "243"});
            ConstraintResult result = constraint.ApplyTo(new { Test = "243" });
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentCollections_Fails()
        {
            var constraint = Has.Properties.EqualTo(new { Test = "222" });
            ConstraintResult result = constraint.ApplyTo(new { Test = "243" });

            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());
            Assert.That(result.ToString(), Is.EqualTo("Properties of the object did not match." +
                                                      "\r\n  The property 'Test <System.String>' was not equals." +
                                                      "\r\n    String lengths are both 3. Strings differ at index 1." +
                                                      "\r\n    Expected: \"222\"" +
                                                      "\r\n    But was:  \"243\"" +
                                                      "\r\n    ------------^" +
                                                      "\r\n  " +
                                                      "\r\n"));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(new { Test = "243", Age = 42 }, Has.Properties.EqualTo(new { Test = "222", Age = 46, Missing = true }));
        }
    }

    public class ObjectPropertiesNotEqualsConstraintTest
    {
        [Test]
        public void ApplyTo_SinglePropertyNotEqual_Passes()
        {
            var constraint = Has.Properties.NotEqualTo(new { Test = "243" });
            ConstraintResult result = constraint.ApplyTo(new { Test = "244" });
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_MultiplePropertiesOneNotEqual_Passes()
        {
            var constraint = Has.Properties.NotEqualTo(new { Test = "243", Age = 42 });
            ConstraintResult result = constraint.ApplyTo(new { Test = "244", Age = 42 });
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_MultiplePropertiesOneNotEqualStrict_Fails()
        {
            var constraint = Has.Properties.NotEqualTo(new { Test = "243", Age = 42 }).Strict();
            ConstraintResult result = constraint.ApplyTo(new { Test = "244", Age = 42 });
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());
        }


        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(new { Test = "243", Age = 42 }, Has.Properties.NotEqualTo(new { Test = "243", Age = 42 }));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay2()
        {
            Assert.That(new { Test = "243", Age = 42 }, Has.Properties.NotEqualTo(new { Test = "243", Age = 43 }).Strict());
        }
    }

    /// <summary>
    /// Tests for stack overflow regression introduced under .NET 9.0 where
    /// InitializeProperties was recursing into static properties (e.g. DateTime.Now, DateTime.Today)
    /// that return new values on each access, defeating the cycle-detection in the references set.
    /// </summary>
    public class ObjectPropertiesEqualsConstraintStackOverflowRegressionTest
    {
        private class WithDateTime
        {
            public DateTime CreatedAt { get; set; }
            public string Name { get; set; }
        }

        private class Parent
        {
            public Child Child { get; set; }
        }

        private class Child
        {
            public Parent Parent { get; set; }
        }

        private class WithTypeProperty
        {
            public Type ObjectType { get; set; }
            public string Name { get; set; }
        }

        [Test]
        public void InitializeProperties_ObjectWithDateTimeProperty_DoesNotStackOverflow()
        {
            var expected = new WithDateTime { CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0), Name = "test" };
            Assert.DoesNotThrow(() =>
            {
                var constraint = new ObjectPropertiesEqualsConstraint<WithDateTime>(expected);
            });
        }

        [Test]
        public void ApplyTo_ObjectWithDateTimeProperty_Passes()
        {
            var expected = new WithDateTime { CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0), Name = "test" };
            var actual = new WithDateTime { CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0), Name = "test" };

            var constraint = new ObjectPropertiesEqualsConstraint<WithDateTime>(expected);
            ConstraintResult result = constraint.ApplyTo(actual);

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }

        [Test]
        public void InitializeProperties_ObjectWithCircularReference_DoesNotStackOverflow()
        {
            var parent = new Parent();
            var child = new Child { Parent = parent };
            parent.Child = child;

            Assert.DoesNotThrow(() =>
            {
                var constraint = new ObjectPropertiesEqualsConstraint<Parent>(parent);
            });
        }

        [Test]
        public void InitializeProperties_ObjectWithTypeProperty_DoesNotStackOverflow()
        {
            var expected = new WithTypeProperty { ObjectType = typeof(string), Name = "test" };
            Assert.DoesNotThrow(() =>
            {
                var constraint = new ObjectPropertiesEqualsConstraint<WithTypeProperty>(expected);
            });
        }
    }
}
