using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreateDateTime { get; set; }

        [Required]
        public int Value { get; set; }

        [Required MaxLength(200)]
        public string Comment { get; set; }

        [ForeignKey("Booking")]
        public int? BookingId { get; set; }
        public Booking Booking { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("Clinic")]
        public int? ClinicId { get; set; }
        public Clinic Clinic { get; set; }
    }
}
