﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace DotJEM.NUnit3.Tests
{
    public class Class1
    {
        [Test, Explicit]
        public void Fax()
        {
            ReportFailure("ToString()");

            dynamic x;

            //Assert.That(42.Is().EqualTo(42) & 32.Is().EqualTo(32));
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