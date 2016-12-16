using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    public class themoviedbActor
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("known_for")]
        public List<themoviedbMovie> Movies { get; set; }
        private string _poster;
        [JsonProperty("profile_path")]
        public string Poster
        {
            get { return _poster; }
            set
            {
                _poster = "https://image.tmdb.org/t/p/w300" + value;
            }
        }


    }
}
