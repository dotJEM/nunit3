using DotJEM.NUnit3.Constraints;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Constraints
{
    [TestFixture]
    public class EnumerableEqualsConstraintTest
    {
        [Test]
        public void Test()
        {
            EnumerableEqualsConstraint constraint = new EnumerableEqualsConstraint(new [] { 1,2,3,4,5 });

            ConstraintResult result = constraint.ApplyTo(new[] { 1, 2, 3, 4, 5 });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }
        [Test]
        public void Test2()
        {
            EnumerableEqualsConstraint constraint = new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 });

            ConstraintResult result = constraint.ApplyTo(new[] { 1, 2, 2, 4, 5 });

            Assert.That(result.IsSuccess, Is.True, result.ToString());
        }
    }
}
