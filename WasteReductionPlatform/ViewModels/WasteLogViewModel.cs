using System;
using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
    public class WasteLogViewModel
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public WasteType WasteType { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
}
