using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class BookingEditItem
    {
        [Key]
        public int Id { get; set; }

        [Required MaxLength(200) DisplayName("First name")]
        public string FirstName { get; set; }

        [Required MaxLength(200) DisplayName("Last name")]
        public string LastName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required MaxLength(100) DisplayName("Email")]
        public string ClientEmail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required MaxLength(50) DisplayName("Phone")]
        public string ClientPhone { get; set; }

        [Required]
        public BookingStatusEnum Status { get; set; }

        [Required DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookDate { get; set; }

        [Required DisplayName("Time")]
        [DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public string BookTime { get; set; }

        public string Comments { get; set; }

        [Required(ErrorMessage = "Location must be selected") DisplayName("Location")]
        [Range(1, int.MaxValue, ErrorMessage = "Location must be selected.")]
        public int ClinicId { get; set; }

        [Required DisplayName("Treatment")]
        public int TreatmentId { get; set; }

        [Required DisplayName("User")]
        public int UserId { get; set; }
        [Required DisplayName("Service")]
        public int TreatmentCategoryId { get; set; }
        public TreatmentItem[] TreatmentList { get; set; }
        public TreatmentCategoryItem[] TreatmentCategoriesList { get; set; }

        public ClinicItem[] ClinicList { get; set; }

        public ValueItem[] UserList { get; set; }

        public object[] TimeList { get; set; }

        public Guid? ApproveKey { get; set; }

        public Guid? DeclineKey { get; set; }

        [Required DisplayName("Is paid")]
        public bool IsPaid { get; set; }

        [DisplayName("Staff comments")]
        public string StaffComments { get; set; }

        public decimal RefundSum { get; set; }

        public bool HasPayments { get; set; }

        [DisplayName("Paid to clinic on")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PaidToClinicOn { get; set; }

        [DisplayName("Paid to clinic by user")]
        public string PaidToClinicUserName { get; set; }
    }
}