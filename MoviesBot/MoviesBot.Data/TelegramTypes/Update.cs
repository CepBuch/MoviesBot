

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    public class Update
    {
        [JsonProperty(PropertyName = "update_id", Required = Required.Always)]
        public long Id { get; set; }
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public Message Message { get; set; }
    }
}
