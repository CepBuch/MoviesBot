

using Newtonsoft.Json;

namespace MoviesBot.Data.TelegramBotData.Types
{
    public class Response<T>
    {
        [JsonProperty(PropertyName = "ok", Required = Required.Always)]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "result", Required = Required.Default)]
        public T Result { get; set; }

    }
}
