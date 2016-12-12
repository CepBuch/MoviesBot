using MoviesBot.Data.MovieData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot
{
    interface IMovieService
    {
        List<Movie> SearchMovies(string query);
        Movie SingleMovieSearch(string query);
    }
}
