using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace DotJEM.NUnit3.Legacy;

public class LegacyConstraintShimResult : ConstraintResult
{
    private readonly LegacyConstraintShim constraint;

    public LegacyConstraintShimResult(LegacyConstraintShim constraint, object actualValue, bool isSuccess)
        : base(constraint, actualValue, isSuccess)
    {
        this.constraint = constraint;
    }

    public override void WriteMessageTo(MessageWriter writer)
    {
        constraint.WriteMessageTo(writer);
    }
}

public abstract class LegacyConstraintShim : Constraint
{
    private bool result = true;
    private StringBuilder message;

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        message = new StringBuilder();
        DoMatches(actual);
        return new LegacyConstraintShimResult(this, actual, this.result);
    }

    protected abstract void DoMatches(object actual);

    public virtual void WriteMessageTo(MessageWriter writer)
    {
        writer.WriteMessageLine(GetType().FullName + " Failed!");
        using StringReader stringReader = new(this.message.ToString());
        while (stringReader.ReadLine() is { } str)
            writer.WriteMessageLine(str);
    }

    public virtual void WriteDescriptionTo(MessageWriter writer)
    {
    }

    protected bool FailWithMessage(string format, params object[] args)
    {
        this.AppendLine(string.Format(format, args));
        return this.Fail();
    }

    protected bool Fail() => this.result = false;

    protected StringBuilder AppendFormat(string format, params object[] args) => this.message.AppendFormat(format, args);

    protected StringBuilder AppendLine(string value) => this.message.AppendLine(value);

    protected StringBuilder AppendLine() => this.message.AppendLine();

}

public class NUnit26ConstraintResult : ConstraintResult
{
    private readonly NUnit26ConstraintShim constraint;

    public NUnit26ConstraintResult(NUnit26ConstraintShim constraint, object actualValue, bool isSuccess)
        : base(constraint, actualValue, isSuccess)
    {
        this.constraint = constraint;
    }

    public override void WriteMessageTo(MessageWriter writer)
    {
        constraint.WriteMessageTo(writer);
    }

    public override void WriteActualValueTo(MessageWriter writer)
    {
        constraint.WriteActualValueTo(writer);
    }

    public override void WriteAdditionalLinesTo(MessageWriter writer)
    {
        base.WriteMessageTo(writer);
    }
}

public abstract class NUnit26ConstraintShim : Constraint
{
    protected object actual;
   
    private readonly int argcnt;
    private readonly object arg1;
    private readonly object arg2;

    protected NUnit26ConstraintShim()
    {

    }

    protected NUnit26ConstraintShim(object arg)
    {
        this.argcnt = 1;
        this.arg1 = arg;
    }

    protected NUnit26ConstraintShim(object arg1, object arg2)
    {
        this.argcnt = 2;
        this.arg1 = arg1;
        this.arg2 = arg2;
    }

    public override ConstraintResult ApplyTo<TActual>(TActual actual)
    {
        bool result = this.Matches(actual);
        return new NUnit26ConstraintResult(this, actual, result);
    }

    public abstract bool Matches(object actualObject);

    public virtual bool Matches<T>(ActualValueDelegate<T> del)
    {
        throw new NotImplementedException();
    }

    public virtual bool Matches<T>(ref T actual)
    {
        throw new NotImplementedException();
    }

    public abstract void WriteDescriptionTo(MessageWriter writer);


    public virtual void WriteMessageTo(MessageWriter writer)
    {
        //writer.DisplayDifferences(this);
    }

    public virtual void WriteActualValueTo(MessageWriter writer)
    {
        writer.WriteActualValue(this.actual);
    }
}