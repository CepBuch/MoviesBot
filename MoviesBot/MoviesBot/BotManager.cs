using MoviesBot.Interfaces;
using MoviesBot.Data;
using MoviesBot.Data.MovieData.Model;
using MoviesBot.Data.TelegramBotData.Enums;
using MoviesBot.Data.TelegramBotData.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesBot
{
    class BotManager
    {
        Dictionary<string, int> _genres;
        List<long> _searchingSimilarUsers;


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
            _searchingSimilarUsers = new List<long>();
            _service = service;
            _client = client;

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
                            var movies = await _service.SearchMovies(message.Text);
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
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleAnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MoviesSearchAnswer(movies));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieChooseMesage(movies.Count()));

                                    _multipleDataForUser[chatId] = movies.Select(m => m.Title).ToList();
                                    _botWaitsForQuery[chatId] = QueryType.SelectingMovie;
                                }
                            }
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
                                var movie = await _service.SingleMovieSearch(title);
                                _multipleDataForUser.Remove(chatId);

                                //Send information about movie if it is not /searchsimilar request.
                                //If it is, we won't show full info about movies, we will show simiar movies instead.
                                if (!_searchingSimilarUsers.Contains(chatId))
                                {
                                    await SendInfoAboutMovie(chatId, movie);
                                    _singleDataForUser[chatId] = title;
                                    _botWaitsForQuery[chatId] = QueryType.AnsweringTrailerQuestion;
                                }
                                else
                                {
                                    var movies = await _service.GetSimilarMovies(movie);
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MoviesSearchAnswer(movies));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieSearchAdvice());
                                    _searchingSimilarUsers.Remove(chatId);
                                    _botWaitsForQuery.Remove(chatId);
                                }
                            }
                            else if (message.Text.Trim().ToLower() == "cancel")
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleCancelAnswer());
                                _multipleDataForUser.Remove(chatId);
                                _botWaitsForQuery.Remove(chatId);
                                _searchingSimilarUsers.Remove(chatId);
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                            }
                        }
                        break;
                    case QueryType.SelectingRandomMovie:
                        {
                            string title;
                            switch (message.Text.Trim().ToLower())
                            {
                                case "next":
                                    title = await _service.GetRandomFrom250();
                                    _singleDataForUser[chatId] = title;
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.RandomMovieAnswer(title));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.RandomMovieChooseIntro());
                                    break;
                                case "ok":
                                    _singleDataForUser.TryGetValue(chatId, out title);
                                    var movie = await _service.SingleMovieSearch(title);
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
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.WrongChoiceMessage());
                                    break;
                            }
                            break;
                        }

                    case QueryType.SearchingPerson:
                        {
                            var actors = await _service.SearchActors(message.Text);

                            if (actors != null && actors.Count != 0)
                            {
                                if (actors.Count == 1)
                                {
                                    _multipleDataForUser.Remove(chatId);
                                    await SendInfoAboutActor(chatId, actors[0].Name);
                                }
                                else
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SimpleAnswerIntroduction());
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.PeopleSearchAnswer(actors));
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.PeopleSearchChoose(actors.Count()));

                                    _multipleDataForUser[chatId] = actors.Select(a => a.Name).ToList();
                                    _botWaitsForQuery[chatId] = QueryType.SelectingPerson;
                                }
                            }
                            else
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NotFoundMessage());
                                _botWaitsForQuery.Remove(chatId);
                            }
                            break;
                        }

                    case QueryType.SelectingPerson:
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
                    case QueryType.SelectingGenre:
                        {
                            if (genresWasDownloaded && _genres.ContainsKey(message.Text.Trim().ToLower()))
                            {
                                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SomeMovieOfGenre(message.Text.Trim().ToLower()));
                                var movie = await _service.GetRandomMovieByGenre(_genres[message.Text.ToLower().Trim()]);
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

                    case QueryType.AnsweringTrailerQuestion:
                        {
                            if (message.Text.Trim().ToLower() == "yes")
                            {
                                var link = await _service.GetTrailerLinkForMovie(_singleDataForUser[chatId]);
                                if (link != null)
                                {
                                    await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.TrailerAnswer());
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
                }
            }
            else
            {
                switch (message.Text.ToLower().Trim())
                {
                    case "/start":
                    case "/info":
                    case "hi":
                    case "hello":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.InfoMessage());
                            break;
                        }
                    case "/moviesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieSearchIntroduction());
                            _botWaitsForQuery[chatId] = QueryType.SearchingMovie;
                            break;
                        }
                    case "/getfromtop250":
                        {
                            string title = await _service.GetRandomFrom250();
                            _singleDataForUser[chatId] = title;
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.RandomMovieAnswer(title));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.RandomMovieChooseIntro());
                            _botWaitsForQuery.Add(chatId, QueryType.SelectingRandomMovie);
                            break;
                        }
                    case "/peoplesearch":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.PeopleSearchIntroduction());
                            _botWaitsForQuery[chatId] = QueryType.SearchingPerson;
                            break;
                        }
                    case "/getnowplaying":
                        {
                            var movies = await _service.GetNowPlaying();
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NowPlayingIntroduction());
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NowPlayingAnswer(movies));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieSearchAdvice());
                            break;
                        }
                    case "/getbygenre":
                        {
                            _genres = await _service.GetGenres();
                            genresWasDownloaded = _genres != null && _genres.Count != 0;
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresIntroduction());
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresAnswer(_genres.Keys.ToList()));
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GenresChoose());
                            _botWaitsForQuery[chatId] = QueryType.SelectingGenre;
                            break;
                        }
                    case "/getsimilars":
                        {
                            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetSimilarIntroduction());
                            _searchingSimilarUsers.Add(chatId);
                            goto case "/moviesearch";
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

            await _client.SendChatAction(chatId, ChatAction.uploading_photo);
            await _client.SendPhotoAsync(chatId, movie.Poster, $"Poster to ''{movie.Title}''");
            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.GetMovieInfoMessage(movie));
            if(movie.ImdbLink != null)
                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieImdbApplication(movie.ImdbLink));
            await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.TrailerQuestion());
        }

        public async Task SendInfoAboutActor(long chatId, string name)
        {
            var actors = await _service.SearchActors(name);
            if (actors != null && actors.Count != 0)
            {
                await _client.SendChatAction(chatId, ChatAction.uploading_photo);
                await _client.SendPhotoAsync(chatId, actors[0].Poster);
                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.SingleSearchActorsAnswer(actors[0]));
                await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.MovieSearchAdvice());
            }
            else await _client.SendMessageAsync(MessageType.TextMessage, chatId, BotAnswers.NotFoundMessage());

        }

    }
}
