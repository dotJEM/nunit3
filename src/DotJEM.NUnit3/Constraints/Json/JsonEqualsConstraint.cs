using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DotJEM.NUnit3.Constraints.Json
{
    public class JsonEqualsConstraint : BaseConstraint
    {
        private readonly JToken expected;

        protected override IMatchResult Matches<T>(T actual)
        {
            throw new NotImplementedException();
        }
    }
}
