using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.omdbDTO
{
    public class omdbMovie
    {
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "Year")]
        public string Year { get; set; }
        [JsonProperty(PropertyName = "Poster")]
        public string Poster { get; set; }
        [JsonProperty(PropertyName = "imdbID")]
        public string ImdbID { get; set; }
        [JsonProperty(PropertyName = "Runtime")]
        public string Runtime { get; set; }
        [JsonProperty(PropertyName = "Genre")]
        public string Genre { get; set; }
        [JsonProperty(PropertyName = "Director")]
        public string Director { get; set; }
        [JsonProperty(PropertyName = "Writer")]
        public string Writer { get; set; }
        [JsonProperty(PropertyName = "Actors")]
        public string Actors { get; set; }
        [JsonProperty(PropertyName = "Plot")]
        public string Plot { get; set; }

        [JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }
        [JsonProperty(PropertyName = "imdbRating")]
        public string ImdbRating { get; set; }

    }
}
