using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyTvTime.Models
{
    public class Movie
    {
        [Key]
        public int ID { get; set; }

        public string IMDBID { get; set; }

        public string Name { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string Language { get; set; }

        public int Runtime { get; set; }

        public ICollection<MovieGenres> Genres { get; set; }

        public string Description { get; set; }

        [DisplayName("Poster")]
        public string ImageURL { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}
