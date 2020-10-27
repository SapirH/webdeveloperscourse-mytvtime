using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MyTvTime.Models
{
	public class User
	{
		public int Id { get; set; }
		public string username { get; set; }
		public string password { get; set; }
		public string email { get; set; }

		[DataType(DataType.Date)]
		public DateTime birthdate { get; set; }
		public string sex { get; set; }
		public string country { get; set; }
		public string language { get; set; }
		public bool isAdmin { get; set; }
	}
}
