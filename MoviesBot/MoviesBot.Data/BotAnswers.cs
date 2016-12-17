using MoviesBot.Data.MovieData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesBot.Data
{
    public static class BotAnswers
    {
        public static string GetInfoMessage()
        {
            return String.Format(@"Hi! This bot is responsible for searching movies:
List of commands:
/info - Shows information about this bot (like this message)
/moviesearch - Provides search by film title/piece of title
/getfromtop250 - Returns random movie from IMDB top-250 best movies
/getbygenre - Returns random movie by genre
/peoplesearch - Search actors by the name/piece of name");
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
movie.Director, movie.Actors, movie.Description, movie.ImdbRating);

        }

        public static string ChooseMovieFromRandom()
        => @"There is a random movie from IMDB top 250 movie:
- enter ''Ok'' to view info about this movie
- enter ''Next'' to get another movie
- enter ''Cancel to cancel this opearation";


        public static string NotFoundMessage()
        => "Unfortunately, I couldn't find anything for you. Please, make sure your request is correct";

        public static string MovieChooseMesage(int to, int from = 1)
        => $"Please, choose the exact movie to get more detailed information (from {from} to {to}) " +
            "or send ''Cancel'' if there is no suitable movie in the list above or you just don't wanna see detailed info";

        public static string EnterMovieTitleInviting()
            => $"Please, put  name of the movie you are looking for";

        public static string WrongQueryMessage()
            => "I can't understand your query. Write down /info to get all possible commands";

        public static string WrongChoiceMessage()
        => "You are asked make a choice to get more information about it or put ''Cancel''";

        public static string AnswerIntroduction()
        => "This is all I can offer you:";

        public static string SimpleCancelAnswer()
            => "OK, I've got you, operation is cancelled, let's try /info to see what else I can do for you";

        public static string AnswerToRandomRequest(string title)
       => $"The movie for you is ''{title}''";

        public static string AnswerTrailer()
        => @"Here is trailer for this movie. Note, that this function doesn't work perfectly good and trailer may be unsuitable";

        public static string SingleSearchActorsAnswer(Actor actor)
        {
            string actorName = $"Actor: {actor.Name}";
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var movie in actor.Movies)
            {
                sb.AppendLine($"\n{i++}. {movie.Title}");
                sb.AppendLine($"Plot: {movie.Description}");
            }
            return actorName + sb.ToString();
        }

        public static string ListActorsAnswer(List<Actor> actors)
        {
            StringBuilder sb = new StringBuilder();
            int n = 1;
            foreach (var actor in actors)
            {
                sb.AppendLine($"{n++}. {actor.Name}");
            }
            return sb.ToString();
        }

        public static string EnterActorNameInviting()
           => $"Please, put  name of the actor you are looking for";

        public static string ActorChooseMesage(int to, int from = 1)
        => $"Please, choose the exact actor to get more detailed information (from {from} to {to}) " +
            "or send ''Cancel'' if there is no suitable actor in the list above or you just don't wanna see detailed info";

        public static string ActorNotFoundMessage()
        => "Unfortunately, I couldn't find anything for you. Please, make sure your request is correct";

        public static string AnswerToNowPlaying(List<Movie> movies)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var movie in movies)
            {
                sb.AppendLine($"{i++}. {movie.Title}");
                sb.AppendLine($"Plot: {movie.Description} \n");
            }
            return sb.ToString();
        }

        public static string IntroductionToNowPlaying()
            => "Here are the movies that are now on screens in theatres: ";

        public static string GenresIntroduction()
            => "Here are all the genres I've already known: ";

        public static string GenresAnswer(List<string> genres)
            {
            StringBuilder sb = new StringBuilder();
            foreach (var genre in genres)
            {
                sb.AppendLine($"- {genre}");
            }
            return sb.ToString();
        }


        public static string GenresChooseMessage()
            => $"Please enter the name of genre to get random movie of this or ''Cancel'' to cancel the operation";

        


    }
}
