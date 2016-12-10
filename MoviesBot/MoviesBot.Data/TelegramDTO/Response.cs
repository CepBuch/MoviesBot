

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    public class Response<T>
    {
        [JsonProperty(PropertyName = "ok", Required = Required.Always)]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public T Result { get; set; }

        public static Response<T> GetResponse(string source)
        {
            return JsonConvert.DeserializeObject<Response<T>>(source);
        }
    }
}
