using MoviesBot.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    class TelegramBotClient
    {
        public event Action<string> LogMessage;
        private readonly string _token;
        private const string _baseUrl = "https://api.telegram.org/bot";
        private int _lastUpdateId = 0;
        public TelegramBotClient(string token)
        {
            _token = token;
        }
        public void GetUpdates()
        {
            using (var client = new HttpClient())
            {
                string source = client.GetStringAsync(_baseUrl + _token + "/getupdates?offset=" + (_lastUpdateId + 1)).Result;
                var response = Response.GetResponse(source);
                if (response.Results.Length != 0)
                {
                    LogMessage?.Invoke(String.Format("Message from {0} was recieved: {1}",
                                     response.Results[0].Message.User.FirstName, response.Results[0].Message.Text));
                    _lastUpdateId = response.Results[0].Id;
                }
            }
        }
    }
}
