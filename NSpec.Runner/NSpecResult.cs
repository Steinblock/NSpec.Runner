using NSpec.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSpec.Runner
{
    [Serializable]
    public class NSpecResults
    {
        private readonly string message;
        private readonly ContextCollection results;

        internal NSpecResults(ContextCollection results)
        {
            this.results = results;
            this.message = String.Format("{0} Examples, {1} Failed, {2} Pending",
                results.Examples().Count(),
                results.Failures().Count(),
                results.Pendings().Count()
            );
        }


        public bool HasFailures { get { return results.Failures().Any(); } }

        public bool HasPendings { get { return results.Pendings().Any(); } }

        public bool HasExamples { get { return results.Examples().Any(); } }

        public string Message { get { return message; } }
        
    }

}
