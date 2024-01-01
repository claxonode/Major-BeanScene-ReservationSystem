using Major_BeanScene_ReservationSystem.CustomValidation;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Major_BeanScene_ReservationSystem.Models.Reservation
{
    public class CreateWithSelectedDate : IValidatableObject
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Is this reservation for someone else?")]
        public bool? ForSomeoneElse { get; set; }
        [Display(Name = "Guest First Name")]
        public string? GuestFirstName { get; set; }

        [Display(Name = "Guest Last Name")]
        public string? GuestLastName { get; set; }

        [Display(Name = "Guest Email Address")]
        public string? GuestEmail { get; set; }

        [Display(Name = "Guests Phone Number")]
        [AustralianPhoneNumberForGuest(ErrorMessage = "Please enter a valid Australian phone number.")]
        [ConditionalRequired("ForSomeoneElse", ErrorMessage = "Guest phone number is required when making a reservation for someone else.")]
        public string? GuestPhoneNumber { get; set; }


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
        
        [Remote(action:"ValidateDate", controller:"Reservation", areaName:"")]
        [Required]
        [Display(Name = "Select Date")]
        [DataType(DataType.Date)]
        public DateTime SelectedDate { get; set; }

        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }
        public SelectList? Sitting { get; set; }

        [Remote(action: "ValidateTime", controller: "Reservation", areaName: "",AdditionalFields = "SittingId,SelectedDate")]
        [Required]
        [Display(Name = "Select Time")]
        [DataType(DataType.Time)]
        public TimeSpan SelectedTime { get; set; }

        [Remote(action:"ValidateDuration", controller:"Reservation", areaName: "")]
        [Required]
        [Display(Name = "How long will your reservation last (minutes)")]
        public int Duration { get; set; }

        [Remote(action: "ValidateGuests", controller: "Reservation", areaName: "")]
        [Required]
        [Display(Name = "How many guests will be attending?")]
        public int NumberOfGuests { get; set; }

        public string? AdditionalNotes { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            var reservation = context.Reservations.Include(x => x.Person).FirstOrDefault(x => x.Id == Id);
            var sitting = context.Sittings.Include(x => x.Reservations).FirstOrDefault(x => x.Id == SittingId);

            if (sitting.Reservations.Sum(x => x.Guests) + NumberOfGuests > sitting.Capacity)
            {
                yield return new ValidationResult("Amount of Guests exceeds this sittings capacity, Please contact us about setting a reservation for this sitting or select another sitting.", new[] { nameof(NumberOfGuests) });
            }
        }
    }
}


