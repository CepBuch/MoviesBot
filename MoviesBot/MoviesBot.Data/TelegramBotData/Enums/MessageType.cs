﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.TelegramBotData.Enums
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
                case MessageType.StickerMessage:
                    return new KeyValuePair<string, string>("sendSticker", "sticker");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
