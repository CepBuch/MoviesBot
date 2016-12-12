using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.Types
{
    public class MovieResponse
    {
        [JsonProperty(PropertyName = "Search")]
        public List<Movie> Movies { get; set; }
        [JsonProperty(PropertyName = "totalResults")]
        public int TotalResults { get; set; }

    }
}
