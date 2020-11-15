using System.ComponentModel.DataAnnotations;

namespace MyTvTime.Models
{
    public class MovieGenres
    {
        [Key]
        public int MovieID { get; set; }

        public Movie Movie { get; set; }
        
        [Key]
        public int GenreID { get; set; }

        public Genre Genre { get; set; }
    }
}
