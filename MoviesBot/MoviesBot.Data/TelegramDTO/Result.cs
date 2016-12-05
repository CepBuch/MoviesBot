

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    class Result
    {
        [JsonProperty(PropertyName = "update_id", Required = Required.Always)]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "message", Required = Required.Default)]
        public Message Message { get; set; }
    }
}
