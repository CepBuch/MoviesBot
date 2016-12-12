using MoviesBot.Data.MovieData.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public  Movie SingleMovieSearch(string query)
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

    
    }
}
