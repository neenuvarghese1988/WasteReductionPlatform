using WasteReductionPlatform.Models;
using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.ViewModels
{
    public class PickupScheduleViewModel
    {
        public int Id { get; set; }

        [Required]
        public DateTime PickupDate { get; set; }

        [Required]
        public WasteType PickupType { get; set; }

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public string Area { get; set; }
    }
}