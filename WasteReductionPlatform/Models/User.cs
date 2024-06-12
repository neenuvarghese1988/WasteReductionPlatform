using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace WasteReductionPlatform.Models
{
	public class User:IdentityUser
	{
		[Required]
		public bool IsAdmin { get; set; }

		[Required]
		public UserType UserType { get; set; }

		[Required]
		public string StreetAddress { get; set; }

		[Required]
		public string City { get; set; }

		[Required]
		public string Province { get; set; }

		[Required]
		public string PostalCode { get; set; }

		public ICollection<WasteLog> WasteLogs { get; set; }
	}

	public enum UserType
	{
		Residential,
		Commercial
	}
}

