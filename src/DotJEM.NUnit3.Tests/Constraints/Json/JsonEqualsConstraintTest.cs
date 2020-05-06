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
        public void ApplyTo_EquivalentJObjects_Passes()
        {
            JsonEqualsConstraint constraint = new JsonEqualsConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new { Name = "FOO" }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }

        [Test]
        public void ApplyTo_DifferentJObjects_Fails()
        {
            JsonEqualsConstraint constraint = new JsonEqualsConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new { Name = "FOX" }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());

            Assert.That(result.ToString(), Is.EqualTo("JTokens did not match." +
                                                      "\r\n  Value at 'Name' was not equals." +
                                                      "\r\n    Expected: FOX" +
                                                      "\r\n    But was:  FOO\r\n\r\n"));
        }

        [Test]
        public void ApplyTo_DifferentJArrays_Fails()
        {
            JsonEqualsConstraint constraint = new JsonEqualsConstraint(JArray.FromObject( new [] { "ONE", "TWO", "THREE", "FOUR" } ))
                .AllowArrayOutOfOrder();
            ConstraintResult result = constraint.ApplyTo(JArray.FromObject( new[] { "ONE", "TWO", "FOUR", "FIVE" } ));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());

            string str = result.ToString();

            Assert.That(result.ToString(), Is.EqualTo("JTokens did not match." +
                                                      "\r\n  Value at '' was not equals." +
                                                      "\r\n    Expected: <missing>" +
                                                      "\r\n    But was:  THREE" +
                                                      "\r\n" +
                                                      "\r\n  Value at '' was not equals." +
                                                      "\r\n    Expected: FIVE" +
                                                      "\r\n    But was:  <extra>" +
                                                      "\r\n" +
                                                      "\r\n"));
        }



        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerDisplay()
        {
            Assert.That(JObject.FromObject(new { Name = "FOO" }), Is.Json.EqualTo(JObject.FromObject(new { Name = "FOX" })));
        }

        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_InRunnerArrayDisplay()
        {
            Assert.That(JObject.FromObject(new { Numbers = new [] { 1,2,3,4,5 } }), Is.Json.EqualTo(JObject.FromObject(new { Numbers = new[] { 1, 1, 1, 1, 1 } })));
        }


        [Test, Explicit("This test-case is meant to fail to verify how the error message is displayed in NUnit runners, e.g. ReSharpers runner")]
        public void Test_NotInRunnerDisplay()
        {
            Assert.That(new[] { 1, 2, 3, 4, 5 }, new NotConstraint(new EnumerableEqualsConstraint(new[] { 1, 2, 3, 4, 5 })));
        }
    }
}
