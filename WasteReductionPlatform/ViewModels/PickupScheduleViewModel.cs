using WasteReductionPlatform.Models;
using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.ViewModels
{
    public class PickupScheduleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pickup date is required.")]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "Please select a pickup type.")]
        public string PickupType { get; set; }

        [Required(ErrorMessage = "Please select the user type.")]
        public UserType UserType { get; set; }

        [Required(ErrorMessage = "Please provide the city for pickup.")]
        public string Area { get; set; }

        public bool IsConfirmed { get; set; }
    }
}