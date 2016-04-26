using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using NSpec.Domain.Formatters;
using PostSharp.Aspects;

namespace NSpec.Runner
{
    /// <summary>
    /// The aspect that gets added as an attribute to a test function.  
    /// It intercepts the function call an builds the nspec contexts for 
    /// the function and then invokes the function.
    /// </summary>
    [Serializable]
    public class TestSpecification : MethodInterceptionAspect
    {
        [XmlArrayItem(Type = typeof(MethodBase))]
        [XmlArray]
        private static List<MethodBase> methodsThatHaveBeenPrepared = new List<MethodBase>();
        
        static TestSpecification()
        {
            // NSpec.AssertionExtensions methods are wrappers for nunit assertations but NSpec has no direct dependency on nunit.framework itself.
            // That's why we include nunit.framework.dll from packages\nspec.1.0.5\tools as an embedded resource
            // and use AssemblyResolve event to dynamically load it at runtime
            // otherwise specs would wile with a FileNotFoundException
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // https://blogs.msdn.microsoft.com/microsoft_press/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition/
            var assemblyName = new AssemblyName(args.Name);
            String resourceName = "NSpec.Runner." + assemblyName.Name + ".dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null) return null;
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        public override void OnInvoke(MethodInterceptionArgs args)
        {

            // skip ignored specs
            if (ShouldIgnore(args.Method)) return;

            // if the method has been prepared, we need to actually execute the function
            if (methodsThatHaveBeenPrepared.Contains(args.Method) || !Enabled)
            {
                base.OnInvoke(args);
                return;
            }

            // mark that the method has been prepared
            methodsThatHaveBeenPrepared.Add(args.Method);

            // prepare the npsec content of the function
            var finder = new SingleClassGetter(args.Instance.GetType());
            var builder = new MethodContextBuilder(finder, new DefaultConventions());
            var formatter = new ConsoleFormatter(); // { WriteLineDelegate = value => Console.WriteLine(Regex.Replace(value, @"\r(?!\n)", Environment.NewLine)) }; // custom formatter to replace \r with newline 
            var runner = new MethodContextRunner(builder, formatter, this.FailFast);

            // set up the contexts for the method
            var methodInfo = args.Method as MethodInfo;
            var contexts = builder.MethodContext(methodInfo);

            // run the contexts
            var builtContexts = contexts.Build();
            var results = runner.Run(builtContexts);

            // wrap result and handle
            var nspecResults = new NSpecResults(results);
            ProcessResults(nspecResults);

        }

        /// <summary>
        /// Override to add Test Framework specific result handling.
        /// </summary>
        /// <param name="results"></param>
        public virtual void ProcessResults(NSpecResults results)
        {
            // if there were any failures, raise the exception so that the test framework has an error
            if (results.HasFailures)
                throw new TestFailedException(results.Message);

            // tests all passed
        }

        /// <summary>
        /// Determine if a certain test should be ignored
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public virtual bool ShouldIgnore(MethodBase method)
        {
            return false;
        }

        /// <summary>
        /// If false specs are not intercepted. Defaults to true, unless running from NSpecRunner.exe
        /// </summary>
        public virtual bool Enabled
        {
            get
            {
                return !Process.GetCurrentProcess().ProcessName.Equals("NSpecRunner", StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Configue if test should fail fast or not. Only applies if run if Enabled
        /// </summary>
        public virtual bool FailFast { get; set; }

    }
}
