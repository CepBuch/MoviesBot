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
        public static string InfoMessage()
        {
            return String.Format(@"Hi! This bot is responsible for different operations with movies:
List of commands:
/info - Shows information about this bot (like this message)
/moviesearch - Provides search by film title/piece of title
/peoplesearch - Search actors/directors by the name or piece of name
/getbygenre - Returns random movie by a particular genre
/getnowplaying - Returns movies, which are on screens now
/getfromtop250 - Returns random movie from IMDB top-250 best movies
/getsimilars - Returns movies which are similar to some movie");
        }




        public static string MovieSearchIntroduction()
    => $"Please, enter a name of the movie you are looking for";

        public static string MovieChooseMesage(int to, int from = 1)
=> $"Please, choose the exact movie to get more detailed information (enter its number from {from} to {to}) " +
    "or send ''Cancel'' if there is no suitable movie in the list above or you just don't want to see detailed info";


        public static string MoviesSearchAnswer(List<Movie> movies)
        {
            StringBuilder sb = new StringBuilder();
            int i = 1;

            foreach (var movie in movies)
                sb.AppendLine($"{i++}. {movie.Title} ({movie.Year})");

            return sb.ToString();
        }



        public static string RandomMovieAnswer(string title)
            => $"There is a random movie from IMDB top 250 movies for you:\n ''{title}''";

        public static string RandomMovieChooseIntro()
            => @"- enter ''ok'' to view info about this movie
- enter ''next'' to get another movie
- enter ''cancel to cancel this operation";




        public static string PeopleSearchIntroduction()
        => $"Please, enter a name of the person you are looking for";

        public static string PeopleSearchAnswer(List<Actor> actors)
        {
            StringBuilder sb = new StringBuilder();
            int n = 1;
            foreach (var actor in actors)
            {
                sb.AppendLine($"{n++}. {actor.Name}");
            }
            return sb.ToString();
        }

        public static string PeopleSearchChoose(int to, int from = 1)
            => $"Please, choose the exact actor to get more detailed information (enter his number from {from} to {to}) " +
            "or send ''Cancel'' if there is no suitable actor in the list above or you just don't want to see detailed info";


        public static string SingleSearchActorsAnswer(Actor actor)
        {
            string actorName = $"Sending most popular movies of {actor.Name}: \n";
            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (var movie in actor.Movies)
            {
                sb.AppendLine($"\n{i++}. {movie.Title}");
                sb.AppendLine($"Plot: {movie.Description}");
            }
            return actorName + sb.ToString();
        }



        public static string TrailerQuestion()
            => "Would you like to watch a trailer on chosen movie? (yes/no) ";
        public static string TrailerAnswer()
=> @"Here is the trailer for this movie. Note, that this function doesn't work perfectly and trailer may be unsuitable. ";

        public static string TrailerWasNotFound()
            => "Unfortunately, I couldn't find any trailers for this movie :( ";



        public static string NowPlayingIntroduction()
           => "Here are the movies that are now on screens in theatres: ";


        public static string NowPlayingAnswer(List<Movie> movies)
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

        public static string GenresChoose()
            => $"Please enter the name of genre to get random movie of this or ''Cancel'' to cancel the operation";

        public static string SomeMovieOfGenre(string genre)
            => $"I find random movie of {genre} for you";



        public static string GetSimilarIntroduction()
            => "Ok, first we should find a movie to which you want to see similars.";



        public static string MovieSearchAdvice()
            => "You can always use /moviesearch command to see more information about one of these movies ";



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
        public static string MovieImdbApplication(string link)
            => $"You can see more information about this movie on IMDB official website\n{link}";




        public static string NotFoundMessage()
        => "Unfortunately, I couldn't find anything for you. Please, make sure your request is correct and try again!";





        public static string WrongQueryMessage()
            => "I can't understand your query. Write down /info to get all possible commands";

        public static string WrongChoiceMessage()
        => "You are asked to make a choice to get more information or put ''Cancel''. Please, try again!";

        public static string SimpleAnswerIntroduction()
        => "This is all I can offer you:";

        public static string SimpleCancelAnswer()
            => "Ok I've got you, operation is cancelled, let's try /info to see what else I can do for you!";

        public static string NotFoundSimilar()
            => "Unfortunately, I can't find similars to these film (maybe it's unpopular?). Please try again!";


    }
}
