using Newtonsoft.Json;


namespace MoviesBot.Data.TelegramTypes
{
    class KeyboardButton
    {
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }


    }
}
