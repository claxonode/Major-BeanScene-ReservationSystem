using Major_BeanScene_ReservationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings
{
    public class Create : IValidatableObject
    {
        [DisplayName("Sitting capacity")]
        [Required]
        [Range(1, 40)]
        public int Capacity { get; set; }
        [DisplayName("Sitting name")]
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [DisplayName("Is the sitting closed? Tick for yes")]
        [Required]
        public bool IsClosed { get; set; }
        [DisplayName("Start Time")]
        ////[Remote(action: "ValidateDateTime", controller: "Sittings", AdditionalFields = nameof(EndTime))]
        //[NonOverlappingDateTime]
        [Remote(action: "ValidateDateTime", controller: "Sittings", AdditionalFields = "EndTime")]
        [Required]
        public DateTime StartTime { get; set; }
        [DisplayName("End Time")]
        //[NonOverlappingDateTime]
        ////[Remote(action: "ValidateDateTime", controller: "Sittings", AdditionalFields = nameof(StartTime))]
        [Remote(action: "ValidateDateTime", controller: "Sittings", AdditionalFields = "StartTime")]
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        [DisplayName("What restaurant is the sitting set for?")]
        public int RestaurantId { get; set; }
        public SelectList? Restaurants { get; set; }
        [Required]
        [DisplayName("What is the sitting type?")]
        public int SittingTypeId { get; set; }
        public SelectList? SittingTypes { get; set; }

        public string? OptionsForRepeat { get; set; }
        [DisplayName("Choose days")]
        public List<DayOfWeek>? SelectedDaysOfWeek { get; set; }
        
        public List<SelectListItem>? RepeatOptions { get; set; }
        public int RepeatOptionId { get; set; }

        [DisplayName("Repeat for how many weeks")]
        [Range(0,13)]
        public int Occurence { get; set; }
        //public class CheckBoxDay
        //{
        //    public DayOfWeek Day { get; set; }
        //    public bool IsSelected { get; set; }
        //}

        //This is server-side validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool hasPastAlready = EndTime < DateTime.Now;
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult("Order of datetimes are invalid.", new[] { nameof(StartTime), nameof(EndTime) });
            }
            if (hasPastAlready)
            {
                yield return new ValidationResult("Cannot create a reservation in the past", new[] { nameof(StartTime), nameof(EndTime) });
            }

        }
    }
    
    
}
