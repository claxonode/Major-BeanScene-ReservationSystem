using Major_BeanScene_ReservationSystem.CustomValidation;
using Major_BeanScene_ReservationSystem.Models.Reservation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Staff.Models.Home
{
    public class StaffCreateReservation 
    {
        [Display(Name = "Reservation Origin")]
        public int ReservationOriginId { get; set; }
        public SelectList? ReservationOrigin { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [AustralianPhoneNumber(ErrorMessage = "Please enter a valid Australian phone number.")]
        public string PhoneNumber { get; set; }

        [Remote(action: "ValidateDate", controller: "Home")]
        [Required]
        [Display(Name = "Select Date")]
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; }

        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }
        public SelectList? Sitting { get; set; }

        [Remote(action: "ValidateTime", controller: "Home", AdditionalFields = "SittingId,SelectedDate")]
        [Required]
        [Display(Name = "Select Time")]
        [DataType(DataType.Time)]
        public TimeSpan SelectedTime { get; set; }

        [Remote(action: "ValidateDuration", controller: "Home",AdditionalFields = "SelectedTime,SittingId")]
        [Required]
        [Display(Name = "How long will your reservation last (minutes)")]
        public int Duration { get; set; }

        [Remote(action: "ValidateGuests", controller: "Home")]
        [Required]
        [Display(Name = "How many guests will be attending?")]
        public int NumberOfGuests { get; set; }
        [Display(Name = "Additional Notes")]
        public string? AdditionalNotes { get; set; }
    }

}
