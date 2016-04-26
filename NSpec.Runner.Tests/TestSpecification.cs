using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace NSpec.Runner.Tests
{
    [Serializable, TestClass]
    public class TestSpecification : global::NSpec.Runner.TestSpecification
    {
        public static TestContext TestContext { get; protected set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // by marking this class as TestClass and adding an AssemblyInitialize attribut
            // we can reliably determine if run via Test Explorer (or hopefully any other MsTest compatible runner)
            // 
            // if TestContext != null Enabled will be false
            // and test will be processed by console runner
            TestContext = context;
        }

        public override void ProcessResults(NSpecResults results)
        {
            if (results.HasFailures)
                Assert.Fail(results.Message);
            else if (results.HasPendings)
                Assert.Inconclusive(results.Message);
        }

        public override bool ShouldIgnore(MethodBase method)
        {
            return method.GetCustomAttribute<IgnoreAttribute>() != null;
        }

        public override bool Enabled
        {
            get
            {
                return TestContext != null;
            }
        }

    }
}
