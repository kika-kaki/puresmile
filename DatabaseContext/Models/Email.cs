using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Email in system
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id of recipient
        /// </summary>
        [ForeignKey("Recepient")]
        public int? RecipientId { get; set; }
        public User Recepient { get; set; }

        /// <summary>
        /// Email of recipient
        /// </summary>
        [Required, MaxLength(200)]
        public string RecipientEmail { get; set; }

        /// <summary>
        /// Subject of notification
        /// </summary>
        [Required, MaxLength(100)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of notification
        /// </summary>
        [Required, MaxLength(2000)]
        public string Body { get; set; }

        /// <summary>
        /// Creating date
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Sent date
        /// </summary>
        public DateTime? SentOn { get; set; }

        /// <summary>
        /// Status (New, Sent, Error)
        /// </summary>
        [Required]
        public EmailStatus Status { get; set; }
    }
}
