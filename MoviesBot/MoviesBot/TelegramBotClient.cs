using MoviesBot.Data;
using MoviesBot.Data.TelegramBotData.Types;
using MoviesBot.Data.TelegramBotData.Enums;
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
using System.Threading;

namespace MoviesBot
{
    class TelegramBotClient : ITelegramBotClient
    {

        public event Action<Message> OnMessageReceived;
        public event Action<string> LogMessage;

        private readonly string _token;
        private const string _baseUrl = "https://api.telegram.org/bot";
        private long _offset = 0;
        public TelegramBotClient(string token)
        {
            _token = token;

            OnMessageReceived += m =>
            {
                LogMessage($"Message from {m.User.FirstName} {m.User.LastName} with chat_id {m.Chat.Id} was recieved: {m.Text}");
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
                catch (Exception ex)
                {
                    LogMessage?.Invoke($"Unable to get updates. Exception occured: {ex.Message}");
                    Thread.Sleep(10000);
                }
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
     Dictionary<string, object> parameters = null)
        {
            if (parameters == null)
                parameters = new Dictionary<string, object>();

            var typeInfo = type.ToKeyValue();

            parameters.Add("chat_id", chatId);
            if (!string.IsNullOrEmpty(typeInfo.Value))
                parameters.Add(typeInfo.Value, content);
            LogMessage($"Response to {chatId} was sent");

            return SendWebRequest<Message>(typeInfo.Key, parameters);
        }

        public async Task<Message> SendPhotoAsync(long chatId, string path, string caption = "")
        {
            var parameters = new Dictionary<string, object>
            {
                {"caption", caption }
            };
            LogMessage($"Photo to {chatId} was sent");
            return await SendMessageAsync(MessageType.PhotoMessage, chatId, path, parameters);
        }


        public async Task<Message> SendStickerAsync(long chatId, string stickerId)
            => await SendMessageAsync(MessageType.StickerMessage, chatId, stickerId);

        public async Task<bool> SendChatAction(long chatId, ChatAction action)
        {
            var parameters = new Dictionary<string, object>
            {
                {"chat_id", chatId},
                {"action",action.ToString() }
            };
            return await SendWebRequest<bool>("sendChatAction", parameters);
        }


        public async Task<T> SendWebRequest<T>(string methodName, Dictionary<string, object> parameters = null)
        {
            var uri = new Uri($"{_baseUrl}{_token}/{methodName}");
            Response<T> responseObject = null;

            using (var client = new HttpClient())
            {
                try
                {
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
                }
                catch (HttpRequestException e)
                {
                    throw new HttpRequestException(e.Message);
                }
            }
            return responseObject.Result;
        }
    }
}
