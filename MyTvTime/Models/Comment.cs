using System;

namespace MyTvTime.Models
{
    public class Comment
    {
        public int ID { get; set; }

        public int MovieID { get; set; }

        public Movie Movie { get; set; }

        public int UserID { get; set; }
        
        public User User { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}
