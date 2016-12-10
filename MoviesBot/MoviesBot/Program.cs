using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MoviesBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient tg = new TelegramBotClient("273892003:AAH2kr6HrehC94NDV_kifhErXmi_TJmTV1A");
            //tg.LogMessage += a => Console.WriteLine(a);
            //while (true)
            //{
            //    tg.GetUpdates();
            //}
            tg.SendSticker(166300012, "BQADAgADVwMAAgw7AAEKdZtTTxHdkgoC");
            // tg.SendMessage("Hello world!", 166300012);
            // tg.SendPhoto(166300012, @"C:\Users\hp\Desktop\IMG_4505.JPG", "coca cola");
            //  Console.ReadLine();
        }
    }
}
