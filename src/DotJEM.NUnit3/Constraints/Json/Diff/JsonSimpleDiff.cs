using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace DotJEM.NUnit3.Constraints.Json.Diff
{
    

    //public interface IDiffComparer<in T> : IComparer<T>
    //{
    //    public int Compare(T left, T right, T origin);
    //}

    //public class DiffComparer<T> : IDiffComparer<T>
    //{
    //    public int Compare(T x, T y)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}


    public interface IJsonSimpleDiff : IEnumerable<(string, JToken, JToken)>
    {
        bool Empty { get; }

        int Count { get; }

        IJsonSimpleDiff Push(string path, JToken left, JToken right);
    }

    public class JsonSimpleDiff : IJsonSimpleDiff
    {
        private readonly List<(string path, JToken left, JToken right)> diffs = new List<(string, JToken, JToken)>();

        public bool Empty => Count == 0;

        public int Count => diffs.Count;

        public IJsonSimpleDiff Push(string path, JToken left, JToken right)
        {
            diffs.Add((path, left, right));
            return this;
        }

        public IEnumerator<(string, JToken, JToken)> GetEnumerator() => diffs.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}