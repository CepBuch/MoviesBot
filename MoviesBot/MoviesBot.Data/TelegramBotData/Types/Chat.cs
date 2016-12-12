
using Newtonsoft.Json;

namespace MoviesBot.Data.TelegramBotData.Types
{
    public class Chat
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }
        [JsonProperty(PropertyName = "first_name", Required = Required.Default)]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "last_name", Required = Required.Default)]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "username", Required = Required.Default)]
        public string UserName { get; set; }
    }
}
