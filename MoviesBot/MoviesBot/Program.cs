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
            StartBot();
            Console.ReadLine();
        }
        static async void StartBot()
        {
            TelegramBotClient tg = new TelegramBotClient("273892003:AAH2kr6HrehC94NDV_kifhErXmi_TJmTV1A");
            while (true)
            {
                //Получаем уведомления
                var updates = await tg.GetUpdatesAsync();
                if (updates != null && updates.Length != 0)
                {
                    //Для каждого уведомления выполняем действия
                    foreach (var update in updates)
                    {
                        Console.WriteLine("Message from {0} was recieved: {1} {2}", update.Message.User.FirstName, update.Message.Text,
                        update.Message.Chat.Id);
                        
                        await tg.SendPhotoAsync(update.Message.Chat.Id,
                           "https://images-na.ssl-images-amazon.com/images/M/MV5BMTY2MTk3MDQ1N15BMl5BanBnXkFtZTcwMzI4NzA2NQ@@._V1_SX300.jpg", 
                           "Here is a poster");
                        //await tg.SendMessageAsync(update.Message.Chat.Id, "Hello");
                        //await tg.SendStickerAsync(update.Message.Chat.Id, "BQADAgADVwMAAgw7AAEKdZtTTxHdkgoC");
                        //await tg.SendChatActionAsync(update.Message.Chat.Id, TelegramBotClient.ChatAction.typing);
                    }
                }
            }
        }
    }
}
