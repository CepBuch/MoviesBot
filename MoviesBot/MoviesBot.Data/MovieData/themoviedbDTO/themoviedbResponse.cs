using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    class themoviedbResponse
    {
        [JsonProperty("results")]
        public List<themoviedbActor> Actors { get; set; }
    }
}
