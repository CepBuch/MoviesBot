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
        private readonly string _token;
        private const string _baseUrl = "https://api.telegram.org/bot";

        public TelegramBotClient(string token)
        {
            _token = token;
        }
    }
}
