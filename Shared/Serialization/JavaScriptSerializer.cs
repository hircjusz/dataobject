using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.Script.Serialization;

namespace SoftwareMind.Shared.Serialization
{
    public class JavaScriptSerializer : System.Web.Script.Serialization.JavaScriptSerializer
    {
        public bool SkipNulls { get; set; }

        private JavaScriptSerializer()
        {
            this.SkipNulls = false;
        }

        public static JavaScriptSerializer Create()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            ScriptingJsonSerializationSection section = ConfigurationManager.GetSection("system.web.extensions/scripting/webServices/jsonSerialization") as ScriptingJsonSerializationSection;

            if (section != null)
            {
                if ((section.Converters != null) && (section.Converters.Count > 0))
                {
                    IEnumerable<JavaScriptConverter> converters = CreateConvertersFrom(section.Converters);
                    serializer.RegisterConverters(converters);
                }

                serializer.MaxJsonLength = section.MaxJsonLength;
                serializer.RecursionLimit = section.RecursionLimit;
            }

            return serializer;
        }

        private static IEnumerable<JavaScriptConverter> CreateConvertersFrom(ConvertersCollection typeDefinitions)
        {
            foreach (Converter definition in typeDefinitions)
            {
                Type type = BuildManager.GetType(definition.Type, false);

                if (type == null)
                {
                    throw new ArgumentException("Unknown type.", definition.Type);
                }

                if (!typeof(JavaScriptConverter).IsAssignableFrom(type))
                {
                    throw new ArgumentException("Unsupported type.", definition.Type);
                }

                yield return (JavaScriptConverter)Activator.CreateInstance(type);
            }
        }
    }
}