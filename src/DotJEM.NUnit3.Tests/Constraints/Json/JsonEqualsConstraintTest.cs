using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotJEM.NUnit3.Constraints;
using DotJEM.NUnit3.Constraints.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Constraints.Json
{
    public class JsonEqualsConstraintTest
    {
        [Test]
        public void ApplyTo_EquivalentCollections_Passes()
        {
            JsonEqualsConstraint constraint = new JsonEqualsConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new { Name = "FOO" }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentCollections_Fails()
        {
            JsonEqualsConstraint constraint = new JsonEqualsConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new { Name = "FOX" }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());

            Assert.That(result.ToString(), Is.EqualTo("  Expected: element at [2] to be: 3\r\n  But was:  2\r\n"));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(JObject.FromObject(new { Name = "FOO" }), Is.Json.EqualTo(JObject.FromObject(new { Name = "FOX" })));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_NotInRunnerDisplay()
        {
            Assert.That(new[] { 1, 2, 3, 4, 5 }, new NotConstraint(new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 })));
        }
    }
}
