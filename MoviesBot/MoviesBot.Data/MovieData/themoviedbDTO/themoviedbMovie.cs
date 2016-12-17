using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.themoviedbDTO
{
    public class themoviedbMovie
    {
        [JsonProperty("original_title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Plot { get; set; }

        [JsonProperty("vote_average")]
        public string VoteAverage { get; set; }

        private string _release;

        [JsonProperty("release_date")]
        public string Release
        {
            get { return _release; }
            set
            {
                _release = value != null && value.Split('-').Length == 3 ? value.Split('-')[0] : value;
            }
        }

    }
}
