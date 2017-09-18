using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class BookingViewItem : BookingItem
    {
        public decimal PaymentAmount { get; set; }

        [Required DisplayName("Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookDate
        {
            get { return BookDateTime; }
        }

        [Required DisplayName("Time")]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public string BookTime
         {
            get { return BookDateTime.ToString("HH:mm"); }
        }
    }
}