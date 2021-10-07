using System.IO;

namespace AutoWifiAdbConnect
{
    public static class JsonUtilities
    {
        public static void SerializeToTextWriter(object value, TextWriter outStream)
        {
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Converters.Add(new Newtonsoft.Json.Converters.JavaScriptDateTimeConverter());
            serializer.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            serializer.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto;
            serializer.Formatting = Newtonsoft.Json.Formatting.Indented;
            using (Newtonsoft.Json.JsonWriter writer = new Newtonsoft.Json.JsonTextWriter(outStream))
            {
                serializer.Serialize(writer, value, value.GetType());
            }
            outStream.Close();
        }

        public static void SerializeToFile(object value, string path)
        {
            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            SerializeToTextWriter(value, File.CreateText(path));
        }



        public static T Deserialize<T>(string serialized)
        {
            return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(serialized, typeof(T), new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });
        }
    }
}
