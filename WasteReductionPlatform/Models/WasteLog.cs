using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
    public class WasteLog
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public string WasteType { get; set; }

        [Required]
        public UserType? UserType { get; set; }
    }
}