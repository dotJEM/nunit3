using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace DotJEM.NUnit3.Constraints.Json
{
    public class JsonEqualsConstraint : BaseConstraint
    {
        private readonly JToken expected;
        private IJArrayStrategy arrayStrategy = new StrictJArrayStrategy();

        public JsonEqualsConstraint(JToken expected)
        {
            this.expected = expected;
        }

        protected override IMatchResult Matches<T>(T actualValue)
        {
            if (ReferenceEquals(actualValue, expected))
                return MatchResult.Success();

            JToken actual = actualValue as JToken;
            if (actual == null)
                return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");
            
            if (JToken.DeepEquals(actual, expected))
                return MatchResult.Success();

            return QuickDiff(new JsonEqualsConstraintMatchResult(), actual, expected);
        }

        private JsonEqualsConstraintMatchResult QuickDiff(JsonEqualsConstraintMatchResult result, JToken actual, JToken expected, string path = "")
        {
            if (actual == null && expected == null)
                return result;

            if (actual == null)
                return result.Failure(path, "<null>", expected.ToString());

            if (expected == null)
                return result.Failure(path, actual.ToString(), "<null>");

            if (actual.Type != expected.Type)
                return result.Failure(path, $"JToken of type '{actual.Type}'", $"JToken of type '{expected.Type}'");

            if (expected is JObject obj)
                return QuickDiffObject(result, obj, (JObject)actual, path);

            //if (expected is JArray array)
            //    arrayStrategy.Compare()

            if (expected is JValue value && !value.Equals((JValue)actual))
                return result.Failure(path, actual.ToString(), expected.ToString());

            return result;
        }

        private JsonEqualsConstraintMatchResult QuickDiffObject(JsonEqualsConstraintMatchResult result, JObject actual, JObject expected, string path = "")
        {
            foreach (string key in UnionKeys(actual, expected))
            {
                string propertyPath = string.IsNullOrEmpty(path) ? key : path + "." + key;
                QuickDiff(result, actual[key], expected[key], propertyPath);
            }
            return result;
        }

        private IEnumerable<string> UnionKeys(IDictionary<string, JToken> update, IDictionary<string, JToken> other)
        {
            HashSet<string> keys = new HashSet<string>(update.Keys);
            keys.UnionWith(other.Keys);
            return keys;
        }
        
        public JsonEqualsConstraint AllowArrayOutOfOrder()
            => SetArrayStrategy<OutOfOrderJArrayStrategy>();

        public JsonEqualsConstraint StrictArrayOrder()
            => SetArrayStrategy<StrictJArrayStrategy>();

        private JsonEqualsConstraint SetArrayStrategy<TStrategy>() where TStrategy : IJArrayStrategy, new()
        {
            arrayStrategy = new TStrategy();
            return this;
        }
    }

    public class JsonEqualsConstraintMatchResult : IMatchResult
    {
        private readonly List<(string, string, string)> failures = new List<(string, string, string)>();

        public bool Matches => !failures.Any();

        public void WriteTo(MessageWriter writer)
        {
            writer.WriteLine("Properties of the object did not match.");
            foreach ((string path, string actual, string expected) in failures)
            {
                writer.WriteLine($"  Value at '{path}' was not equals.");
                writer.Write("  ");
                writer.Write(TextMessageWriter.Pfx_Expected);
                writer.WriteLine(expected);

                writer.Write("  ");
                writer.Write(TextMessageWriter.Pfx_Actual);
                writer.WriteLine(actual);
                writer.WriteLine();
            }
        }

        public JsonEqualsConstraintMatchResult Failure(string path, string actual, string expected)
        {
            failures.Add(( path,  actual,  expected));
            return this;
        }
    }

    public class HasJsonPropertiesConstraint : BaseConstraint
    {
        private readonly JToken expected;
        private IJArrayStrategy arrayStrategy = new StrictJArrayStrategy();

        public HasJsonPropertiesConstraint(JToken expected)
        {
            this.expected = expected;
        }

        protected override IMatchResult Matches<T>(T actualValue)
        {
            if (ReferenceEquals(actualValue, expected))
                return MatchResult.Success();

            JToken actual = actualValue as JToken;
            if (actual == null)
                return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");


            return MatchResult.Fail($"a {expected.GetType()}", $"a {actualValue.GetType()}");
        }


        private void QuickDiff(JToken actual, JToken expected, string path = "")
        {
            if (actual == null && expected == null)
                return;


        }




        public HasJsonPropertiesConstraint AllowArrayOutOfOrder()
            => SetArrayStrategy<OutOfOrderJArrayStrategy>();
 
        public HasJsonPropertiesConstraint StrictArrayOrder()
            => SetArrayStrategy<StrictJArrayStrategy>();

        private HasJsonPropertiesConstraint SetArrayStrategy<TStrategy>() where TStrategy : IJArrayStrategy, new()
        {
            arrayStrategy = new TStrategy();
            return this;
        }
    }

    public interface IJArrayStrategy
    {

    }

    public class StrictJArrayStrategy : IJArrayStrategy
    {
    }

    public class OutOfOrderJArrayStrategy : IJArrayStrategy
    {
    }
}
