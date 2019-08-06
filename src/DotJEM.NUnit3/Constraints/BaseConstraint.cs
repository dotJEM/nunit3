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
            MatchResult result = Matches(actual);
            return new BaseConstraintResult(this, actual, result);
        }

        protected abstract MatchResult Matches<T>(T actual);
    }

    public class MatchResult
    {
        public bool Failed => !Matches;
        public bool Matches { get; }

        public string ActualMessage { get; }
        public string ExpectedMessage { get; }

        protected MatchResult(bool matches, string actualMessage, string expectedMessage)
        {
            Matches = matches;
            ActualMessage = actualMessage;
            ExpectedMessage = expectedMessage;
        }

        public static MatchResult Fail(string expectedMessage, string actualMessage)
            => new MatchResult(false, expectedMessage, actualMessage);

        public static MatchResult Success() 
            => new MatchResult(true, string.Empty, string.Empty);

        public static implicit operator MatchResult(bool value)
        {
            return value
                ? Success()
                : Fail(null, null);
        }
    }


    public class BaseConstraintResult : ConstraintResult
    {
        private readonly MatchResult result;

        public BaseConstraintResult(IConstraint constraint, object actualValue, MatchResult result) 
            : base(constraint, actualValue, result.Matches ? ConstraintStatus.Success : ConstraintStatus.Failure)
        {
            this.result = result;
        }


        public override void WriteMessageTo(MessageWriter writer)
        {
            writer.WriteValue(result.ExpectedMessage);
            writer.WriteActualValue(result.ActualMessage);

            //base.WriteMessageTo(writer);
        }

        public override void WriteAdditionalLinesTo(MessageWriter writer)
        {
            base.WriteAdditionalLinesTo(writer);
        }

        public override void WriteActualValueTo(MessageWriter writer)
        {
            base.WriteActualValueTo(writer);
        }

        public override string ToString()
        {
            MessageWriter writer = new TextMessageWriter();
            WriteMessageTo(writer);
            return writer.ToString();
        }
    }
}
