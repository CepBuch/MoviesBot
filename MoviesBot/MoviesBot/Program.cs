using MoviesBot.Data;
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
            AnotherStartBot();
            
            Console.ReadLine();
        }


        static async void AnotherStartBot()
        {
            TelegramBotClient tg = new TelegramBotClient("273892003:AAH2kr6HrehC94NDV_kifhErXmi_TJmTV1A");
            tg.LogMessage += m => Console.WriteLine(m);
            if (await tg.TestBot())
            {
                tg.StartBot();
            }
        }
        static async void StartBot()
        {
            TelegramBotClient tg = new TelegramBotClient("273892003:AAH2kr6HrehC94NDV_kifhErXmi_TJmTV1A");
            var ms = new MovieService();
            while (true)
            {
                var updates = await tg.GetUpdatesAsync();
                if (updates != null && updates.Length != 0)
                {
                    foreach (var update in updates)
                    {
                        if (tg.WaitingChats.Exists(id => update.Message.Chat.Id == id))
                        {
                            var result = await ms.MovieSearch(update.Message.Text);


                            if (result != null && result.Count != 0)
                            {
                                var titles = new List<string>();
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "This is all I can offer you:");

                                foreach (var movie in result)
                                {
                                    titles.Add(movie.Title);
                                }
                                tg.ChatMoviesDict[update.Message.Chat.Id] = titles;
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, BotAnswerMessages.GetListOfMoviesMessage(result));
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, BotAnswerMessages.MovieChooseMesage(result.Count));

                            }
                            else
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, BotAnswerMessages.MovieNotFoundMessage());


                            tg.WaitingChats.Remove(update.Message.Chat.Id);
                        }
                        else if (tg.ChatMoviesDict.ContainsKey(update.Message.Chat.Id))
                        {
                            int chosenIndex = 1;
                            if (update.Message.Text.Trim().ToLower() == "no")
                            {
                                tg.ChatMoviesDict.Remove(update.Message.Chat.Id);
                            }
                            else if (int.TryParse(update.Message.Text, out chosenIndex) && chosenIndex >= 1
                                && chosenIndex <= tg.ChatMoviesDict[update.Message.Chat.Id].Count)
                            {
                                var movie = await ms.SingleMovieSearch(tg.ChatMoviesDict[update.Message.Chat.Id][chosenIndex - 1]);
                                tg.ChatMoviesDict.Remove(update.Message.Chat.Id);

                                await tg.SendPhotoAsync(update.Message.Chat.Id, movie.Poster, movie.Title);
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, BotAnswerMessages.GetMovieInfoMessage(movie));

                            }
                            else
                            {
                                await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, "You are asked to chose the movie to get more about it or put ''No'' to cancel ");
                            }
                        }
                        else
                        {
                            switch (update.Message.Text)
                            {
                                case "/start":
                                case "/info":
                                    {
                                        await tg.SendMessageAsync(MessageType.TextMessage, update.Message.Chat.Id, BotAnswerMessages.GetInfoMessage());
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



                    }
                }
            }
        }
    }
}
