using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
    public class UserPickupRequest
    {
        public int Id { get; set; }

        [Required]
        public int PickupScheduleId { get; set; }
        public PickupSchedule PickupSchedule { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime RequestedDate { get; set; }

        public bool IsConfirmed { get; set; }
    }
}