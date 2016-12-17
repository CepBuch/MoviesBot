using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    public class themoviedbResponse<T>
    {
        [JsonProperty("results")]
        public List<T> Results { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
    }
}
