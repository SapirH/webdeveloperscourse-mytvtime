using System;
using System.Collections.Generic;

namespace MyTvTime.Models
{
    // movies from IMDB
    public class SearchMovieResult
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string Poster { get; set; }
    }

    public class SearchMovieResultRoot
    {
        public List<SearchMovieResult> Search { get; set; }
        public string totalResults { get; set; }
    }

    public class IMDBMovieDetails
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public string Metascore { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string DVD { get; set; }
        public string BoxOffice { get; set; }
        public string Production { get; set; }
        public string Website { get; set; }
        public string Response { get; set; }

        public string[] GetGenresArray()
        {
            if (!string.IsNullOrWhiteSpace(Genre))
            {
                string[] genres = Genre.Split(",", StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < genres.Length; i++)
                    genres[i] = genres[i].Trim();
                return genres;
            }

            return null;
        }

        public string GetFirstLanguage()
        {
            if (!string.IsNullOrWhiteSpace(Language))
            {
                string[] langs = Language.Split(",", StringSplitOptions.RemoveEmptyEntries);
                return langs[0].Trim();
            }

            return null;
        }

        public int GetRuntimeAsInt()
        {
            int runtime = int.TryParse(Runtime, out runtime) ? runtime : 0;
            if (runtime == 0)
                int.TryParse(Runtime.Split(" ")[0], out runtime);
            return runtime;
        }

        public DateTime GetReleaseDateTime()
        {
            DateTime dateTime = DateTime.TryParse(Released, out dateTime) ? dateTime : default;
            return dateTime;
        }
    }
}
