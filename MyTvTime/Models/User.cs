using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyTvTime.NamespaceAnnotations;

namespace MyTvTime.Models
{
    public class User
	{
		public User() : base()
		{
			this.Watchlist = new HashSet<UserMovie>();
		}

		public int Id { get; set; }

		[Required(ErrorMessage = "Username is required")]
		public string username { get; set; }

		[Required(ErrorMessage = "password is required")]
		[RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,10}$",
			ErrorMessage = "Password must be at least 6 characters, no more than 10 characters, and must include at least one upper case letter, one lower case letter, and one numeric digit.")]
		public string password { get; set; }

		[Required(ErrorMessage = "email is required")]
		[EmailAddress(ErrorMessage = "not using email format")]
		public string email { get; set; }

		[DataType(DataType.Date)]
		[MinimumAge(13, "you must be above 13")]
		public DateTime birthdate { get; set; }

		[Required(ErrorMessage = "You have to choose your gender")]
		public string sex { get; set; }

		[Required(ErrorMessage = "You have to choose your country")]
		public string country { get; set; }

		[Required(ErrorMessage = "You have to choose your language")]
		public string language { get; set; }
		public bool isAdmin { get; set; }

		
		public ICollection<UserMovie> Watchlist { get; set; }


		

		public ICollection<Comment> Comments { get; set; }
	}
}
