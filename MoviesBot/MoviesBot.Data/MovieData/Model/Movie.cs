using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data.MovieData.Model
{
    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Poster { get; set; }
        public string ImdbID { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string ImdbRating { get; set; }
    }

    public class Actor
    {
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }
        public string Poster { get; set; }
    }
}
