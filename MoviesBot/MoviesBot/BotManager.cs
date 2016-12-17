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
        Dictionary<long, QueryType> _botWaitsForQuery;
        Dictionary<long, List<string>> _multipleDataForUser;
        Dictionary<long, string> _singleDataForUser;

        bool genresWasDownloaded;


        IMovieService _service;
        ITelegramBotClient _client;


        public BotManager(ITelegramBotClient client, IMovieService service)
        {
            _botWaitsForQuery = new Dictionary<long, QueryType>();
            _multipleDataForUser = new Dictionary<long, List<string>>();
            _singleDataForUser = new Dictionary<long, string>();

            _service = service;
            _client = client;

            //Getting genres from themoviedb service
            _genres = _service.GetGenres();
            genresWasDownloaded = _genres != null && _genres.Count != 0;


            client.OnMessageReceived += ProcessMessage;
        }

        public async void ProcessMessage(Message message)
        {
            var chatId = message.Chat.Id;
            //Checking if bot is waiting for user's answer or choice.
            if (_botWaitsForQuery.ContainsKey(chatId))
            {
                switch (_botWaitsForQuery[chatId])
                {
                    case QueryType.SearchingMovie:
                        {
                            var movies = _service.SearchMovies(message.Text);
                            if (movies != null && movies.Count != 0)
                            {
                                if (movies.Count == 1)
                                {
                                    await SendInfoAboutMovie(chatId, movies[0]);
                                    _botWaitsForQuery[chatId] = QueryType.AnsweringTrailerQuestion;
                                    _singleDataForUser[chatId] = movies[0].Title;
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetListOfMoviesMessage(movies));
                                    //Запоминаем фильмы, которые пришли конкретно этому пользователю
                                    _multipleDataForUser[chatId] = movies.Select(m => m.Title).ToList();
                                    //Предлагаем выбрать фильм от 1 до movieCount
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieChooseMesage(movies.Count()));
                                    //А также меняем статус ожидания бота - бот теперь ожидает выбор фильма пользоваталем
                                    _botWaitsForQuery[chatId] = QueryType.SelectingMovie;
                                }
                            }
                            //Выводим сообщение и удаляем из словаря, если результатов найдено не было
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NotFoundMessage());
                                _botWaitsForQuery.Remove(chatId);
                            }
                            break;
                        }
                    case QueryType.SelectingMovie:
                        {
                            //Checking if user entered the correct number of movie or cancelled the operation.
                            int chosenIndex = 1;

                            if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                chosenIndex <= _multipleDataForUser[chatId].Count)
                            {
                                var title = _multipleDataForUser[chatId][chosenIndex - 1];
                                var movie = _service.SingleMovieSearch(title);
                                await SendInfoAboutMovie(chatId, movie);
                                _multipleDataForUser.Remove(chatId);
                                _singleDataForUser[chatId] = title;
                                _botWaitsForQuery[chatId] = QueryType.AnsweringTrailerQuestion;

                            }
                            else if (message.Text.Trim().ToLower() == "cancel")
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                _multipleDataForUser.Remove(chatId);
                                _botWaitsForQuery.Remove(chatId);
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                            }
                        }
                        break;
                    case QueryType.AnsweringTrailerQuestion:
                        {
                            if (message.Text.Trim().ToLower() == "yes")
                            {
                                var link = _service.GetTrailerLinkForMovie(_singleDataForUser[chatId]);
                                if (link != null)
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerTrailer());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, link);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.TrailerWasNotFound());
                                }
                                _singleDataForUser.Remove(chatId);
                                _botWaitsForQuery.Remove(chatId);
                            }
                            else if (message.Text.Trim().ToLower() == "no" || message.Text.Trim().ToLower() == "cancel")
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                _singleDataForUser.Remove(chatId);
                                _botWaitsForQuery.Remove(chatId);
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                            }
                            break;
                        }
                    case QueryType.SelectingRandomMovie:
                        {
                            string title;
                            switch (message.Text.Trim().ToLower())
                            {

                                case "next":
                                     title = _service.GetRandomFrom250();
                                    _singleDataForUser[chatId] = title;
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerToRandomRequest(title));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ChooseMovieFromRandom());
                                    break;
                                case "ok":
                                    _singleDataForUser.TryGetValue(chatId, out title);
                                    var movie = _service.SingleMovieSearch(title);
                                    _botWaitsForQuery[chatId] = QueryType.AnsweringTrailerQuestion;
                                    _singleDataForUser[chatId] = movie.Title;
                                    await SendInfoAboutMovie(chatId, movie);
                                    break;
                                case "no":
                                case "cancel":
                                    _singleDataForUser.Remove(chatId);
                                    _botWaitsForQuery.Remove(chatId);
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
                                    _multipleDataForUser.Remove(chatId);
                                    await SendInfoAboutActor(chatId, actors[0].Name);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ListActorsAnswer(actors));
                                    _multipleDataForUser[chatId] = actors.Select(a => a.Name).ToList();
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ActorChooseMesage(actors.Count()));
                                    _botWaitsForQuery[chatId] = QueryType.SelectingPerson;
                                }
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ActorNotFoundMessage());
                                _botWaitsForQuery.Remove(chatId);
                            }
                            break;
                        }

                    case QueryType.SelectingPerson:
                        {
                            if (_multipleDataForUser.ContainsKey(chatId))
                            {
                                int chosenIndex = 1;
                                if (int.TryParse(message.Text, out chosenIndex) && chosenIndex >= 1 &&
                                  chosenIndex <= _multipleDataForUser[chatId].Count)
                                {
                                    await SendInfoAboutActor(chatId, _multipleDataForUser[chatId][chosenIndex - 1]);
                                    _multipleDataForUser.Remove(chatId);
                                    _botWaitsForQuery.Remove(chatId);
                                }
                                else if (message.Text.Trim().ToLower() == "cancel")
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                    _multipleDataForUser.Remove(chatId);
                                    _botWaitsForQuery.Remove(chatId);
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
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SomeMovieOfGenre(message.Text.Trim().ToLower()));
                                var movie = _service.GetRandomMovieByGenre(_genres[message.Text.ToLower().Trim()]);
                                await SendInfoAboutMovie(chatId, movie);
                                _botWaitsForQuery[chatId] = QueryType.AnsweringTrailerQuestion;
                                _singleDataForUser[chatId] = movie.Title;
                            }
                            else if (message.Text.Trim().ToLower() == "cancel")
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                _botWaitsForQuery.Remove(chatId);
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
                            _botWaitsForQuery[chatId] = QueryType.SearchingMovie;
                            break;
                        }
                    case "/getfromtop250":
                        {
                            string title = _service.GetRandomFrom250();
                            _singleDataForUser[chatId] = title;
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.AnswerToRandomRequest(title));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.ChooseMovieFromRandom());
                            _botWaitsForQuery.Add(chatId, QueryType.SelectingRandomMovie);
                            break;
                        }
                    case "/peoplesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.EnterActorNameInviting());
                            _botWaitsForQuery[chatId] = QueryType.SearchingPerson;
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
                            _botWaitsForQuery[chatId] = QueryType.SelectingGenre;
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



        public async Task SendInfoAboutMovie(long chatId, Movie movie)
        {

            await _client.SendPhotoAsync(chatId, movie.Poster, $"Poster to ''{movie.Title}''");
            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetMovieInfoMessage(movie));
            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.TrailerQuestion());
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
