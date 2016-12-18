using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.omdbDTO
{
    public class omdbResponse
    {
        [JsonProperty(PropertyName = "Search")]
        public List<omdbMovie> Movies { get; set; }
        [JsonProperty(PropertyName = "totalResults")]
        public int TotalResults { get; set; }

    }
}
