using DatabaseContext.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class EmailItem
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
        [DataType(DataType.EmailAddress)]
        public string RecipientEmail { get; set; }

        /// <summary>
        /// Name of recipient
        /// </summary>
        public string RecipientName { get; set; }

        /// <summary>
        /// Subject of notification
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Creating date
        /// </summary>
        [DisplayName("Created")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
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
        public EmailStatus Status { get; set; }

        [DisplayName("Status")]
        public string EmailStatus
        {
            get
            {
                return Enum.GetName(typeof(EmailStatus), Status);
            }
        }
    }
}