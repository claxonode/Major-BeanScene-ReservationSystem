using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Major_BeanScene_ReservationSystem.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        //in minutes 1 = 1 min
        public int Duration { get; set; }
        public int Guests { get; set; }
        public List<Table> Tables { get; set; } = new();
        public ReservationOrigin ReservationOrigin { get; set; }
        public int ReservationOriginId { get; set; }
        public ReservationStatus ReservationStatus { get; set; }
        public int ReservationStatusId { get; set; }
        public Sitting Sitting { get; set; }
        public int SittingId { get; set; }
        [StringLength(250)]
        public string? AdditionalNotes { get; set; }

        public Person Person { get; set; }
        public int PersonId { get; set; }
        public Guest? Guest { get; set; }
        public int? GuestId { get; set; }
        public string TablesToString
        {
            get { return Tables.Count == 0 ? "No Tables" : $"{string.Join(", ", Tables.Select(x=>x.Name))}"; }
        }

        public DateTime EndTime {
            get {  return StartTime.AddMinutes(Duration); } 
        }
    }
}
