using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
	public class UserProfileViewModel
	{
		[DisplayName("Username")]
		public string Email { get; set; }

		
		public UserType? UserType { get; set; } // Nullable

		[Required(ErrorMessage = "Street address is required.")]
		public string StreetAddress { get; set; }

		[Required(ErrorMessage = "City is required.")]
		public string City { get; set; }

		[Required(ErrorMessage = "Province is required.")]
		public string Province { get; set; }

		[Required(ErrorMessage = "Postal code is required.")]
		public string PostalCode { get; set; }
	}
}
