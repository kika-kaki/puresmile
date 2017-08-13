using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Total { get; set; }

        [Required]
        public int StatusId { get; set; }

        [MaxLength(50)]
        public string TransactionCode { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public bool IsRefund { get; set; }

        [Required, ForeignKey("Booking")]
        public int BookingId { get; set; }
        public virtual Booking Booking { get; set; }

        [Required, ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [ForeignKey("RefundByUser")]
        public int? RefundByUserId { get; set; }        
        public virtual User RefundByUser { get; set; }

        [MaxLength(1024)]
        public string TransactionResponseText { get; set; }

        [MaxLength(64)]
        public string TransactionId { get; set; }
        [MaxLength(1024)]
        public string Message { get; set; }
        [MaxLength(64)]
        public string OrderId { get; set; }

        public DateTime? CreatedAt { get; set; }
        public bool IsSuccessTransaction { get; set; }
        [MaxLength(64)]
        public string PaypalDebugId { get; set; }
        [MaxLength(64)]
        public string PayPalPaymentId { get; set; }
        [MaxLength(64)]
        public string PayPalTransactionFeeAmount { get; set; }
        [MaxLength(128)]
        public string PayPalPayeeEmail { get; set; }
        [MaxLength(128)]
        public string PayPalPayerEmail { get; set; }
        [MaxLength(64)]
        public string Status { get; set; }
        [MaxLength(16)]
        public string CardBin { get; set; }
        [MaxLength(64)]
        public string CardCardholderName { get; set; }
        [MaxLength(16)]
        public string CardType { get; set; }
        [MaxLength(16)]
        public string CardExpirationDate { get; set; }
        [MaxLength(8)]
        public string CardLastFour { get; set; }
    }
}
