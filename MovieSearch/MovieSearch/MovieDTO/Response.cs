using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieSearch.MovieDTO
{
    class Response
    {
        [JsonProperty(PropertyName = "Search")]
        public List<Movie> Movies { get; set; }
 
    }
}
