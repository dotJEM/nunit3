using DotJEM.NUnit3.Constraints;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Constraints.Collections
{
    public class EnumerableEqualsConstraintTest
    {
        [Test]
        public void ApplyTo_EquivalentCollections_Passes()
        {
            EnumerableEqualsConstraint constraint = new EnumerableEqualsConstraint(new [] { 1,2,3,4,5 });
            ConstraintResult result = constraint.ApplyTo(new[] { 1, 2, 3, 4, 5 });
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentCollections_Fails()
        {
            EnumerableEqualsConstraint constraint = new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 });
            ConstraintResult result = constraint.ApplyTo(new[] { 1, 2, 2, 4, 5 });

            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());
            Assert.That(result.ToString(), Is.EqualTo("  Expected: element at [2] to be: 3\r\n  But was:  2\r\n"));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(new[] { 1, 2, 2, 4, 5 }, new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 }));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_NotInRunnerDisplay()
        {
            Assert.That(new[] { 1, 2, 3, 4, 5 }, new NotConstraint(new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 })));
        }
    }
}
