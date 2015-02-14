namespace SoftwareMind.Shared.Serialization
{
    public static class JsonSerializerHelper
    {
        private static readonly JavaScriptSerializer Serializer;

        static JsonSerializerHelper()
        {
            Serializer = JavaScriptSerializer.Create();
        }

        public static TDto Deserialize<TDto>(string data)
        {
            return Serializer.Deserialize<TDto>(data);
        }

        public static string Serialize(object data)
        {
            return Serializer.Serialize(data);
        }
    }
}