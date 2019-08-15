using System;
using System.Collections;
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

            return new JsonEqualsConstraintMatchResult(QuickDiff(new JsonSimpleDiff(), actual, expected));
        }

        private IJsonSimpleDiff QuickDiff(IJsonSimpleDiff result, JToken actual, JToken expected, string path = "")
        {
            if (actual == null && expected == null)
                return result;

            if (actual == null)
                return result.Push(path, "<null>", expected.ToString());

            if (expected == null)
                return result.Push(path, actual.ToString(), "<null>");

            if (actual.Type != expected.Type)
                return result.Push(path, $"JToken of type '{actual.Type}'", $"JToken of type '{expected.Type}'");

            if (expected is JObject obj)
                return QuickDiffObject(result, obj, (JObject)actual, path);

            //if (expected is JArray array)
            //    arrayStrategy.Compare()

            if (expected is JValue value && !value.Equals((JValue)actual))
                return result.Push(path, actual.ToString(), expected.ToString());

            return result;
        }

        private IJsonSimpleDiff QuickDiffObject(IJsonSimpleDiff result, JObject actual, JObject expected, string path = "")
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

    public interface IJsonSimpleDiff : IEnumerable<(string, JToken, JToken)>
    {
        int Count { get; }
        IJsonSimpleDiff Push(string path, JToken left, JToken right);
    }

    public class JsonSimpleDiff : IJsonSimpleDiff
    {
        private readonly List<(string path, JToken left, JToken right)> diffs = new List<(string, JToken, JToken)>();

        public int Count => diffs.Count;

        public IJsonSimpleDiff Push(string path, JToken left, JToken right)
        {
            diffs.Add((path, left, right));
            return this;
        }

        public IEnumerator<(string, JToken, JToken)> GetEnumerator() => diffs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class JsonEqualsConstraintMatchResult : IMatchResult
    {
        private readonly IJsonSimpleDiff diff;
        public bool Matches => diff.Count == 0;

        public JsonEqualsConstraintMatchResult(IJsonSimpleDiff diff)
        {
            this.diff = diff;
        }

        public void WriteTo(MessageWriter writer)
        {
            writer.WriteLine("Properties of the object did not match.");
            foreach ((string path, JToken actual, JToken expected) in diff)
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
