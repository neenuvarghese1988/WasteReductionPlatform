using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
    public class RegisterViewModel
    {
        //[Required(ErrorMessage = "Email is required.")]
        //[EmailAddress(ErrorMessage = "Invalid email address.")]
      //  public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

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

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
        [Required]
        public string Role { get; set; } // Add role selection


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role != "Admin" && !UserType.HasValue)
            {
                yield return new ValidationResult("User Type is required for non-admin users.", new[] { "UserType" });
            }
        }
    }
}
