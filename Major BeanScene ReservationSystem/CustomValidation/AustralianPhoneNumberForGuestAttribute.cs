﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Major_BeanScene_ReservationSystem.CustomValidation
{
    public class AustralianPhoneNumberForGuestAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow null when ForSomeoneElse is false
            }

            string phoneNumber = value.ToString();
            string pattern = @"^(\(61\)|\+61|0)[1-9]\d{8}$"; // Australian phone number pattern

            if (Regex.IsMatch(phoneNumber, pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Please enter a valid Australian phone number.");
        }
    }
}
