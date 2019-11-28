using Newtonsoft.Json;

namespace medlink.Helpers
{
    public class Serializer : ISerializer
    {
        public string Serialize<T>(T source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public T Deserialize<T>(string source)
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
    }
}