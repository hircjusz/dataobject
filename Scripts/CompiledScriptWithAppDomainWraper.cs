
namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Kontener zawieraj�cy skompilowany skrypt wraz z jego AppDomen�
    /// </summary>
    public class CompiledScriptWithAppDomainWraper
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="CompiledScriptWithAppDomainWraper"/>.
        /// </summary>
        /// <param name="compiledScript">Skompilowany skrypt</param>
        /// <param name="appDomainCacheWraper">Wraper AppDomain.</param>
        public CompiledScriptWithAppDomainWraper(Scripts.CompiledScript compiledScript, Scripts.AppDomainCacheWraper appDomainCacheWraper)
        {
            this.CompiledScript = compiledScript;
            this.AppDomainCacheWraper = appDomainCacheWraper;
        }



        /// <summary>
        /// Pozwala pobra� i ustawi� wraper AppDomain
        /// </summary>
        /// <value>Wraper AppDomain.</value>
        public AppDomainCacheWraper AppDomainCacheWraper
        {
            get;
            set;
        }

        /// <summary>
        /// Pozwala pobra� i ustawi� skompilowany skrypt
        /// </summary>
        /// <value>Skompilwoany skrypt</value>
        public CompiledScript CompiledScript
        {
            get;
            set;
        }
    }
}
