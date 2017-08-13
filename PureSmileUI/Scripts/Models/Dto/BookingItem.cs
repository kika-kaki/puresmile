using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DatabaseContext.Extension;
using DatabaseContext.Models;

namespace PureSmileUI.Models.Dto
{
    public class BookingItem
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ClientEmail { get; set; }

        public string ClientPhone { get; set; }

        public DateTime BookDateTime { get; set; }

        public BookingStatusEnum Status { get; set; }

        public string BookingStatus
        {
            get
            {
                return Status.Description();
            }
        }

        public string Comments { get; set; }

        public int ClinicId { get; set; }

        [DisplayName("Clinic name")]
        public string ClinicName { get; set; }

        public int TreatmentId { get; set; }

        [DisplayName("Treatment name")]
        public string TreatmentName { get; set; }


        [DisplayName("Treatment name")]
        public string TreatmentCategory { get; set; }

        public int UserId { get; set; }

        [DisplayName("User name")]
        public string UserName { get; set; }

        public bool IsPaid { get; set; }
        public string PaymentSuccess { get; set; }
        public string PaymentDetailsUrl { get; set; }

        public DateTime? PaidToClinicOn { get; set; }

        public string PaidToClinicByUserName { get; set; }
    }
}