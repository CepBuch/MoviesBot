using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    class themoviedbMovie
    {
        [JsonProperty("original_title")]
        public string Title { get; set; }
        [JsonProperty("overview")]
        public string Plot { get; set; }
    }
}
