

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    public class Response
    {
        [JsonProperty(PropertyName = "ok", Required = Required.Always)]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public Result[] Results { get; set; }

        public static Response GetResponse(string source)
        {
            return JsonConvert.DeserializeObject<Response>(source);
        }
    }
}
