using DotJEM.NUnit3.Legacy.Constraints;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DotJEM.NUnit3.Tests.Legacy.Constraints
{
    public class HasJsonPropertiesConstraintTest
    {
        [Test]
        public void ApplyTo_EquivalentJObjects_Passes()
        {
            HasJsonPropertiesConstraint constraint = new HasJsonPropertiesConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new { Name = "FOO" }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).True, result.ToString());
        }
        
        [Test]
        public void ApplyTo_MissingProperty_Passes()
        {
            HasJsonPropertiesConstraint constraint = new HasJsonPropertiesConstraint(JObject.FromObject(new { Name = "FOO" }));
            ConstraintResult result = constraint.ApplyTo(JObject.FromObject(new {  }));
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());
        }
     
        [Test]
        public void ApplyTo_DifferentJArrays_Fails()
        {
            JObject expected = JObject.FromObject(new { Arr = new [] { "ONE", "TWO", "THREE", "FOUR" }  });
            JObject actual = JObject.FromObject(new { Arr = new [] {"ONE", "TWO", "FOUR", "FIVE"}  });

            HasJsonPropertiesConstraint constraint = new HasJsonPropertiesConstraint(expected);
            ConstraintResult result = constraint.ApplyTo(actual);
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());

            string str = result.ToString();

            Assert.That(result.ToString(), Is.EqualTo("  DotJEM.NUnit3.Legacy.Constraints.HasJsonPropertiesConstraint Failed!" +
                                                      "\r\n  'Arr[2]' was expected to be 'THREE' but was 'FOUR'." +
                                                      "\r\n  'Arr[3]' was expected to be 'FOUR' but was 'FIVE'." +
                                                      "\r\n"));
        }

        [Test]
        public void ApplyTo_DeepObjects_Fails()
        {
            JObject expected = JObject.Parse("{" +
                                             "  arr: [" +
                                             "    { " +
                                             "      name: 'fox-01'," +
                                             "      arr: [" +
                                             "        { type: 'X01', number: 42 }," +
                                             "        { type: 'Y02', empty: [] }" +
                                             "      ]" +
                                             "    }," +
                                             "    { " +
                                             "      name: 'fox-02'," +
                                             "      arr: [" +
                                             "        { type: 'X01', number: 42 }," +
                                             "        { type: 'Y02', empty: [] }" +
                                             "      ]" +
                                             "    }" +
                                             "   " +
                                             "  ]" +
                                             "}");
            JObject actual = JObject.Parse("{" +
                                           "  arr: [" +
                                           "    { " +
                                           "      name: 'fox-01'," +
                                           "      arr: [" +
                                           "        { type: 'X01' }," +
                                           "        { type: 'Y02' }" +
                                           "      ]" +
                                           "    }," +
                                           "    { " +
                                           "      name: 'fox-02'," +
                                           "      arr: [" +
                                           "        { type: 'X01' }," +
                                           "        { type: 'Y02' }" +
                                           "      ]" +
                                           "    }" +
                                           "   " +
                                           "  ]" +
                                           "}");

            HasJsonPropertiesConstraint constraint = new HasJsonPropertiesConstraint(expected);
            ConstraintResult result = constraint.ApplyTo(actual);
            Assert.That(result, Has.Property<ConstraintResult>(r => r.IsSuccess).False, result.ToString());

            string str = result.ToString();

            Assert.That(result.ToString(), Is.EqualTo("  DotJEM.NUnit3.Legacy.Constraints.HasJsonPropertiesConstraint Failed!" +
                                                      "\r\n  Actual object did not contain a property named 'arr[0].arr[0].number'" +
                                                      "\r\n  Actual object did not contain a property named 'arr[0].arr[1].empty'" +
                                                      "\r\n  Actual object did not contain a property named 'arr[1].arr[0].number'" +
                                                      "\r\n  Actual object did not contain a property named 'arr[1].arr[1].empty'" +
                                                      "\r\n"));
        }
    }
}
