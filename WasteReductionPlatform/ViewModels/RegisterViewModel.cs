using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
	public class RegisterViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

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
	}
}