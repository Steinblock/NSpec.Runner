using System;
using NSpec.Domain;
using NSpec.Domain.Formatters;

namespace NSpec.Runner
{
    [Serializable]
    public class MethodContextRunner
    {
        public ContextCollection Run(ContextCollection contexts)
        {
            try
            {
                ILiveFormatter liveFormatter = new SilentLiveFormatter();

                if (formatter is ILiveFormatter) liveFormatter = formatter as ILiveFormatter;

                contexts.Run(liveFormatter, failFast);

                if (builder.tagsFilter.HasTagFilters()) contexts.TrimSkippedContexts();

                formatter.Write(contexts);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return contexts;
        }

        public MethodContextRunner(MethodContextBuilder builder, IFormatter formatter, bool failFast)
        {
            this.failFast = failFast;
            this.builder = builder;
            this.formatter = formatter;
        }

        MethodContextBuilder builder;
        bool failFast;
        IFormatter formatter;
    }
}