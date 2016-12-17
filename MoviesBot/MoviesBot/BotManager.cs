using MoviesBot.Data;
using MoviesBot.Data.MovieData.Model;
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
        Dictionary<string, int> _genres;
        Dictionary<long, QueryType> _waitingForQuery;
        Dictionary<long, List<string>> _moviesForUser;
        Dictionary<long, string> _answerForUser;
        Dictionary<long, List<string>> _actorsForUser;


        IMovieService _service;
        ITelegramBotClient _client;
        public BotManager(ITelegramBotClient client, IMovieService service)
        {
            _waitingForQuery = new Dictionary<long, QueryType>();
            _moviesForUser = new Dictionary<long, List<string>>();
            _answerForUser = new Dictionary<long, string>();
            _actorsForUser = new Dictionary<long, List<string>>();

            _service = service;
            _client = client;
            //Getting genres from themoviedb service
            _genres = _service.GetGenres();
            client.OnMessageReceived += ProcessMessage;
        }

        public async void ProcessMessage(Message message)
        {
            var chatId = message.Chat.Id;
            //Если бот сейчас ждет ответа от пользователя
            if (_waitingForQuery.ContainsKey(chatId))
            {
                //Смотрим ответ какого рода он ждет
                switch (_waitingForQuery[chatId])
                {
                    //Если он ждет ответа на запрос поиска фильмов:
                    case QueryType.SearchingMovie:
                        {
                            var movies = _service.SearchMovies(message.Text);
                            if (movies != null && movies.Count != 0)
                            {
                                if (movies.Count == 1)
                                {
                                    _waitingForQuery.Remove(chatId);
                                    await SendInfoAboutMovie(chatId, movies[0].Title);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetListOfMoviesMessage(movies));
                                    //Запоминаем фильмы, которые пришли конкретно этому пользователю
                                    _moviesForUser[chatId] = movies.Select(m => m.Title).ToList();
                                    //Предлагаем выбрать фильм от 1 до movieCount
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieChooseMesage(movies.Count()));
                                    //А также меняем статус ожидания бота - бот теперь ожидает выбор фильма пользоваталем
                                    _waitingForQuery[chatId] = QueryType.SelectingMovie;
                                }
                            }
                            //Выводим сообщение и удаляем из словаря, если результатов найдено не было
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NotFoundMessage());
                                _waitingForQuery.Remove(chatId);
                            }
                            break;
                        }
                    //Если он ждет ответа на выбор из списка фильмов:
                    case QueryType.SelectingMovie:
                        {
                            if (_moviesForUser.ContainsKey(chatId))
                            {
                                int chosenIndex = 1;
                                //Если пользователь правильно выбрал фильм
                                if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                    chosenIndex <= _moviesForUser[chatId].Count)
                                {
                                    await SendInfoAboutMovie(chatId, _moviesForUser[chatId][chosenIndex - 1]);
                                    _moviesForUser.Remove(chatId);
                                    _waitingForQuery.Remove(chatId);
                                }
                                //Если от отменил операцию
                                else if (message.Text.Trim().ToLower() == "cancel")
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                    _moviesForUser.Remove(chatId);
                                    _waitingForQuery.Remove(chatId);
                                }
                                //Если он ввел другой тип сообщения
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                                }
                            }
                            break;
                        }
                    //Если бот ждет согласия, на то, чтобы получить трейлер
                    case QueryType.WaitingTrailer:
                        {
                            //-----------------------
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerTrailer());
                            //-----------------------
                            break;
                        }
                    case QueryType.SelectingRandomMovie:
                        {
                            switch (message.Text.Trim().ToLower())
                            {
                                case "next":
                                    string title = _service.GetRandomFrom250();
                                    _answerForUser[chatId] = title;
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerToRandomRequest(title));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ChooseMovieFromRandom());
                                    break;
                                case "ok":
                                    string name = "";
                                    _answerForUser.TryGetValue(chatId, out name);
                                    _waitingForQuery.Remove(chatId);
                                    await SendInfoAboutMovie(chatId, name);
                                    break;
                                case "no":
                                case "cancel":
                                    _answerForUser.Remove(chatId);
                                    _waitingForQuery.Remove(chatId);
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                    break;
                                default:
                                    //can't understand message
                                    break;
                            }
                            break;
                        }

                    case QueryType.SearchingPerson:
                        {
                            var actors = _service.SearchActors(message.Text);

                            if (actors != null && actors.Count != 0)
                            {
                                if (actors.Count == 1)
                                {
                                    _actorsForUser.Remove(chatId);
                                    await SendInfoAboutActor(chatId, actors[0].Name);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ListActorsAnswer(actors));
                                    _actorsForUser[chatId] = actors.Select(a => a.Name).ToList();
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ActorChooseMesage(actors.Count()));
                                    _waitingForQuery[chatId] = QueryType.SelectingPerson;
                                }
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ActorNotFoundMessage());
                                _waitingForQuery.Remove(chatId);
                            }
                            break;
                        }

                    case QueryType.SelectingPerson:
                        {
                            if (_actorsForUser.ContainsKey(chatId))
                            {
                                int chosenIndex = 1;
                                if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                  chosenIndex <= _actorsForUser[chatId].Count)
                                {
                                    await SendInfoAboutActor(chatId, _actorsForUser[chatId][chosenIndex - 1]);
                                    _actorsForUser.Remove(chatId);
                                    _waitingForQuery.Remove(chatId);
                                }
                                else if (message.Text.Trim().ToLower() == "cancel")
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                    _actorsForUser.Remove(chatId);
                                    _waitingForQuery.Remove(chatId);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                                }
                            }
                            break;
                        }
                    case QueryType.SelectingGenre:
                        {
                            if (_genres.ContainsKey(message.Text.Trim().ToLower()))
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, "сейчас вам придет фильм на жанр по айдишнику " + _genres[message.Text.ToLower().Trim()]);
                            }
                            else if (message.Text.Trim().ToLower() == "cancel")
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                _waitingForQuery.Remove(chatId);
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                            }
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
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetInfoMessage());
                            break;
                        }
                    case "/moviesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.EnterMovieTitleInviting());
                            _waitingForQuery[chatId] = QueryType.SearchingMovie;
                            break;
                        }
                    case "/getfromtop250":
                        {
                            string title = _service.GetRandomFrom250();
                            _answerForUser[chatId] = title;
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerToRandomRequest(title));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ChooseMovieFromRandom());
                            _waitingForQuery.Add(chatId, QueryType.SelectingRandomMovie);
                            break;
                        }
                    case "/peoplesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.EnterActorNameInviting());
                            _waitingForQuery[chatId] = QueryType.SearchingPerson;
                            break;
                        }
                    case "/getnowplaying":
                        {
                            var movies = _service.GetNowPlaying();
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.IntroductionToNowPlaying());
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerToNowPlaying(movies));
                            break;
                        }
                    case "/getbygenre":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresIntroduction());
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresAnswer(_genres.Keys.ToList()));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresChooseMessage());
                            _waitingForQuery[chatId] = QueryType.SelectingGenre;
                            break;
                        }
                    default:
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongQueryMessage());
                            break;
                        }
                }
            }
        }



        public async Task SendInfoAboutMovie(long chatId, string title)
        {
            var movie = _service.SingleMovieSearch(title);
            await _client.SendPhotoAsync(chatId, movie.Poster, $"Poster to ''{movie.Title}''");
            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetMovieInfoMessage(movie));
        }

        public async Task SendInfoAboutActor(long chatId, string name)
        {

            var actors = _service.SearchActors(name);
            if (actors != null && actors.Count != 0)
            {
                await _client.SendPhotoAsync(chatId, actors[0].Poster);
                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SingleSearchActorsAnswer(actors[0]));
            }
            else await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NotFoundMessage());

        }
    }
}
