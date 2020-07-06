using Newtonsoft.Json;
using System.IO;

namespace InstagrammPasper.Classes
{
    public class UniversalSerializeDataClass<TModel>
    {
        public TModel DeserializeData(string fullPath)
        {
            var serializer = new JsonSerializer();
            TModel model;

            // Deserialize
            using var sr = new StreamReader(fullPath);
            using (var reader = new JsonTextReader(sr))
            {
                model = serializer.Deserialize<TModel>(reader);
                reader.Close();
            }
            sr.Close();

            return model;
        }

        public void SerializeData(TModel model, string fullPath)
        {
            var serializer = new JsonSerializer();

            // Serialize
            using var sw = new StreamWriter(fullPath);
            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, model);
                writer.Close();
            }
            sw.Close();
        }
    }
}
