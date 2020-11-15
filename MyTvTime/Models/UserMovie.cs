using System.ComponentModel.DataAnnotations;

namespace MyTvTime.Models
{
    public class UserMovie
	{
		[Key]
		public int UserId { get; set; }

		[Key]
		public int MovieId { get; set; }
		public virtual User User { get; set; }
		public virtual Movie Movie { get; set; }
	}
}
