using MoviesBot.Data;
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


        public event Action<Message> OnMessageReceived;
        public event Action<string> LogMessage;

        private readonly string _token;
        private const string _baseUrl = "https://api.telegram.org/bot";
        private long _offset = 0;
        public TelegramBotClient(string token)
        {
            _token = token;
            WaitingChats = new List<long>();
            ChatMoviesDict = new Dictionary<long, List<string>>();
            OnMessageReceived += m =>
            {
                LogMessage($"Message from {m.User.FirstName} was recieved: {m.Text} {m.Chat.Id}");
                SendMessageAsync(MessageType.TextMessage, m.Chat.Id, BotAnswerMessages.GetInfoMessage());
            };
        }



        public async void StartBot()
        {
            while (true)
            {
                try
                {
                    var updates = await GetUpdatesAsync();
                    foreach (var update in updates)
                    {
                        OnMessageReceived?.Invoke(update.Message);
                        _offset = update.Id + 1;
                    }
                }
                catch (WebException) { }
            }
        }


        public async Task<bool> TestBot()
        {
            try
            {
                var response = await GetMeAsync();
                return response != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<User> GetMeAsync() => await SendWebRequest<User>("getMe");

        public async Task<Update[]> GetUpdatesAsync()
        {
            var parameters = new Dictionary<string, object>
            {
                {"offset", _offset},
            };
            return await SendWebRequest<Update[]>("getUpdates", parameters);
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


        public async Task<Message> SendStickerAsync(long chatId, string stickerId)
            => await SendMessageAsync(MessageType.StickerMessage, chatId, stickerId);

        public Task<bool> SendChatAction(long chatId, ChatAction action)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chat_id", chatId},
                {"action",action.ToString() }
            };
            return SendWebRequest<bool>("sendChatAction", parameters);
        }




        public async Task<T> SendWebRequest<T>(string methodName, Dictionary<string, object> parameters = null)
        {
            var uri = new Uri($"{_baseUrl}{_token}/{methodName}");

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
