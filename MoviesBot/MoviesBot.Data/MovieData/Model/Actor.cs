using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.Model
{
    public class Actor
    {
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }
        public string Poster { get; set; }
    }
}
