using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Hours of clinic's work.
    /// </summary>
    public class ClinicHours
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
        public DayOfWeek Day { get; set; }

        /// <summary>
        /// Time of clinic opening
        /// </summary>
        public DateTime? OpenHour { get; set; }

        /// <summary>
        /// Time of clinic closing
        /// </summary>
        public DateTime? CloseHour { get; set; }

        /// <summary>
        /// Time of clinic booking open
        /// </summary>
        public DateTime? BookingOpenHour { get; set; }

        /// <summary>
        /// Time of clinic booking closing
        /// </summary>
        public DateTime? BookingCloseHour { get; set; }

        /// <summary>
        /// Clinic id
        /// </summary>
        [Required, ForeignKey("Clinic")]
        public int ClinicId { get; set; }

        /// <summary>
        /// Clinic additional field
        /// </summary>
        public virtual Clinic Clinic { get; set; }
    }
}
