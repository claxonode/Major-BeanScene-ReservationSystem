using Major_BeanScene_ReservationSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Sittings
{
    public class Edit : Create
    {
        [Required]
        [CheckReservationDates]
        public int Id { get; set; }

    }
    public class CheckReservationDates : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
            Edit updateSitting = (Edit)validationContext.ObjectInstance;
            
            var currentSitting = context.Sittings.Include("Reservations").FirstOrDefault(s=>s.Id == updateSitting.Id);
            
            if (currentSitting.Reservations.Any(r=>r.StartTime < updateSitting.StartTime ||
                (r.StartTime.AddMinutes(r.Duration) > updateSitting.EndTime))) 
            {
                return new ValidationResult($"{currentSitting.Reservations.Count} reservations date/s conflicts with this sitting time period.");
            }
            var isPastSitting = currentSitting.EndTime < DateTime.Now;
            if (isPastSitting)
            {
                return new ValidationResult($"Cannot change a past sitting's details");
            }
            var reservationGuestTotal = currentSitting.Reservations.Sum(r => r.Guests);
            if (reservationGuestTotal >updateSitting.Capacity)
            {
                return new ValidationResult($"Total of all reservations guest is {reservationGuestTotal} conflicts with the sitting capacity you want to save.");
            }
            if (currentSitting.IsClosed)
            {
                return new ValidationResult("Sitting needs to be open");
            }


            return ValidationResult.Success;
        }
    }
}
