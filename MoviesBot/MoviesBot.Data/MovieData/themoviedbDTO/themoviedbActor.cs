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
        [JsonProperty("profile_path")]
        public string Poster { get; set; }

    }
}
