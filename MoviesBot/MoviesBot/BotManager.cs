using MoviesBot.Data;
using MoviesBot.Data.MovieData.Enums;
using MoviesBot.Data.TelegramBotData.Enums;
using MoviesBot.Data.TelegramBotData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    class BotManager
    {
        Dictionary<long, QueryType> _waitingForQuery;
        Dictionary<long, List<string>> _moviesForUser;
        Dictionary<long, string> _randomMovie;


        IMovieService _service;
        ITelegramBotClient _client;
        public BotManager(ITelegramBotClient client, IMovieService service)
        {
            _waitingForQuery = new Dictionary<long, QueryType>();
            _moviesForUser = new Dictionary<long, List<string>>();
            _randomMovie = new Dictionary<long, string>();
            _service = service;
            _client = client;
            client.OnMessageReceived += ProcessMessage;
        }

        public async void ProcessMessage(Message message)
        {
            //Если бот сейчас ждет ответа от пользователя
            if (_waitingForQuery.ContainsKey(message.Chat.Id))
            {
                //Смотрим ответ какого рода он ждет
                switch (_waitingForQuery[message.Chat.Id])
                {
                    //Если он ждет ответа на запрос поиска фильмов:
                    case QueryType.SearchMovie:
                        {
                            //Выполняем запрос, выводим сообщения, и заносим Названия фильмов в словарь если результаты были найдены
                            var movies = _service.SearchMovies(message.Text);
                            if (movies != null && movies.Count != 0)
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.AnswerIntroduction());
                                await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.GetListOfMoviesMessage(movies));
                                //Предлагаем выбрать фильм от 1 до movieCount
                                await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.MovieChooseMesage(movies.Count()));
                                _moviesForUser[message.Chat.Id] = movies.Select(m => m.Title).ToList();
                                //А также меняем статус ожидания бота - бот теперь ожидает выбор числа из фильмов
                                _waitingForQuery[message.Chat.Id] = QueryType.SelectMovie;
                            }
                            //Выводим сообщение и удаляем из словаря, если результатов найдено не было
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.MovieNotFoundMessage());
                                _waitingForQuery.Remove(message.Chat.Id);
                            }
                            break;
                        }
                    //Если он ждет ответа на выбор из списка фильмов:
                    case QueryType.SelectMovie:
                        {
                            if (_moviesForUser.ContainsKey(message.Chat.Id))
                            {
                                int chosenIndex = 1;
                                if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                    chosenIndex <= _moviesForUser[message.Chat.Id].Count)
                                {
                                    var movie = _service.SingleMovieSearch(_moviesForUser[message.Chat.Id][chosenIndex - 1]);
                                    _moviesForUser.Remove(message.Chat.Id);
                                    _waitingForQuery.Remove(message.Chat.Id);
                                    await _client.SendPhotoAsync(message.Chat.Id, movie.Poster, "Poster to " + movie.Title);
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.GetMovieInfoMessage(movie));
                                }
                                else if (message.Text.Trim().ToLower() == "no")
                                {
                                    _moviesForUser.Remove(message.Chat.Id);
                                    _waitingForQuery.Remove(message.Chat.Id);
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.SimpleCancelAnswer());
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.WrongChoseMessage());
                                }

                            }
                            break;
                        }
                    case QueryType.WaitingTrailer:
                        {
                            //-----------------------

                            //-----------------------
                            break;
                        }
                    case QueryType.ChoosingRandomMovie:
                        {
                            switch (message.Text.Trim().ToLower())
                            {
                                case "next":
                                    string title = _service.GetRandomFrom250();
                                    _randomMovie[message.Chat.Id] = title;
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.AnswerToRandomRequest(title));
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.ChooseMovieFromRandom());
                                    break;
                                case "ok":
                                    string name = "";
                                    _randomMovie.TryGetValue(message.Chat.Id, out name);
                                    var movie = _service.SingleMovieSearch(name);
                                    await _client.SendPhotoAsync(message.Chat.Id, movie.Poster);
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.GetMovieInfoMessage(movie));
                                    _waitingForQuery.Remove(message.Chat.Id);
                                    break;
                                case "no":
                                case "cancel":
                                    _randomMovie.Remove(message.Chat.Id);
                                    _waitingForQuery.Remove(message.Chat.Id);
                                    await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.SimpleCancelAnswer());
                                    break;
                                default:
                                    //can't understand message
                                    break;
                            }
                            break;
                        }

                }
            }
            //Если же бот не ждет ответа:
            else
            {
                switch (message.Text)
                {
                    case "/start":
                    case "/info":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.GetInfoMessage());
                            break;
                        }
                    case "/moviesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.EnterMovieTitleInviting());
                            _waitingForQuery[message.Chat.Id] = QueryType.SearchMovie;
                            break;
                        }
                    case "/getfromtop250":
                        {
                            string title = _service.GetRandomFrom250();
                            _randomMovie[message.Chat.Id] = title;
                            await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.AnswerToRandomRequest(title));
                            await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.ChooseMovieFromRandom());
                            _waitingForQuery.Add(message.Chat.Id, QueryType.ChoosingRandomMovie);
                            break;
                        }
                    default:
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, message.Chat.Id, BotAnswers.WrongQueryMessage());
                            break;
                        }
                }
            }
        }


    }
}
