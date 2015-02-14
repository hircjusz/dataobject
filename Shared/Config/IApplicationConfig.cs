using System;
using SoftwareMind.Shared.Infrastructure.Config;

namespace SoftwareMind.Shared.Config
{
    [Configuration(Prefix = null)]
    public interface IApplicationConfig
    {
        DateTime BnppScaleInstalationDate { get; }
    }
}
