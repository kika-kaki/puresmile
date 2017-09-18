using System;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class ClinicHourEditItem
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of clinic
        /// </summary>
        [Required]
        public int Day { get; set; }

        /// <summary>
        /// Name of clinic
        /// </summary>
        [Required]
        public string DayName { get { return ((DayOfWeek)Day).ToString(); } }

        /// <summary>
        /// Time of clinic opening
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? OpenHour { get; set; }

        /// <summary>
        /// Time of clinic closing
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? CloseHour { get; set; }

        /// <summary>
        /// Time of clinic opening booking
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? BookingOpenHour { get; set; }

        /// <summary>
        /// Time of clinic closing booking
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Time)]
        public DateTime? BookingCloseHour { get; set; }
    }
}