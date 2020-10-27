using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTvTime.Models
{
    public class MovieResult
    {
        public string title { get; set; }
        public int year { get; set; }
        public string imdb_id { get; set; }
    }

    public class MovieResultRoot
    {
        public List<MovieResult> movie_results { get; set; }
        public int search_results { get; set; }
        public string status { get; set; }
        public string status_message { get; set; }
    }

    public class IMDBMovieDetails
    {
        public string title { get; set; }
        public string description { get; set; }
        public string year { get; set; }
        public string release_date { get; set; }
        public string imdb_id { get; set; }
        public string imdb_rating { get; set; }
        public string vote_count { get; set; }
        public string popularity { get; set; }
        public string youtube_trailer_key { get; set; }
        public string rated { get; set; }
        public string runtime { get; set; }
        public List<string> genres { get; set; }
        public List<string> stars { get; set; }
        public List<string> directors { get; set; }
        public List<string> countries { get; set; }
        public List<string> language { get; set; }
        public string status { get; set; }
        public string status_message { get; set; }
    }

    public class ImageResult
    {
        public string title { get; set; }
        public string IMDB { get; set; }
        public string poster { get; set; }
        public string fanart { get; set; }
        public string status { get; set; }
        public string status_message { get; set; }
    }
}
