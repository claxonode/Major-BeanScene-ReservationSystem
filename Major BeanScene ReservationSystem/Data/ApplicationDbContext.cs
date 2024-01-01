using Major_BeanScene_ReservationSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Major_BeanScene_ReservationSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Area> Areas { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationOrigin> ReservationOrigins { get; set; }
        public DbSet<ReservationStatus> ReservationStatuses { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Sitting> Sittings { get; set; }
        public DbSet<SittingType> SittingTypes { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Guest> GuestsDetails { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            // Add FLUID API
            builder.Entity<Sitting>()
                .ToTable(r => r.HasCheckConstraint("CHK_StartTimeAndEndTime", "[StartTime] < [EndTime]"))
                .ToTable(r => r.HasCheckConstraint("CHK_Capacity", "[Capacity] > 0"));

            //Should we do OverLapping Sittings

            //builder.Entity<Reservation>()
            //    .Property(r => r.AdditionalNotes).HasMaxLength(250);
                
            builder.Entity<Reservation>()
               //When creating reservation, it is always pending
               .Property(r => r.ReservationStatusId).HasDefaultValue(1);

            //builder.Entity<Person>()
            //    .Property(p => p.FirstName).HasMaxLength(50);
            //builder.Entity<Person>()
            //    .Property(p => p.LastName).HasMaxLength(50);
            //builder.Entity<Person>()
            //    .Property(p => p.Email).HasMaxLength(250);
            //builder.Entity<Person>()
            //    .Property(p => p.PhoneNumber).HasMaxLength(12);
            //builder.Entity<Person>()
            //    .ToTable("People")
            //    .Property(p => p.FirstName).HasMaxLength(50);

            /* Database constraint for reservation that adds toomuch capacity??
            builder.Entity<Reservation>().
                ToTable(r => r.HasCheckConstraint("CHK_Guests", "[Guests] > Sitting.Reservation.Sum(x=>x.Guests)"));
            */




            //If we delete a reservation it should not cascade to the Table
            //If we delete a table, it should not cascade to the reservation



            new MyDataSeeder(builder);

            base.OnModelCreating(builder);
        }
    }
}