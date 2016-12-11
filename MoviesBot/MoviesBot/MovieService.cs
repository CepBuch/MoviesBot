using MoviesBot.Data.MovieDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    class MovieService
    {
        public async Task<List<Movie>> MovieSearch(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = await client.GetStringAsync(String.Format("http://www.omdbapi.com/?s={0}&y=&plot=short&r=json", source));
                var data = JsonConvert.DeserializeObject<MovieResponse>(result);
                return data.Movies;
            }
        }

        public async Task<Movie> SingleMovieSearch(string query)
        {
            using (var client = new HttpClient())
            {
                string[] words = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string source = String.Join("+", words);
                string result = await client.GetStringAsync(String.Format("http://www.omdbapi.com/?t={0}&y=&plot=full&r=json", source));
                var data = JsonConvert.DeserializeObject<Movie>(result);
                return data;
            }
        }
    }
}
