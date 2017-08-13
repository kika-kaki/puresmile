using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FirstName { get; set; }

        [Required, MaxLength(200)]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required, MaxLength(100)]
        public string ClientEmail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required, MaxLength(50)]
        public string ClientPhone { get; set; }

        [Required]
        public DateTime BookDateTime { get; set; }
        
        [Required]
        public int StatusId { get; set; }

        [Required]
        [DefaultValue("false")]
        public bool IsPaid { get; set; }

        [MaxLength(100)]
        public string Comments { get; set; }

        public Guid? ApproveKey { get; set; }

        public Guid? DeclineKey { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required, ForeignKey("Clinic")]
        public int ClinicId { get; set; }
        public Clinic Clinic { get; set; }

        [Required, ForeignKey("Treatment")]
        public int TreatmentId { get; set; }
        public Treatment Treatment { get; set; }

        [InverseProperty("Booking")]
        public virtual List<Payment> Payments { get; set; }

        [MaxLength(4096)]
        public string StaffComments { get; set; }

        public DateTime? PaidToClinicOn { get; set; }

        [ForeignKey("PaidToClinicByUser")]
        public int? PaidToClinicByUserId { get; set; }
        public User PaidToClinicByUser { get; set; }
    }
}
