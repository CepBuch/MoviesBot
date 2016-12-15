using MoviesBot.Data;
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
        Dictionary<long, string> _answerForUser;
        long _chatId;


        IMovieService _service;
        ITelegramBotClient _client;
        public BotManager(ITelegramBotClient client, IMovieService service)
        {
            _waitingForQuery = new Dictionary<long, QueryType>();
            _moviesForUser = new Dictionary<long, List<string>>();
            _answerForUser = new Dictionary<long, string>();
            _service = service;
            _client = client;
            client.OnMessageReceived += ProcessMessage;
        }

        public async void ProcessMessage(Message message)
        {
            _chatId = message.Chat.Id;
            //Если бот сейчас ждет ответа от пользователя
            if (_waitingForQuery.ContainsKey(_chatId))
            {
                //Смотрим ответ какого рода он ждет
                switch (_waitingForQuery[_chatId])
                {
                    //Если он ждет ответа на запрос поиска фильмов:
                    case QueryType.SearchingMovie:
                        {
                            var movies = _service.SearchMovies(message.Text);
                            if (movies != null && movies.Count != 0)
                            {
                                if (movies.Count == 1)
                                {
                                    _waitingForQuery.Remove(_chatId);
                                    await SendInfoAboutMovie(movies[0].Title);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.AnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.GetListOfMoviesMessage(movies));
                                    //Запоминаем фильмы, которые пришли конкретно этому пользователю
                                    _moviesForUser[_chatId] = movies.Select(m => m.Title).ToList();
                                    //Предлагаем выбрать фильм от 1 до movieCount
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.MovieChooseMesage(movies.Count()));
                                    //А также меняем статус ожидания бота - бот теперь ожидает выбор фильма пользоваталем
                                    _waitingForQuery[_chatId] = QueryType.SelectingMovie;
                                }
                            }
                            //Выводим сообщение и удаляем из словаря, если результатов найдено не было
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.MovieNotFoundMessage());
                                _waitingForQuery.Remove(_chatId);
                            }
                            break;
                        }
                    //Если он ждет ответа на выбор из списка фильмов:
                    case QueryType.SelectingMovie:
                        {
                            if (_moviesForUser.ContainsKey(_chatId))
                            {
                                int chosenIndex = 1;
                                //Если пользователь правильно выбрал фильм
                                if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                    chosenIndex <= _moviesForUser[_chatId].Count)
                                {
                                    await SendInfoAboutMovie(_moviesForUser[_chatId][chosenIndex - 1]);
                                    _moviesForUser.Remove(_chatId);
                                    _waitingForQuery.Remove(_chatId);
                                }
                                //Если от отменил операцию
                                else if (message.Text.Trim().ToLower() == "cancel")
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.SimpleCancelAnswer());
                                    _moviesForUser.Remove(_chatId);
                                    _waitingForQuery.Remove(_chatId);
                                }
                                //Если он ввел другой тип сообщения
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId , BotAnswers.WrongChoseMessage());
                                }
                            }
                            break;
                        }
                    //Если бот ждет согласия, на то, чтобы получить трейлер
                    case QueryType.WaitingTrailer:
                        {
                            //-----------------------
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.AnswerTrailer());
                            //-----------------------
                            break;
                        }
                    case QueryType.ChoosingRandomMovie:
                        {
                            switch (message.Text.Trim().ToLower())
                            {
                                case "next":
                                    string title = _service.GetRandomFrom250();
                                    _answerForUser[_chatId] = title;
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.AnswerToRandomRequest(title));
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.ChooseMovieFromRandom());
                                    break;
                                case "ok":
                                    string name = "";
                                    _answerForUser.TryGetValue(_chatId, out name);
                                    _waitingForQuery.Remove(_chatId);
                                    await SendInfoAboutMovie(name);
                                    break;
                                case "no":
                                case "cancel":
                                    _answerForUser.Remove(_chatId);
                                    _waitingForQuery.Remove(_chatId);
                                    await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.SimpleCancelAnswer());
                                    break;
                                default:
                                    //can't understand message
                                    break;
                            }
                            break;
                        }

                    case QueryType.SearchingPerson:
                        {
                            break;
                        }

                }
            }
            //Если же бот не ждет ответа:
            else
            {
                switch (message.Text.ToLower().Trim())
                {
                    case "/start":
                    case "/info":
                    case "hi":
                    case "hello":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.GetInfoMessage());
                            break;
                        }
                    case "/moviesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.EnterMovieTitleInviting());
                            _waitingForQuery[_chatId] = QueryType.SearchingMovie;
                            break;
                        }
                    case "/getfromtop250":
                        {
                            string title = _service.GetRandomFrom250();
                            _answerForUser[_chatId] = title;
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.AnswerToRandomRequest(title));
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.ChooseMovieFromRandom());
                            _waitingForQuery.Add(_chatId, QueryType.ChoosingRandomMovie);
                            break;
                        }
                    default:
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.WrongQueryMessage());
                            break;
                        }
                }
            }
        }



        public async Task SendInfoAboutMovie(string title)
        {
            var movie = _service.SingleMovieSearch(title);
            await _client.SendPhotoAsync(_chatId, movie.Poster, $"Poster to ''{movie.Title}''");
            await _client.SendMessageAsync(MessageType.TextMessage, _chatId, BotAnswers.GetMovieInfoMessage(movie));
        }

        


    }
}
