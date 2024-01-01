using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Data
{
    public class MyDataSeeder
    {
        public MyDataSeeder(ModelBuilder mb)
        {
            mb.Entity<Person>().HasData(
                new Person() {Id = 1, FirstName = "Zac", LastName = "Chalmers", Email = "z@c.com", PhoneNumber = "0410932851"},
                new Person() {Id = 2, FirstName = "Geoffrey", LastName = "Li", Email = "g@l.com", PhoneNumber = "0424278461" }
                );

            mb.Entity<Restaurant>().HasData(
                new Restaurant() { Id = 1, Name = "Bean Scene" }
                );
            mb.Entity<Area>().HasData(
                new Area() { Id = 1, Name = "Main", RestaurantId = 1 },
                new Area() { Id = 2, Name = "Balcony", RestaurantId = 1 },
                new Area() { Id = 3, Name = "Outside", RestaurantId = 1 }
                );
            mb.Entity<ReservationOrigin>().HasData(
               new ReservationOrigin() { Id = 1, Name = "Online" },
               new ReservationOrigin() { Id = 2, Name = "Mobile App" },
               new ReservationOrigin() { Id = 3, Name = "In-person" },
               new ReservationOrigin() { Id = 4, Name = "Email" },
               new ReservationOrigin() { Id = 5, Name = "Phone" }
               );
            mb.Entity<ReservationStatus>().HasData(
                new ReservationStatus() { Id = 1, Name = "Pending" },
                new ReservationStatus() { Id = 2, Name = "Confirmed" },
                new ReservationStatus() { Id = 3, Name = "Cancelled" },
                new ReservationStatus() { Id = 4, Name = "Seated" },
                new ReservationStatus() { Id = 5, Name = "Completed" }
                );
            mb.Entity<SittingType>().HasData(
                new SittingType() { Id = 1, Name = "Breakfast" },
                new SittingType() { Id = 2, Name = "Lunch" },
                new SittingType() { Id = 3, Name = "Dinner" },
                new SittingType() { Id = 4, Name = "Special" }
                );
            mb.Entity<Table>().HasData(MakeTables());
            // how do we ensure that sittings do not overlap

            mb.Entity<Sitting>().HasData(
                new Sitting()
                {
                    Id = 1,
                    Name = "Breakfast",
                    Capacity = 40,
                    StartTime = new DateTime(2023, 9, 1, 7, 0, 0),
                    EndTime = new DateTime(2023, 9, 1, 10, 0, 0),
                    RestaurantId = 1,
                    SittingTypeId = 1,
                    IsClosed = false
                },
                new Sitting()
                {
                    Id = 2,
                    Name = "Lunch",
                    Capacity = 40,
                    StartTime = new DateTime(2023, 9, 1, 11, 0, 0),
                    EndTime = new DateTime(2023, 9, 1, 15, 0, 0),
                    RestaurantId = 1,
                    SittingTypeId = 2,
                    IsClosed = false
                },
                new Sitting()
                {
                    Id = 3,
                    Name = "Dinner",
                    Capacity = 40,
                    StartTime = new DateTime(2023, 9, 1, 18, 0, 0),
                    EndTime = new DateTime(2023, 9, 1, 21, 0, 0),
                    RestaurantId = 1,
                    SittingTypeId = 3,
                    IsClosed = false
                },
                new Sitting()
                {
                    Id = 4,
                    Name = "Special Dinner",
                    Capacity = 40,
                    StartTime = new DateTime(2023, 9, 2, 18, 0, 0),
                    EndTime = new DateTime(2023, 9, 2, 21, 0, 0),
                    RestaurantId = 1,
                    SittingTypeId = 3,
                    IsClosed = true
                }
                );
            mb.Entity<Reservation>().HasData(
                new Reservation()
                {
                    Id = 1,
                    Guests = 3,
                    Duration = 60,
                    SittingId = 1,
                    ReservationOriginId = 1,
                    StartTime = new DateTime(2023, 9, 1, 7, 0, 0),
                    PersonId = 1
                },
                new Reservation()
                {
                    Id = 2,
                    Guests = 10,
                    Duration = 60,
                    SittingId = 1,
                    ReservationOriginId = 1,
                    StartTime = new DateTime(2023, 9, 1, 7, 0, 0),
                    PersonId = 1
                },
                new Reservation()
                {
                    Id = 4,
                    Guests = 3,
                    Duration = 60,
                    SittingId = 2,
                    ReservationOriginId = 1,
                    StartTime = new DateTime(2023, 9, 11, 7, 0, 0),
                    PersonId = 2
                },
                new Reservation()
                {
                    Id = 5,
                    Guests = 10,
                    Duration = 60,
                    SittingId = 3,
                    ReservationOriginId = 1,
                    StartTime = new DateTime(2023, 9, 18, 7, 0, 0),
                    PersonId = 2
                }
                );
            //when we delete a sitting, cascade delete the reservation?? should we




            mb.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Member", NormalizedName = "MEMBER" },
                new IdentityRole() { Name = "Staff", NormalizedName = "STAFF" },
                new IdentityRole() { Name = "Manager", NormalizedName = "MANAGER" }
                );
            //Add some reservations
            //Add some Sittings
            //Sitting has an Id, Name-randomName, Capacity, Start-EndTime, IsClosed (prevents bookings), SittingType, RestaurantId

            //ReservationOrigin decider-> Member makes reservation on website = Online, on mobileapp = Mobile App
            //Staff/Manager makes reservation = SelectList In-person,Email,Phone
            //ReservationStatus decider-> by default pending
            //The member, staff/manager can -> cancel. Must be them
            //within the one week -> confirm by staff/manager
            //within the day and before the end time, staff/manager can seat
            //seated->completed at end time.

            //staff/manager assigns tables

            //we can create custom attributes for validating start and end time: server side
            //class level validation with IValidatableObject
            //ValidationResult 

            //Later make a person
            //mb.Entity<Person>().HasData(
            //    new Person() { FirstName="John",LastName="Smith",Email="johnsmith@email.com",PhoneNumber="0412345678"});
        }

        private List<Table> MakeTables()
        {
            List<Table> tables = new List<Table>();
            int tableId = 1;
            int areaId = 1;
            foreach (string area in new string[] { "Main", "Balcony", "Outside" })
            {
                for (int i = 1; i < 11; i++)
                {
                    tables.Add(new Table() { Id = tableId, Name = $"{area[0]}{i}", AreaId = areaId });
                    tableId++;
                }
                areaId++;
            }

            return tables;
        }
    }
}
