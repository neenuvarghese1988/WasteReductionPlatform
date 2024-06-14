using System.ComponentModel.DataAnnotations;

namespace WasteReductionPlatform.Models
{
    public class EducationalResource
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string URL { get; set; }

        public string Category { get; set; }

        public string ThumbnailURL { get; set; }

        public string VideoURL { get; set; }
    }
}