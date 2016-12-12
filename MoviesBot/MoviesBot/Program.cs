using MoviesBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoviesBot
{
    class Program
    {
        
        static  void Main(string[] args)
        {
            TelegramBotClient tg = new TelegramBotClient("273892003:AAH2kr6HrehC94NDV_kifhErXmi_TJmTV1A");
            tg.LogMessage += m => Console.WriteLine(m);
            MovieService ms = new MovieService();
            BotManager manager = new BotManager(tg, ms);
            tg.StartBot();
            Console.ReadLine();
        }

  
    }
}
