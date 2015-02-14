using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using log4net;

namespace SoftwareMind.Scripts
{
    [Serializable]
    internal class AssemblyResolver
    {
        public static Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == "ProxyAssembly, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                byte[] assemblyBytes = File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\ProxyAssemblyCache\\ProxyAssembly.dll");
                return Assembly.Load(assemblyBytes);
            }
            return null;
        }
    }

    /// <summary>
    /// Wraps AddDomain class and adds loaded assembly counter.
    /// </summary>
    public class AppDomainCacheWraper :  IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AppDomainCacheWraper));

        /// <summary>
        /// The created scripts.
        /// </summary>
        private List<string> createdScripts;
        /// <summary>
        /// Is object disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainCacheWraper"/> class.
        /// </summary>
        public AppDomainCacheWraper()
        {
            createdScripts = new List<string>();
            AppDomain currentDomain = AppDomain.CurrentDomain;
            AppDomainSetup setup = new AppDomainSetup();

            setup.ApplicationName = "ScriptAppDomain";
            setup.ApplicationBase = currentDomain.SetupInformation.ApplicationBase;
            setup.ConfigurationFile = currentDomain.SetupInformation.ConfigurationFile;
            setup.PrivateBinPath = currentDomain.SetupInformation.PrivateBinPath;

            Evidence baseEvidence = currentDomain.Evidence;
            Evidence evidence = new Evidence(baseEvidence);


            PermissionSet permisions = new PermissionSet(PermissionState.Unrestricted);
            //permisions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            //permisions.AddPermission(new ReflectionPermission(PermissionState.Unrestricted));
            //permisions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            //permisions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory + "\\ProxyAssemblyCache\\"));
            //permisions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, AppDomain.CurrentDomain.BaseDirectory + "\\ProxyAssemblyCache\\ProxyAssembly.dll"));
            //permisions.AddPermission(new OraclePermission(PermissionState.Unrestricted));
            //permisions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Append, AppDomain.CurrentDomain.BaseDirectory + "\\CommandLog.txt"));

            this.AppDomain = AppDomain.CreateDomain("ScriptAppDomain", evidence, setup, permisions, null);

            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            string typeName = typeof(DomainStartupAction).FullName;

            DomainStartupAction startupActions = (DomainStartupAction)this.AppDomain.CreateInstanceAndUnwrap(assemblyName, typeName);
            startupActions.SetStartupScript(DomainStartupAction.StartupCode);

            this.AppDomain.AssemblyResolve += AssemblyResolver.AppDomain_AssemblyResolve;
        }

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        /// <value>The active count.</value>
        public int ActiveCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the app domain.
        /// </summary>
        /// <value>The AppDomain.</value>
        private AppDomain AppDomain
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the created scripts.
        /// </summary>
        /// <value>The wraped readonly collection.</value>
        public ReadOnlyCollection<string> CreatedScripts
        {
            get
            {
                return new ReadOnlyCollection<string>(createdScripts);
            }
        }




        /// <summary>
        /// Creates the compiled script.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="referencedAssemblies">The referenced assemblies.</param>
        /// <returns>The CompiledScript and AppDomainWraper objects.</returns>
        /// <exception cref="Workflow.Scripts.CompilerErrorException">Throws when script contains syntax errors or other compiler
        /// error occurred.</exception>
        public CompiledScriptWithAppDomainWraper CreateCompiledScript(string sourceCode, IList<string> referencedAssemblies, string scriptKey)
        {
            string assemblyName = typeof(CompiledScript).Assembly.FullName;
            string className = typeof(CompiledScript).FullName;
            try
            {
                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                CompilerParameters cp = new CompilerParameters();
                cp.GenerateInMemory = false;
#if DEBUG
                cp.IncludeDebugInformation = true;
                string outPath = Path.GetTempFileName();
                cp.OutputAssembly = outPath;
#else
                cp.IncludeDebugInformation = false;
#endif
                foreach (string referencedAssembly in referencedAssemblies)
                    cp.ReferencedAssemblies.Add(referencedAssembly);
                if (!cp.ReferencedAssemblies.Contains(typeof(GeneratedScriptBase).Assembly.Location))
                    cp.ReferencedAssemblies.Add(typeof(GeneratedScriptBase).Assembly.Location);
                if (!cp.ReferencedAssemblies.Contains(typeof(System.Linq.Enumerable).Assembly.Location))
                    cp.ReferencedAssemblies.Add(typeof(System.Linq.Enumerable).Assembly.Location);

#if !DEBUG
                CompilerResults result = provider.CompileAssemblyFromSource(cp, sourceCode);
#else
                string tmpPath = Path.GetTempFileName();
                File.WriteAllText(tmpPath, sourceCode);
                CompilerResults result = provider.CompileAssemblyFromFile(cp, tmpPath);
#endif
                if (result.Errors.HasErrors)
                {
                    String msg = CreateCompilerErrorMessage(sourceCode, cp, result);
                    throw new CompilerErrorException(result.Errors, "Compiler error(s) occurred: " + msg);
                }
                byte[] data = File.ReadAllBytes(result.PathToAssembly);
#if !DEBUG

                File.Delete(result.PathToAssembly);
#endif

                CompiledScript compiledScript = (CompiledScript)AppDomain.CreateInstanceAndUnwrap(assemblyName, className, false,
                    BindingFlags.Default, null, new object[] { data }, null, null);
                ActiveCount++;
                createdScripts.Add(scriptKey);
                return new CompiledScriptWithAppDomainWraper(compiledScript, this);
            }
            catch (CompilerErrorException ex)
            {
                log.Error("CreateCompiledScript", ex);
                throw;
            }
            catch (TargetInvocationException ex)
            {
                log.Error("CreateCompiledScript", ex);
                throw ex.InnerException;
            }
            catch (Exception ex)
            {
                log.Error("CreateCompiledScript", ex);
                throw new CompilerErrorException("Compiler error(s) occurred:" + ex.Message, ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                AppDomain.Unload(AppDomain);
                AppDomain = null;
            }
            disposed = true;
        }

        /// <summary>
        /// Creates the compiler error message.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="cp">The compiler parameters.</param>
        /// <param name="result">The result.</param>
        /// <returns>The message</returns>
        private static String CreateCompilerErrorMessage(string sourceCode, CompilerParameters cp, CompilerResults result)
        {
            StringBuilder sb = new StringBuilder();
            foreach (CompilerError error in result.Errors)
                if (!error.IsWarning)
                {
                    sb.AppendLine(error.ToString());
                    sb.AppendLine();
                }
            sb.AppendLine();
            sb.AppendLine("In code : \r\n ");
            sb.AppendLine(sourceCode);
            sb.AppendLine();
            sb.AppendLine("Referenced assemblies:");
            foreach (var referencedAssemby in cp.ReferencedAssemblies)
                sb.AppendLine(referencedAssemby.ToString());
            return sb.ToString();
        }
    }
}
