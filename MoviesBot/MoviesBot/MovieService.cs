using MoviesBot.Data.MovieData.Types;
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
        public List<Movie> SearchMovies(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = client.GetStringAsync(String.Format("http://www.omdbapi.com/?s={0}&y=&plot=short&r=json", source)).Result;
                var data = JsonConvert.DeserializeObject<MovieResponse>(result);
                return data.Movies;
            }
        }

        public Movie SingleMovieSearch(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = client.GetStringAsync(String.Format("http://www.omdbapi.com/?t={0}&y=&plot=full&r=json", source)).Result;
                var data = JsonConvert.DeserializeObject<Movie>(result);
                return data;
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


    }
}
