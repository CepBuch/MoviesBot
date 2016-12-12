using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoviesBot.Data.TelegramBotData.Types;
using System.Threading.Tasks;
using MoviesBot.Data.TelegramBotData.Enums;

namespace MoviesBot
{
    interface ITelegramBotClient
    {
        event Action<Message> OnMessageReceived;

        void StartBot();
        Task<bool> TestBot();

        Task<User> GetMeAsync();

        Task<Update[]> GetUpdatesAsync();

        Task<Message> SendMessageAsync(MessageType type, long chatId, string content,
            Dictionary<string, object> parameters = null);

        Task<Message> SendPhotoAsync(long chatId, string path, string caption = "");

        Task<Message> SendStickerAsync(long chatId, string stickerId);

        Task<bool> SendChatAction(long chatId, ChatAction action);

    }
}
