using System;
using System.Collections.Generic;
using log4net;
using SoftwareMind.Shared.Mef;
using SoftwareMind.Shared.Service;

namespace SoftwareMind.Shared.Utils
{
    public static class ResX
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ResX));

        private static readonly Lazy<ITextResourcesService> service = new Lazy<ITextResourcesService>(() => ServiceLocator.Current.GetInstance<ITextResourcesService>());

        #region · Public interface ·
        //[Obsolete("This method is left for compatibility reason. Use GetString with language parameter instead. If there is no need to use specific localization, use ResX.DefaultLanguage.")]
        public static string GetString(string key)
        {
            return service.Value.GetText(DefaultLanguage, key);
        }

        public static string GetString(string key, string language)
        {
            return service.Value.GetText(language, key);
        }

        public static string GetFormattedString(string key, string language, params object[] parameters)
        {
            return service.Value.GetTextFormatted(language, key, parameters);
        }

        /// <summary>
        /// Gets all resources defined
        /// Every entry is returned as a dictionary entry with key and value
        /// </summary>
        /// <returns>defined resources as a dictionary with key and value</returns>
        public static IDictionary<string, string> GetResXEntries(string language)
        {
            return service.Value.GetEntries(language);

        }
        #endregion

        #region · Properties ·
        public static string DefaultLanguage
        {
            get
            {
                return service.Value.DefaultLanguage;
            }
        }

        public static IEnumerable<string> AvailableLanguages
        {
            get
            {
                return service.Value.GetLanguages();
            }
        }

        public static IDictionary<string, string> LanguageMappings
        {
            get
            {
                return service.Value.GetLanguageMappings();
            }
        }

        #endregion
    }


}
