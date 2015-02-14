
namespace SoftwareMind.Scripts
{
    /// <summary>
    /// Kontener zawieraj¹cy skompilowany skrypt wraz z jego AppDomen¹
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
        /// Pozwala pobraæ i ustawiæ wraper AppDomain
        /// </summary>
        /// <value>Wraper AppDomain.</value>
        public AppDomainCacheWraper AppDomainCacheWraper
        {
            get;
            set;
        }

        /// <summary>
        /// Pozwala pobraæ i ustawiæ skompilowany skrypt
        /// </summary>
        /// <value>Skompilwoany skrypt</value>
        public CompiledScript CompiledScript
        {
            get;
            set;
        }
    }
}
