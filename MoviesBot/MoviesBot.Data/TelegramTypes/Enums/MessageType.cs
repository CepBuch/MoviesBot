using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.TelegramTypes.Enums
{
    public enum MessageType
    {

        TextMessage,
        PhotoMessage,
        VideoMessage,
        StickerMessage,
    }

    public static class MessageTypeExtension
    {
        public static KeyValuePair<string, string> ToKeyValue(this MessageType type)
        {
            switch (type)
            {
                case MessageType.TextMessage:
                    return new KeyValuePair<string, string>("sendMessage", "text");
                case MessageType.PhotoMessage:
                    return new KeyValuePair<string, string>("sendPhoto", "photo");
                case MessageType.VideoMessage:
                    return new KeyValuePair<string, string>("sendVideo", "video");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
