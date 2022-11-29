using System;
using System.Collections.Generic;
using System.Text;

namespace DotJEM.NUnit3.Expectations
{
    public class Assert : NUnit.Framework.Assert
    {
        public static void That(params ExpectationBuilder[] expectation)
        {

        }
    }

    public class Expect
    {
        public static ExpectationBuilder<T> That<T>(T value)
        {
            return null;
        }


    }


    public static class StaticImport
    {
        public static ExpectationBuilder<T> Expect<T>(T value)
        {
            return new ExpectationBuilder<T>(value);
        }
    }
    public interface IExpectation
    {

    }

    public abstract class Expectation : IExpectation
    {

    }

    public class AndExpectationBuilder : ExpectationBuilder
    {
        private readonly ExpectationBuilder[] expectations;

        public AndExpectationBuilder(params ExpectationBuilder[] expectations)
        {
            this.expectations = expectations;
        }
    }

    public class ExpectationBuilder
    {
        public static AndExpectationBuilder operator &(ExpectationBuilder left, ExpectationBuilder right)
        {
            return new AndExpectationBuilder(left, right);
        }
    }

    public class ExpectationBuilder<T>: ExpectationBuilder
    {
        public ExpectationBuilder(T value)
        {
        }
    }

    public static class ObjectExtentions
    {
        public static ExpectationBuilder<T> Is<T>(this T value)
        {
            return new ExpectationBuilder<T>(value);
        }
    }

    public static class ExpectationBuilderExtensions {
        public static ExpectationBuilder<T> EqualTo<T>(this ExpectationBuilder<T> self, T other)
        {
            return null;//self.Capture();
        }
    }

    
}
