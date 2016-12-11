using MovieSearch.MovieDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MovieSearch
{
    class Repository
    {
        public async Task<List<Movie>> GetResults(string movie)
        {
            string updatedMovie = movie.Replace(' ', '+');
            using (var client = new HttpClient())
            {
                var result =  await client.GetStringAsync(String.Format("http://www.omdbapi.com/?s={0}&y=&plot=full&r=json", updatedMovie));
                var data = JsonConvert.DeserializeObject<Response>(result);
              
                return data.Movies; 
            }

        }
        public async Task<Movie> GetProperResults(string movie)
        {
            string updatedMovie = movie.Replace(' ', '+');
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync(String.Format("http://www.omdbapi.com/?t={0}&y=&plot=full&r=json", updatedMovie));
                var data = JsonConvert.DeserializeObject<Movie>(result);

                return data;
            }

        }
    }
}
