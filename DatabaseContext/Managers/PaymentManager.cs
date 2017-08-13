using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using DatabaseContext.Extension;
using DatabaseContext.Logger;
using DatabaseContext.Models;
using DatabaseContext.Models.Enums;
using PureSmileUI.Extension;

namespace DatabaseContext.Managers
{
    public class PaymentManager
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        public Payment GetById(int id)
        {
            return _context.Payments.FirstOrDefault(c => c.Id == id);
        }

        public int CreatePayment(Payment payment)
        {
            try
            {
                _context.Payments.Add(payment);
                bool isPaid = (int) PaymentStatus.Success == payment.StatusId;
                if (isPaid)
                {
                    var booking = _context.Bookings.FirstOrDefault(b => b.Id == payment.BookingId);
                    if (booking == null)
                    {
                        throw new Exception("FATAL ERROR. PaymentManager.CreatePayment booking not found by payment.");
                    }
                    booking.IsPaid = true;
                    _context.Entry(booking).Property(p => p.IsPaid).IsModified = true;
                }
                _context.SaveChanges();
                return payment.Id;
            }
            catch (DbEntityValidationException dbex)
            {
                LoggerHelper.LogException(dbex.ToDescription()); 
            }
            catch (Exception ex)
            {
                string details = payment.Serialize();
                string msg = string.Format(
                    "PaymentManager.CreatePayment payment not found by id: {1}{0}Payment details:{2}{0}",
                    Environment.NewLine, payment.Id, details);
                LoggerHelper.LogException(msg, ex);
            }
            return default(int);
        }

        public List<Payment> GetByBookingId(int id)
        {
            var list = _context.Payments.Where(p => p.BookingId == id).OrderByDescending(b => b.Id).ToList();
            return list;
        }

        public List<Payment> GetAll(int page, int rows)
        {
            var list = _context.Payments
                .Include("Booking.Treatment")
                .OrderBy(p => p.Id)
                .Skip((page - 1) * rows)
                .Take(rows)
                .ToList();

            return list;
        }

        public void Refund(int id, decimal refundSum, int userId)
        {
            Payment payment = new Payment()
            {
                BookingId = id,
                Total = refundSum,
                IsRefund = true,
                UserId = userId,
                CreatedOn = DateTime.Now.ToUniversalTime(),
                StatusId = (int)PaymentStatus.Success
            };
            try
            {
                _context.Payments.Add(payment);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbex)
            {
                LoggerHelper.LogException(dbex.ToDescription());
            }
            catch (Exception ex)
            {
                string details = payment.Serialize();
                string msg = string.Format(
                    "PaymentManager.CreatePayment payment not found by id: {1}{0}Payment details:{2}{0}",
                    Environment.NewLine, payment.Id, details);
                LoggerHelper.LogException(msg, ex);
            }
        }

        public int GetPaymentCount()
        {
            return _context.Payments.Count();
        }
        
        public decimal GetMaxAmountToRefund(int id)
        {
            var successPaymentsSum = _context.Payments.Where(p => p.BookingId == id && p.IsSuccessTransaction).ToList();
            var refundedSum = _context.Payments.Where(p => p.BookingId == id && p.IsRefund).ToList();
            return successPaymentsSum.Sum(p => p.Total) - refundedSum.Sum(p => p.Total);
        }
    }
}