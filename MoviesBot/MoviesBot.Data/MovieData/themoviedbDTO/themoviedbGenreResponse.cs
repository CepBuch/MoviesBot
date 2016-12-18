using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    public class themoviedbGenreResponse
    {
        [JsonProperty("genres")]
        public List<themoviedbGenre> Genres { get; set; }
    }
}
