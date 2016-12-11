using MoviesBot.Data.DTO;
using MoviesBot.Data.TelegramTypes.Enums;
using Newtonsoft.Json;
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
        public List<long> WaitingChats { get; set; }
        public Dictionary<long, List<string>> ChatMoviesDict { get; set; }

        private readonly string _apiToken;
        private const string _baseUrl = "https://api.telegram.org/bot";
        private long _offset = 0;
        public TelegramBotClient(string token)
        {
            _apiToken = token;
            WaitingChats = new List<long>();
            ChatMoviesDict = new Dictionary<long, List<string>>();
        }

        public async Task<Update[]> GetUpdatesAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                {"offset", _offset+1 },
            };
            var updates = await SendWebRequest<Update[]>("getUpdates", parameters);
            if (updates != null && updates.Length != 0)
            {
                _offset = updates.Last().Id;
            }
            return updates;
        }



        public Task<Message> SendMessageAsync(MessageType type, long chatId, string content,
     Dictionary<string, object> parameters = null, long replyMessageId = 0)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            var typeInfo = type.ToKeyValue();

            parameters.Add("chat_id", chatId);
            if (!string.IsNullOrEmpty(typeInfo.Value))
                parameters.Add(typeInfo.Value, content);

            return SendWebRequest<Message>(typeInfo.Key, parameters);
        }

        public async Task<Message> SendPhotoAsync(long chatId, string path, string caption = "")
        {
            var parameters = new Dictionary<string, object>
            {
                {"caption", caption }
            };
            return await SendMessageAsync(MessageType.PhotoMessage, chatId, path, parameters);
        }


        public void SendSticker(long chatId, string stickerId)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection parse = new NameValueCollection();
                parse.Add("chat_id", chatId.ToString());
                parse.Add("sticker", stickerId);
                webClient.UploadValues(_baseUrl + _apiToken + "/sendSticker", parse);
            }
        }

        public void SendChatAction(long chatId, ChatAction action)
        {
            using (WebClient webClient = new WebClient())
            {
                NameValueCollection parse = new NameValueCollection();
                parse.Add("chat_id", chatId.ToString());
                parse.Add("action", action.ToString());
                webClient.UploadValues(_baseUrl + _apiToken + "/sendChatAction", parse);
            }
        }




        public async Task<T> SendWebRequest<T>(string methodName, Dictionary<string, object> parameters = null)
        {
            var uri = new Uri($"{_baseUrl}{_apiToken}/{methodName}");

            using (var client = new HttpClient())
            {
                Response<T> responseObject = null;
                HttpResponseMessage response;

                if (parameters == null || parameters.Count == 0)
                {
                    response = await client.GetAsync(uri);
                }
                else
                {
                    var data = JsonConvert.SerializeObject(parameters);
                    var httpContent = new StringContent(data, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(uri, httpContent);
                }
                var resultStr = await response.Content.ReadAsStringAsync();
                responseObject = JsonConvert.DeserializeObject<Response<T>>(resultStr);
                return responseObject.Result;
            }
        }
    }
}
