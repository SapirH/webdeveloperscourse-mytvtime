using System.Collections.Generic;

namespace MyTvTime.Models
{
    public class Genre
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ICollection<MovieGenres> Movies { get; set; }
    }
}
