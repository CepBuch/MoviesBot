using Newtonsoft.Json;

namespace MoviesBot.Data.TelegramTypes
{
    class ReplyKeyboardMarkup
    {
        [JsonProperty(PropertyName = "keyboard", Required = Required.Always)]
        public KeyboardButton[][] Keyboard { get; set; }


    }
}
