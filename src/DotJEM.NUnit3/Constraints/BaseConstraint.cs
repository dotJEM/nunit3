using System;
using System.Text;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;

namespace DotJEM.NUnit3.Constraints
{
    public abstract class BaseConstraint : Constraint
    {
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            IMatchResult result = Matches(actual);
            return new BaseConstraintResult(this, actual, result);
        }

        protected abstract IMatchResult Matches<T>(T actual);
    }
    
    public interface IMatchResult
    {
        bool Matches { get; }
        void WriteTo(MessageWriter writer);
    }

    public class MatchResult : IMatchResult
    {
        private readonly string actualMessage;
        private readonly string expectedMessage;

        public bool Matches { get; }

        protected MatchResult(bool matches, string expectedMessage, string actualMessage)
        {
            Matches = matches;
            this.actualMessage = actualMessage;
            this.expectedMessage = expectedMessage;
        }

        public void WriteTo(MessageWriter writer)
        {
            writer.Write(TextMessageWriter.Pfx_Expected);
            writer.WriteLine(expectedMessage);

            writer.Write(TextMessageWriter.Pfx_Actual);
            writer.WriteLine(actualMessage);
        }

        public static MatchResult Fail(string expectedMessage, string actualMessage)
            => new MatchResult(false, expectedMessage, actualMessage);

        public static MatchResult Success() 
            => new MatchResult(true, string.Empty, string.Empty);
    }

    public class BaseConstraintResult : ConstraintResult
    {
        private readonly IMatchResult result;

        public BaseConstraintResult(IConstraint constraint, object actualValue, IMatchResult result) 
            : base(constraint, actualValue, result.Matches ? ConstraintStatus.Success : ConstraintStatus.Failure)
        {
            this.result = result;
        }

        public override void WriteMessageTo(MessageWriter writer) => result.WriteTo(writer);

        public override void WriteAdditionalLinesTo(MessageWriter writer) => throw new NotImplementedException();

        public override void WriteActualValueTo(MessageWriter writer) => throw new NotImplementedException();

        public override string ToString()
        {
            MessageWriter writer = new TextMessageWriter();
            WriteMessageTo(writer);
            return writer.ToString();
        }
    }
}
