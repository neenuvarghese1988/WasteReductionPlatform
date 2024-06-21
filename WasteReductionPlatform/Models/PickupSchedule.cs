using System;
using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
    public class PickupSchedule
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The pickup date is required.")]
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
