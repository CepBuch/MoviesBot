using MoviesBot.Data.DTO;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    class TelegramBotClient
    {
        public enum ChatAction
        {
            typing,
            uploading_photo,
            recording_video,
            uploading_video
        }

        public event Action<string> LogMessage;
        private readonly string _token;
        private const string _baseUrl = "https://api.telegram.org/bot";
        private int _lastUpdateId = 0;
        public TelegramBotClient(string token)
        {
            _token = token;
        }
        public async Task GetUpdates()
        {
            using (var client = new HttpClient())
            {
                string source = await client.GetStringAsync(_baseUrl + _token + "/getupdates?offset=" + (_lastUpdateId + 1));
                var response = Response.GetResponse(source);
                if (response.Results.Length != 0)
                {
                    LogMessage?.Invoke(String.Format("Message from {0} was recieved: {1} {2}",
                                     response.Results[0].Message.User.FirstName, response.Results[0].Message.Text, response.Results[0].Message.Chat.Id));
                    _lastUpdateId = response.Results[0].Id;
                }
            }
        }

        public void SendMessage(string message, int chatId)
        {
            using (var client = new WebClient())
            {
                NameValueCollection parse = new NameValueCollection();
                parse.Add("chat_id", chatId.ToString());
                parse.Add("text", message);
                client.UploadValues(_baseUrl + _token + "/sendMessage", parse);
            }
        }

        public async Task SendPhoto(int chatId, string path, string caption = "")
        {
            using (MultipartFormDataContent form = new MultipartFormDataContent())
            {
                string url = _baseUrl + _token + "/sendPhoto";
                string fileName = path.Split('\\').Last();

                form.Add(new StringContent(chatId.ToString(), Encoding.UTF8), "chat_id");
                form.Add(new StringContent(caption, Encoding.UTF8), "caption");
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    form.Add(new StreamContent(fileStream), "photo", fileName);
                    using (HttpClient httpClient = new HttpClient())
                    {
                        await httpClient.PostAsync(url, form);
                    }
                }
            }
        }

        public void SendSticker(int chatId, string stickerId)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection parse = new NameValueCollection();
                parse.Add("chat_id", chatId.ToString());
                parse.Add("sticker", stickerId);
                webClient.UploadValues(_baseUrl + _token + "/sendSticker", parse);
            }
        }

        public void SendChatAction(int chatId, ChatAction action)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection parse = new NameValueCollection();
                parse.Add("chat_id", chatId.ToString());
                parse.Add("action", action.ToString());
                webClient.UploadValues(_baseUrl + _token + "/sendChatAction", parse);
            }
        }
    }
}
