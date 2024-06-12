using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
    public class UserProfileViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

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
