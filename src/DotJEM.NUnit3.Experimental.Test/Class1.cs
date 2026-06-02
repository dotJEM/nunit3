using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using DotJEM.NUnit3.Experimental.Expectations;

namespace DotJEM.NUnit3.Experimental.Test
{
    public class Class1
    {
        [Test, Explicit]
        public void Fax()
        {
            ReportFailure("ToString()");

            dynamic x;

            XAssert.That(42.Is().EqualTo(42) & 32.Is().EqualTo(32));
            //Expect(x)
        }

        private static void ReportFailure(string message)
        {
            // Record the failure in an <assertion> element
            var result = TestExecutionContext.CurrentContext.CurrentResult;
            result.RecordAssertion(AssertionStatus.Failed, message, "GetStackTrace()");
            result.RecordTestCompletion();
            throw new AssertionException(message);
        }
    }
}