using MoviesBot.Data.MovieDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data
{
    public static class BotAnswerMessages
    {
        public static string GetInfoMessage()
        {
            return String.Format(@"This bot is responsible for searching movies:
List of commands:
/info - Shows information about this bot (like this message)
/moviesearch - Provides search by film title/piece of title
/getfromtop250 - Returns random movie from IMDB top-250 best movies
/getbygenre - Returns random movie by genre");
        }

        public static string GetListOfMoviesMessage(List<Movie> movies)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var movie in movies)
            {
                sb.AppendLine($"{i++}. {movie.Title} ({movie.Year})");
            }
            return sb.ToString();
        }


        public static string GetMovieInfoMessage(Movie movie)
        {
            return String.Format(@"{0}  ({1}) 
Runtime: {2}
Genre: {3}
Country: {4}
Director: {5}
Actors: {6}
Description: {7}

IMDB rating: {8}"
, movie.Title, movie.Year, movie.Runtime, movie.Genre, movie.Country,
movie.Director, movie.Actors, movie.Plot, movie.Plot, movie.ImdbID);

        }

        public static string MovieNotFoundMessage()
        => "Unfortunately, I couldn't find anything for you. Please, make sure your request is correct";

        public static string MovieChooseMesage(int to, int from = 1)
        => $"Please choose the exact movie to get more detailed information (from {from} to {to}) "+
            "or send ''No'' if there is no suitable movie for in the list above";


    }
}
