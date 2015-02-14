using System;

namespace SoftwareMind.Shared.Infrastructure.Config
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class ConfigurationAttribute : Attribute
    {
        public ConfigurationType ConfigurationType { get; set; }
        public string Prefix { get; set; }
    }

    public enum ConfigurationType
    {
        Application = 0,
        ConnectionStrings
    }
}
