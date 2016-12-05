

using Newtonsoft.Json;

namespace MoviesBot.Data.DTO
{
    class Message
    {
        [JsonProperty(PropertyName = "message_id", Required = Required.Always)]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "from", Required = Required.Default)]
        public User User { get; set; }
        [JsonProperty(PropertyName = "chat", Required = Required.Always)]
        public Chat Chat { get; set; }
        [JsonProperty(PropertyName = "text", Required = Required.Default)]
        public string Text { get; set; }
    }
}
