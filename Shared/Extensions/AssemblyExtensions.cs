using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SoftwareMind.Utils.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetManifestResourceString(this Assembly assembly, string name)
        {
            string result = null;

#if DEBUG
            // try to find file
            string basePath = Path.GetDirectoryName(assembly.CodeBase.Remove(0, 8)); // remove file:///
            List<string> parts = name.Split('.').ToList();

            while (parts.Count > 0 && !Directory.Exists(Path.Combine(basePath, parts.First())))
            {
                parts.RemoveAt(0);
            }

            while (parts.Count > 0 && Directory.Exists(Path.Combine(basePath, parts.First())))
            {
                basePath = Path.Combine(basePath, parts.First());
                parts.RemoveAt(0);

                string fileName = string.Join(".", parts);
                if (File.Exists(Path.Combine(basePath, fileName)))
                {
                    result = File.ReadAllText(Path.Combine(basePath, fileName), Encoding.UTF8);
                    return result;
                }
            }
#endif

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    return result;

                using(var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
