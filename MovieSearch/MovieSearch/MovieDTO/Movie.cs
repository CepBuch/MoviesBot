using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieSearch.MovieDTO
{
    class Movie
    {
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "Year")]
        public int Year { get; set; }
        [JsonProperty(PropertyName = "Released")]
        public string Released { get; set; }
        [JsonProperty(PropertyName = "Runtime")]
        public string Runtime { get; set; }
        [JsonProperty(PropertyName = "Genre")]
        public string Genre { get; set; }
        [JsonProperty(PropertyName = "Director")]
        public string Director { get; set; }
        [JsonProperty(PropertyName = "Actors")]
        public string Actors { get; set; }
        [JsonProperty(PropertyName = "Plot")]
        public string Plot { get; set; }

    }
}
