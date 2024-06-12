using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
        public class PickupSchedule
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

        public bool IsConfirmed { get; set; } 
    }

        public enum WasteType
        {
            BlueBox, // Recyclables
            GreenCart, // Organic Waste
            Garbage, // Non-Recyclable Waste
            Bulk, // Large Items
            Hazardous // Hazardous Waste
        }
    }
