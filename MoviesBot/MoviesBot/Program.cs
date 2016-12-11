using MoviesBot.Data.TelegramTypes.Enums;
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
            var ms = new MovieService();
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
                        if (tg.WaitingChats.Exists(id => update.Message.Chat.Id == id))
                        {
                            var result = await ms.MovieSearch(update.Message.Text);
                            string messageForm = "";

                            if (result != null && result.Count != 0)
                            {
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "This is all I can offer you:");

                                int i = 1;
                                foreach (var movie in result)
                                    messageForm += i++ + ". " + movie.Title + " (" + movie.Year + ")" + "\n";

                            }
                            else
                                messageForm = "Unfortunately, I couldn't find anything for you. Please, make sure your request is correct";

                            await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, messageForm);
                            tg.WaitingChats.Remove(update.Message.Chat.Id);
                        }
                        else
                        {
                            switch (update.Message.Text)
                            {
                                case "/start":
                                case "/info":
                                    {
                                        await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "This bot is responsible for searching movies:\n"
                                            + "List of commands:\n"
                                            + "/info - Shows information about this bot (like this message)\n"
                                            + "/moviesearch - Provides search by film title/piece of title\n"
                                            + "/getfromtop250 - Returns random movie from IMDB top-250 best movies\n"
                                            + "/getbygenre - Returns random movie by genre");
                                        break;
                                    }
                                case "/moviesearch":
                                    {
                                        await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "Put estimated name of the movie you are looking for");
                                        tg.WaitingChats.Add(update.Message.Chat.Id);
                                        break;
                                    }
                                default:
                                    {
                                        await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "I can't understand your query. Write down /info to get all possible commands");
                                        break;
                                    }
                            }

                        }


                        //await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, $"Hello, {update.Message.User.FirstName}");
                        //await tg.SendPhotoAsync(update.Message.Chat.Id,
                        //"https://images-na.ssl-images-amazon.com/images/M/MV5BMTY2MTk3MDQ1N15BMl5BanBnXkFtZTcwMzI4NzA2NQ@@._V1_SX300.jpg", 
                        //"Here is a poster");
                        //await tg.SendStickerAsync(update.Message.Chat.Id, "BQADAgADVwMAAgw7AAEKdZtTTxHdkgoC");

                        //await tg.SendChatActionAsync(update.Message.Chat.Id, TelegramBotClient.ChatAction.typing);


                    }
                }
            }
        }
    }
}
