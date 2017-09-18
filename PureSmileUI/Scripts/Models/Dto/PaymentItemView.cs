using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseContext.Extension;
using DatabaseContext.Models;
using DatabaseContext.Models.Enums;

namespace PureSmileUI.Models.Dto
{
    public class PaymentItemView
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public int StatusId { get; set; }
        public string StringStatus
        {
            get { return ((PaymentStatus)StatusId).Description(); }
        }
        public string TransactionCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsRefund { get; set; }
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int? RefundByUserId { get; set; }
        public string TransactionResponseText { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsSuccessTransaction { get; set; }
        public string PaypalDebugId { get; set; }
        public string PayPalPaymentId { get; set; }
        public string PayPalTransactionFeeAmount { get; set; }
        public string PayPalPayeeEmail { get; set; }
        public string PayPalPayerEmail { get; set; }
        public string Status { get; set; }
        public string CardBin { get; set; }
        public string CardCardholderName { get; set; }
        public string CardType { get; set; }
        public string CardExpirationDate { get; set; }
        public string CardLastFour { get; set; }
    }
}