using MoviesBot.Data.MovieData.Model;
using MoviesBot.Data.MovieData.omdbDTO;
using MoviesBot.Data.MovieData.themoviedbDTO;
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
        public List<Movie> SearchMovies(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = client.GetStringAsync(String.Format("http://www.omdbapi.com/?s={0}&y=&plot=short&r=json", source)).Result;
                var data = JsonConvert.DeserializeObject<omdbResponse>(result);

                if (data.Movies != null && data.Movies.Count != 0)
                {
                    return data.Movies.Select(m => new Movie
                    {
                        Title = m.Title,
                        Year = m.Year,
                        Poster = m.Poster,
                        ImdbID = m.ImdbID,
                        Runtime = m.Runtime,
                        Genre = m.Genre,
                        Writer = m.Writer,
                        Actors = m.Actors,
                        Description = m.Plot,
                        Director = m.Director,
                        Country = m.Country,
                        ImdbRating = m.ImdbRating
                    }
         ).ToList();
                }
                else return null;
            }
        }

        public Movie SingleMovieSearch(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = client.GetStringAsync(String.Format("http://www.omdbapi.com/?t={0}&y=&plot=full&r=json", source)).Result;
                var movie = JsonConvert.DeserializeObject<omdbMovie>(result);
                return new Movie
                {

                    Title = movie.Title,
                    Year = movie.Year,
                    Poster = movie.Poster,
                    ImdbID = movie.ImdbID,
                    Runtime = movie.Runtime,
                    Genre = movie.Genre,
                    Writer = movie.Writer,
                    Actors = movie.Actors,
                    Description = movie.Plot,
                    Director = movie.Director,
                    Country = movie.Country,
                    ImdbRating = movie.ImdbRating
                };
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

        public List<Actor> SearchActors(string query)
        {

            using (var client = new HttpClient())
            {
                string result = client.GetStringAsync($"https://api.themoviedb.org/3/search/person?api_key={_token}&include_adult=False&query={query}").Result;
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
                    }
                                       ).ToList();
                }
                else return null;
            }
        }

        public List<Movie> GetNowPlaying()
        {
            using (var client = new HttpClient())
            {
                string result = client.GetStringAsync($"https://api.themoviedb.org/3/movie/now_playing?sort_by=popularity.desc&api_key={_token}").Result;
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

        public Dictionary<string, int> GetGenres()
        {
            using (var client = new HttpClient())
            {
                string result = client.GetStringAsync($"https://api.themoviedb.org/3/genre/movie/list?api_key={_token}").Result;
                var response = JsonConvert.DeserializeObject<themoviedbGenreResponse>(result);
                return response.Genres.ToDictionary(genre => genre.Name.ToLower().Trim(), genre => genre.Id);
            }
        }
    }
}
