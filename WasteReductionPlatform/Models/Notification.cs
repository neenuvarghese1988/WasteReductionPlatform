using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WasteReductionPlatform.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        public DateTime? ScheduledPickupDate { get; set; }
    }

    public enum NotificationType
    {
        HighWasteProduction,
        MissedPickup
    }
}
