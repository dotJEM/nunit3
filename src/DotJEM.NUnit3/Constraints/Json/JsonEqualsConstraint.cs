using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotJEM.NUnit3.Constraints.Json.Diff;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace DotJEM.NUnit3.Constraints.Json
{
    public class JsonEqualsConstraint : BaseConstraint
    {
        private readonly JToken expected;
        private IJArrayStrategy arrayStrategy;

        public JsonEqualsConstraint(JToken expected)
        {
            this.expected = expected;
            WithStrictArrayOrder();
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

            return new JsonEqualsConstraintMatchResult(Diff(new JsonSimpleDiff(), actual, expected));
        }

        private IJsonSimpleDiff Diff(IJsonSimpleDiff result, JToken actual, JToken expected, string path = "")
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
                return Diff(result, obj, (JObject)actual, path);

            if (expected is JArray array)
                return arrayStrategy.Diff(result, (JArray) actual, array, path);

            if (expected is JValue value && !value.Equals((JValue)actual))
                return result.Push(path, actual.ToString(), expected.ToString());

            return result;
        }

        private IJsonSimpleDiff Diff(IJsonSimpleDiff result, JObject actual, JObject expected, string path = "")
        {
            foreach (string key in UnionKeys(actual, expected))
            {
                string propertyPath = string.IsNullOrEmpty(path) ? key : path + "." + key;
                Diff(result, actual[key], expected[key], propertyPath);
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

        public JsonEqualsConstraint WithStrictArrayOrder()
            => SetArrayStrategy<StrictJArrayStrategy>();

        private JsonEqualsConstraint SetArrayStrategy<TStrategy>() where TStrategy : IJArrayStrategy, new()
        {
            arrayStrategy = new TStrategy().WithTokenDiffer(Diff);
            return this;
        }
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
            writer.WriteLine("JTokens did not match.");
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

    public interface IJArrayStrategy
    {
        IJsonSimpleDiff Diff(IJsonSimpleDiff result, JArray actual, JArray expected, string path);

        IJArrayStrategy WithTokenDiffer(Func<IJsonSimpleDiff, JToken, JToken, string, IJsonSimpleDiff> tokenDiffer);

    }

    public abstract class AbstractJArrayStrategy : IJArrayStrategy
    {
        private Func<IJsonSimpleDiff, JToken, JToken, string, IJsonSimpleDiff> tokenDiffer;

        public abstract IJsonSimpleDiff Diff(IJsonSimpleDiff result, JArray actual, JArray expected, string path);

        protected IJsonSimpleDiff DiffToken(IJsonSimpleDiff result, JToken actual, JToken expected, string path) => tokenDiffer(result, actual, expected, path);

        public IJArrayStrategy WithTokenDiffer(Func<IJsonSimpleDiff, JToken, JToken, string, IJsonSimpleDiff> tokenDiffer)
        {
            this.tokenDiffer = tokenDiffer;
            return this;
        }
    }

    public class StrictJArrayStrategy : AbstractJArrayStrategy
    {
        public override IJsonSimpleDiff Diff(IJsonSimpleDiff result, JArray actual, JArray expected, string path)
        {
            if (actual.Count != expected.Count)
            {
                result.Push(path, $"JArray with {expected.Count} items", $"JArray with {actual.Count} items");
                return result;
            }

            for (int i = 0; i < expected.Count; i++)
                result = DiffToken(result, actual[i], expected[i], $"{path}[{i}]");
            return result;
        }
    }

    public class OutOfOrderJArrayStrategy : AbstractJArrayStrategy
    {
        public override IJsonSimpleDiff Diff(IJsonSimpleDiff result, JArray actual, JArray expected, string path)
        {
            if (actual.Count != expected.Count)
            {
                result.Push(path, $"JArray with {expected.Count} items", $"JArray with {actual.Count} items");
                return result;
            }

            int count = expected.Count;

            Table<IJsonSimpleDiff> table = new Table<IJsonSimpleDiff>(count, count);
            for (int x = 0; x < count; x++)
            for (int y = 0; y < count; y++)
            {
                IJsonSimpleDiff diff = DiffToken(new JsonSimpleDiff(), actual[y], expected[x], $"{path}[{x}]");
                table[x, y] = diff;
            }

            if (table.Columns.All(col => col.Any(diff => diff.Empty)) && table.Rows.All(col => col.Any(diff => diff.Empty)))
                return result;

            IEnumerable<int> missing = table.Columns
                .Select((col, i) => col.All(diff => !diff.Empty) ? i : -1)
                .Where(index => index >= 0);

            IEnumerable<int> extra = table.Rows
                .Select((row, i) => row.All(diff => !diff.Empty) ? i : -1)
                .Where(index => index >= 0);

            foreach (int i in missing)
                result.Push(path, expected[i], "<missing>");

            foreach (int i in extra)
                result.Push(path, "<extra>", actual[i]);

            return result;
        }
    }

    public interface ITable<T>
    {
        T this[int x, int y] { get; set; }
    }

    public class Table<T> : ITable<T>
    {
        private readonly T[] values;

        public int Width { get; }
        public int Height { get; }

        public T this[int x, int y]
        {
            get => values[Position(x,y)];
            set => values[Position(x, y)] = value;
        }

        public Table(int width, int height)
            : this(width, height, new T[width * height])
        {
            Width = width;
            Height = height;
            values = new T[width*height];
        }

        private Table(int width, int height, T[] values)
        {
            Width = width;
            Height = height;
            Debug.Assert(values.Length == width * height);
            this.values = values;
        }

        private int Position(int x, int y) => y * Width + x;

        public Table<TOut> Map<TOut>(Converter<T, TOut> mapper)
        {
            return new Table<TOut>(Width, Height, Array.ConvertAll(values, mapper));
        }

        public IEnumerable<IEnumerable<T>> Rows
        {
            get
            {
                for (int i = 0; i < Height; i++)
                    yield return EnumerateRow(i);
            }
        }

        public IEnumerable<T> EnumerateRow(int row)
        {
            if(row < 0) throw new ArgumentOutOfRangeException();
            if(row >= Height) throw new ArgumentOutOfRangeException();

            int lower = row * Width;
            int upper = lower + Width;
            for (int i = lower; i < upper; i++)
                yield return values[i];
        }
        public IEnumerable<IEnumerable<T>> Columns
        {
            get
            {
                for (int i = 0; i < Width; i++)
                    yield return EnumerateColumn(i);
            }
        }

        public IEnumerable<T> EnumerateColumn(int col)
        {
            if (col < 0) throw new ArgumentOutOfRangeException();
            if (col >= Width) throw new ArgumentOutOfRangeException();

            for (int i = col; i < values.Length; i += Width)
                yield return values[i];
        }
    }

    
}
