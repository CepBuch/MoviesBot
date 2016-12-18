using MoviesBot.Data.MovieData.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesBot
{
    interface IMovieService
    {
        Task<List<Movie>> SearchMovies(string query);
        Task<Movie> SingleMovieSearch(string query);
        Task<string> GetRandomFrom250();
        Task<List<Actor>> SearchActors(string query);
        Task<List<Movie>> GetNowPlaying();
        Task<Dictionary<string, int>> GetGenres();
        Task<Movie> GetRandomMovieByGenre(int genreId);

        Task<string> GetTrailerLinkForMovie(string title);

        Task<List<Movie>> GetSimilarMovies(Movie movie);
    }
}
