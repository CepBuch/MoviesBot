

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    public class Result
    {
        [JsonProperty(PropertyName = "update_id", Required = Required.Always)]
        public int Id { get; set; }
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public Message Message { get; set; }
    }
}
