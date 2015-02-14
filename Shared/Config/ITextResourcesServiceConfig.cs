using System.Collections.Generic;
using System.ComponentModel;
using SoftwareMind.Shared.Infrastructure.Config;

namespace SoftwareMind.Shared.Config
{
    [Configuration(Prefix = "TextResources")]
    public interface ITextResourcesServiceConfig
    {
        [DefaultValue("PL=polski")]
        IDictionary<string, string> LanguageMapping { get; }

        [DefaultValue("polski")]
        string DefaultLanguage { get; }
    }
}