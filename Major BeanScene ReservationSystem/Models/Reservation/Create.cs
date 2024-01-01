using Major_BeanScene_ReservationSystem.CustomValidation;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Major_BeanScene_ReservationSystem.Models.Reservation
{
    public class Create
    {
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

        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }
        public SelectList? Sitting { get; set; }

        [Required]
        //[DisplayFormat(ApplyFormatInEditMode =true,DataFormatString ="{0:dd/MM/yyyy HH:mm tt}")]
        [Display(Name = "What date would you like to dine with us?")]
        public DateTime StartTime { get; set; }
        [Required]
        [Display(Name = "How long will your reservation last (minutes)")]
        public int Duration { get; set; }
        [Required]
        [Display(Name = "How many guests will be attending?")]
        public int NumberOfGuests { get; set; }
        public string? AdditionalNotes { get; set; }
    }
}
