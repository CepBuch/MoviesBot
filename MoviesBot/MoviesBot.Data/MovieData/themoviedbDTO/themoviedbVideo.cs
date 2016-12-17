using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    public class themoviedbVideo
    {
        [JsonProperty("key")]
        public string YouTubeKey { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
