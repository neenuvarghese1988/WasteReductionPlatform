using System;
using System.ComponentModel.DataAnnotations;
using WasteReductionPlatform.Models;

namespace WasteReductionPlatform.ViewModels
{
    public class WasteLogViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please provide a valid date.")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "Weight must be a positive number.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Please enter a valid weight in kilograms.")]
        public double? Weight { get; set; }

        [Required(ErrorMessage = "Please select a type of waste.")]
        public string WasteType { get; set; }

        public List<string> WasteTypes { get; set; } = new List<string>();

        //  [Required(ErrorMessage = "User type is required.")]
        public UserType? UserType { get; set; }
	}
}
