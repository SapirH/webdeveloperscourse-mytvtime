using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
