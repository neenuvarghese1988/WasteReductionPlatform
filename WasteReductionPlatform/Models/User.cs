using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Please specify if the user is an admin.")]
        public bool IsAdmin { get; set; }

		[Required(ErrorMessage = "Please select a user type.")]
		
		public UserType? UserType { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        public string StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [RegularExpression(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$", ErrorMessage = "Invalid postal code format.")]
        public string PostalCode { get; set; }

        public ICollection<WasteLog> WasteLogs { get; set; }

    }

    public enum UserType
    {
        Residential,
        Commercial
    }
}
