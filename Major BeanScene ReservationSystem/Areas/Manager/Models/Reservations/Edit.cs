using Major_BeanScene_ReservationSystem.CustomValidation;
using Major_BeanScene_ReservationSystem.Data;
using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Major_BeanScene_ReservationSystem.Areas.Manager.Models.Reservations
{
    public class Edit : IValidatableObject
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int PersonId { get; set; }
        //public Person Person { get; set; }
        public DateTime SelectedDate { get; set; }
        public DateTime SelectedTime { get; set; }
        public DateTime SelectedDateTime { 
            get
            {
                return SelectedDate.Date.AddHours(SelectedTime.Hour).AddMinutes(SelectedTime.Minute);
            }
            set
            {
                value = SelectedDate.Date.AddHours(SelectedTime.Hour).AddMinutes(SelectedTime.Minute);
            }
        }
        [Required]
        public int SittingId { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public int Guests { get; set; }
        [Required]
        public int ReservationOriginId { get; set; }
        [Required]
        public int ReservationStatusId { get; set; }
        
        public string? AdditionalNotes { get; set; }

        [Display(Name = "Guest First Name")]
        public string? GuestFirstName { get; set; }

        [Display(Name = "Guest Last Name")]
        public string? GuestLastName { get; set; }

        [Display(Name = "Guest Email Address")]
        public string? GuestEmail { get; set; }

        [Display(Name = "Guests Phone Number")]
        public string? GuestPhoneNumber { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            if (context == null)
            {
                yield return new ValidationResult("Could not retreive the database");
            }
            else
            {
                var currentReservation = context.Reservations.Include(x => x.Person).FirstOrDefault(x => x.Id == Id);
                var currentSitting = context.Sittings.FirstOrDefault(s => s.Id == currentReservation.SittingId);

                var chosenSitting = context.Sittings.Include(x=>x.Reservations).FirstOrDefault(x => x.Id == SittingId);

                bool hasReservationOriginId = context.ReservationOrigins.Any(x => x.Id == ReservationStatusId);
                bool hasReservationStatusId = context.ReservationStatuses.Any(x => x.Id == ReservationStatusId);

                if (chosenSitting != null & currentReservation != null)
                {
                    bool isPastSitting = currentSitting.EndTime < DateTime.Now;
                    bool isSittingToBeEditInThePast = chosenSitting.EndTime < DateTime.Now;
                    if (currentSitting.Id != chosenSitting.Id & isPastSitting)
                    {
                        yield return new ValidationResult("Cannot change the sitting if it is already past.", new[] { nameof(SelectedDateTime) });
                    }
                    if (Guests != currentReservation.Guests & isPastSitting)
                    {
                        yield return new ValidationResult("Cannot change guest count if it is already past.", new[] { nameof(Guests) });
                    }
                    if (Duration != currentReservation.Duration & isPastSitting)
                    {
                        yield return new ValidationResult("Cannot change duration if it is already past.", new[] { nameof(Duration) });
                    }
                    if (SelectedDateTime != currentReservation.StartTime & isPastSitting)
                    {
                        yield return new ValidationResult("Cannot change start time if it is already past.", new[] { nameof(Duration) });
                    }
                    if (SelectedDateTime.AddMinutes(Duration)>=chosenSitting.EndTime)
                    {
                        yield return new ValidationResult("Minutes cannot exceed this sitting end time.", new[] { nameof(SelectedDateTime) });
                    }
                    
                    if (SelectedDateTime > chosenSitting.EndTime || SelectedDateTime < chosenSitting.StartTime)
                    {
                        yield return new ValidationResult("Selected Date time does not match for chosen sitting.", new[] { nameof(SelectedDateTime) });
                    }
                    //current Capacity of sitting + reservation.NumberOfGuest > newSittings's capacity
                    if (chosenSitting.Reservations.Sum(x=>x.Guests)+Guests>chosenSitting.Capacity)
                    {
                        yield return new ValidationResult("Guest exceed this sitting's capacity.", new[] { nameof(Guests) });
                    }
                    if (chosenSitting.IsClosed)
                    {
                        yield return new ValidationResult("This sitting is closed and cannot be booked into.", new[] { nameof(SittingId) });
                    }
                    if (currentReservation.PersonId != PersonId)
                    {
                        yield return new ValidationResult("Person does not match.", new[] { nameof(PersonId) });
                    }
                    if (!hasReservationOriginId)
                    {
                        yield return new ValidationResult("Reservation Origin can't be found", new[] { nameof(ReservationStatusId) });
                    }
                    if (!hasReservationStatusId)
                    {
                        yield return new ValidationResult("Reservation Status can't be found", new[] { nameof(ReservationOriginId) });
                    }
                    //Prevent reservation from moving to a sitting that has already past.
                    if (isSittingToBeEditInThePast)
                    {
                        yield return new ValidationResult("Cannot change to a selected sitting that is in the past", new[] {nameof(SittingId)});
                    }
                }
                else
                {
                    yield return new ValidationResult("Error occurred", new[] { nameof(SittingId) });
                }
            }
            

            
            

        }
    }
    
    //public class ValidateSitting : ValidationAttribute
    //{
    //    //TODO complete validation check for sittingid
    //    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    {
    //        Edit model = (Edit)validationContext.ObjectInstance;
    //        var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

    //    }
    //    //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    //    //{
    //    //    Edit model = (Edit)validationContext.ObjectInstance;
    //    //    var context = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));
    //    //    var reservation = context.Reservations.Include(x=>x.Person).Include(x=>x.Sitting).FirstOrDefault(s => s.Id == model.Id);

    //    //    if (query.Reservations.Any(r => r.StartTime < model.StartTime ||
    //    //        (r.StartTime.AddMinutes(r.Duration) > model.EndTime)))
    //    //    {
    //    //        return new ValidationResult($"{query.Reservations.Count} reservations date/s conflicts with this sitting time period.");
    //    //    }
    //    //    var reservationGuestTotal = query.Reservations.Sum(r => r.Guests);
    //    //    if (reservationGuestTotal > model.Guests)
    //    //    {
    //    //        return new ValidationResult($"Total of all reservations guest is {reservationGuestTotal} conflicts with the sitting capacity you want to save.");
    //    //    }
    //    //    if (query.Sitting.IsClosed)
    //    //    {
    //    //        return new ValidationResult("Sitting needs to be open");
    //    //    }


    //    //    return ValidationResult.Success;
    //    //}
    //}
}
