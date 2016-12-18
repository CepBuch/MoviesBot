using System;

namespace MoviesBot
{
    class Program
    {

        static void Main(string[] args)
        {
            TelegramBotClient tg = new TelegramBotClient("327020777:AAFLnI2lCN3xCSyI7t-KCnizFNRT6qApbCE");
            tg.LogMessage += m => Console.WriteLine(m);

            MovieService ms = new MovieService();

            BotManager manager = new BotManager(tg, ms);

            tg.StartBot();
            Console.ReadLine();
        }
    }
}
