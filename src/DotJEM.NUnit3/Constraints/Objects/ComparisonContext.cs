using System.Collections.Generic;
using DotJEM.NUnit3.Util;

namespace DotJEM.NUnit3.Constraints.Objects
{
    /// <summary>
    /// Shared state for a single property-equality comparison tree. Tracks which expected objects
    /// have already been registered (construction-time) and binds them to their corresponding
    /// actual objects (match-time) so that back-references can be verified via reference equality
    /// instead of being silently skipped.
    /// </summary>
    internal class ComparisonContext
    {
        private readonly Dictionary<object, int> _expectedRefIds = new Dictionary<object, int>(new ReferenceComparer());
        private readonly Dictionary<int, object> _actualBindings = new Dictionary<int, object>();
        private int _nextId;
        private int _matchDepth;

        /// <summary>
        /// Registers <paramref name="obj"/> in the context. Returns <c>true</c> if this is the
        /// first registration (forward reference); <c>false</c> if already registered
        /// (back-reference). In both cases <paramref name="refId"/> is set to the object's id.
        /// </summary>
        public bool TryRegisterExpected(object obj, out int refId)
        {
            if (_expectedRefIds.TryGetValue(obj, out refId))
                return false;
            refId = _nextId++;
            _expectedRefIds[obj] = refId;
            return true;
        }

        /// <summary>Marks the start of a <c>Matches</c> call. Clears bindings on the outermost entry.</summary>
        public void EnterMatch()
        {
            if (_matchDepth++ == 0)
                _actualBindings.Clear();
        }

        /// <summary>Marks the end of a <c>Matches</c> call.</summary>
        public void ExitMatch() => _matchDepth--;

        /// <summary>Binds <paramref name="refId"/> to <paramref name="actual"/> on first visit.</summary>
        public void BindActual(int refId, object actual)
        {
            if (!_actualBindings.ContainsKey(refId))
                _actualBindings[refId] = actual;
        }

        /// <summary>Retrieves the actual object previously bound to <paramref name="refId"/>.</summary>
        public bool TryGetActualBinding(int refId, out object actual)
            => _actualBindings.TryGetValue(refId, out actual);
    }
}
