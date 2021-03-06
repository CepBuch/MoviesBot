﻿using MoviesBot.Data.MovieData.Model;
using MoviesBot.Data.MovieData.omdbDTO;
using MoviesBot.Data.MovieData.themoviedbDTO;
using MoviesBot.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    class MovieService : IMovieService
    {
        const string _token = "b5384e21c2615e7fdff81bf8bd5b3a82";
        public async Task<List<Movie>> SearchMovies(string query)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string source = String.Join("+", words);
                    string result = await client.GetStringAsync(String.Format("http://www.omdbapi.com/?s={0}&y=&plot=short&r=json", source));
                    var data = JsonConvert.DeserializeObject<omdbResponse>(result);

                    if (data.Movies != null && data.Movies.Count != 0)
                    {
                        return data.Movies.Select(m => ConvertToMovie(m)).ToList();
                    }
                    else return null;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public async Task<Movie> SingleMovieSearch(string query)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string source = String.Join("+", words);
                    string result = await client.GetStringAsync(String.Format("http://www.omdbapi.com/?t={0}&y=&plot=full&r=json", source));
                    var movie = JsonConvert.DeserializeObject<omdbMovie>(result);
                    return ConvertToMovie(movie);
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }

        public string GetRandomFrom250()
        {
            List<string> top250Movies = new List<string>();
            using (StreamReader streamReader = new StreamReader
                ("../../../MoviesBot.Data/MovieData/Files/top250.txt", Encoding.UTF8))
            {
                while (!streamReader.EndOfStream)
                {
                    string movie = streamReader.ReadLine();
                    top250Movies.Add(movie.Trim());
                }
            }
            Random rnd = new Random();
            return top250Movies[rnd.Next(0, top250Movies.Count)];
        }


        public async Task<List<Actor>> SearchActors(string query)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string result = await client.GetStringAsync($"https://api.themoviedb.org/3/search/person?api_key={_token}&include_adult=False&query={query}");
                    var response = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbActor>>(result);
                    if (response.Results != null && response.Results.Count != 0)
                    {
                        return response.Results.Select(actor => new Actor
                        {
                            Name = actor.Name,
                            Movies = actor.Movies.Select(movie => new Movie
                            {
                                Title = movie.Title,
                                Description = movie.Plot
                            }).ToList(),
                            Poster = actor.Poster
                        }).ToList();
                    }
                    else return null;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<List<Movie>> GetNowPlaying()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string result = await client.GetStringAsync($"https://api.themoviedb.org/3/movie/now_playing?sort_by=popularity.desc&api_key={_token}");
                    var response = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(result);
                    if (response.Results != null && response.Results.Count != 0)
                    {
                        return response.Results.Select(movie =>
                                  new Movie
                                  {
                                      Title = movie.Title,
                                      Description = movie.Plot,
                                      ImdbRating = movie.VoteAverage
                                  }).Take(10).ToList();
                    }
                    else return null;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<Dictionary<string, int>> GetGenres()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string result = await client.GetStringAsync($"https://api.themoviedb.org/3/genre/movie/list?api_key={_token}");
                    var response = JsonConvert.DeserializeObject<themoviedbGenreResponse>(result);
                    var genres = response.Genres.ToDictionary(genre => genre.Name.ToLower().Trim(), genre => genre.Id);
                    genres.Remove("tv movie");
                    genres.Remove("documentary");
                    return genres;
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<Movie> GetRandomMovieByGenre(int genreId)
        {
            string url =
                $"https://api.themoviedb.org/3/discover/movie?with_genres={genreId}&vote_average.gte=6&vote_count.gte=500&api_key={_token}";
            try
            {
                using (var client = new HttpClient())
                {
                    string result1 = await client.GetStringAsync(url);
                    var response1 = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(result1);

                    Random rnd = new Random();
                    var page = rnd.Next(1, response1.TotalPages + 1);
                    string result2 = await client.GetStringAsync(url + $"&page={page}");

                    var response2 = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(result2);
                    var movies = response2.Results.Select(movie => movie.Title).ToList();
                    return await SingleMovieSearch(movies[rnd.Next(0, movies.Count)]);
                }
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<string> GetTrailerLinkForMovie(string title)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var findMoviesUrl = $"https://api.themoviedb.org/3/search/movie?api_key={_token}&language=en-US&query={title}";
                    var moviesResponse = await client.GetStringAsync(findMoviesUrl);
                    var movies = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(moviesResponse).Results;

                    string movieId;
                    if (movies != null && movies.Count != 0)
                    {
                        var movieWithId = movies.FirstOrDefault(m => !String.IsNullOrWhiteSpace(m.TMDBId));

                        if (movieWithId != null)
                        {
                            movieId = movieWithId.TMDBId;
                            var url = $"https://api.themoviedb.org/3/movie/{movieId}/videos?api_key={_token}";
                            var trailerResponse = await client.GetStringAsync(url);

                            var videos = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbVideo>>(trailerResponse).Results;
                            var trailer = videos.FirstOrDefault(t => t.Type.ToLower().Trim() == "trailer");
                            return trailer != null ? $"https://www.youtube.com/watch?v={trailer.YouTubeKey}" : null;
                        }
                    }
                }
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public async Task<List<Movie>> GetSimilarMovies(Movie movie)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var findMoviesUrl = $"https://api.themoviedb.org/3/search/movie?api_key={_token}&language=en-US&query={movie.Title}";
                    var moviesResponse = await client.GetStringAsync(findMoviesUrl);
                    var movies = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(moviesResponse).Results;

                    string movieId;
                    if (movies != null && movies.Count != 0)
                    {
                        var movieWithId = movies.FirstOrDefault(m => !String.IsNullOrWhiteSpace(m.TMDBId));

                        if (movieWithId != null)
                        {
                            movieId = movieWithId.TMDBId;
                            var url = $"https://api.themoviedb.org/3/movie/{movieId}/similar_movies?api_key={_token}";
                            var similarResponse = await client.GetStringAsync(url);
                            var similar = JsonConvert.DeserializeObject<themoviedbResponse<themoviedbMovie>>(similarResponse).Results;

                            return similar.Select(m => new Movie
                            {
                                Title = m.Title,
                                Year = m.Release,
                                Description = m.Plot,
                                ImdbRating = m.VoteAverage

                            }).ToList();
                        }
                    }
                }
                return null;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }


        public static Movie ConvertToMovie(omdbMovie movie)
        {
            return new Movie
            {

                Title = movie.Title,
                Year = movie.Year,
                Poster = movie.Poster,
                Runtime = movie.Runtime,
                Genre = movie.Genre,
                Writer = movie.Writer,
                Actors = movie.Actors,
                Description = movie.Plot,
                Director = movie.Director,
                Country = movie.Country,
                ImdbRating = movie.ImdbRating,
                ImdbLink = movie.ImdbID
            };
        }
    }
}
