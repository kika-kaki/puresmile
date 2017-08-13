using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class EmailEditItem
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Email of recipient
        /// </summary>
        [DisplayName("Recipient email")]
        [Required MaxLength(200)]
        [DataType(DataType.EmailAddress)]
        public string RecipientEmail { get; set; }

        /// <summary>
        /// Subject of notification
        /// </summary>
        [Required MaxLength(100)]
        public string Subject { get; set; }

        /// <summary>
        /// Body of notification
        /// </summary>
        [Required MaxLength(2000)]
        public string Body { get; set; }

        /// <summary>
        /// Creating date
        /// </summary>
        [Required]
        [DisplayName("Date of send")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Sent date
        /// </summary>
        [DisplayName("Date of send")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? SentOn { get; set; }

        /// <summary>
        /// Status (New, Sent, Error)
        /// </summary>
        [Required]
        public EmailStatus Status { get; set; }
    }
}