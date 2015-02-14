using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    public static class CloneHelper
    {
        /// <remarks>
        /// HACK: dość szybki sposób na sklonowanie bardzie zlozonego objektu :)
        /// </remarks>
        public static T DeepCopy<T>(this T original) where T : class
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, original);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }

    }
}
