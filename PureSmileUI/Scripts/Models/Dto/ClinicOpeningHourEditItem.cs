using System;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class ClinicOpeningHourEditItem
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
        public float? OpenHour { get; set; }

        /// <summary>
        /// Time of clinic closing
        /// </summary>
        public float? CloseHour { get; set; }

        /// <summary>
        /// Time of clinic opening booking
        /// </summary>
        public float? BookingOpenHour { get; set; }

        /// <summary>
        /// Time of clinic closing booking
        /// </summary>
        public float? BookingCloseHour { get; set; }
    }
}