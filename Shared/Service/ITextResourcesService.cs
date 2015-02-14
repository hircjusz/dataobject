using System;
using System.Collections.Generic;

namespace SoftwareMind.Shared.Service
{
    public interface ITextResourcesService
    {
        event EventHandler Changed;

        IDictionary<string, string> GetEntries(string language);
        IEnumerable<string> GetLanguages();
        IDictionary<string, string> GetLanguageMappings();

        string GetText(string lang, string key);
        string GetTextFormatted(string lang, string key, params object[] parameters);

        string DefaultLanguage { get; }
    }
}