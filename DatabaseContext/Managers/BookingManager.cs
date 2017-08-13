using DatabaseContext.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using DatabaseContext.Logger;
using DatabaseContext.Models.Enums;

namespace DatabaseContext.Managers
{
    public class BookingManager
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private UserManager _userManager = new UserManager();
        private EmailManager _emailManager = new EmailManager();
        private TreatmentManager _treatmentManager = new TreatmentManager();
        private ClinicManager _clinicManager = new ClinicManager();

        public List<Booking> GetAll(string treatmentId, DateTime? dateFrom, DateTime? dateTo, int? page, int? rows, bool isDone = false, bool? isPaid = null)
        {
            IQueryable<Booking> query = _context.Bookings
                  .Include("Treatment")
                  .Include("Clinic")
                  .Include("User")
                  .OrderByDescending(b => b.Id);

            if (!string.IsNullOrEmpty(treatmentId))
            {
                int treatId = int.Parse(treatmentId);
                query = query.Where(c => c.TreatmentId == treatId);
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(c => c.BookDateTime >= dateFrom);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(c => c.BookDateTime <= dateTo);
            }
            if (isDone)
            {
                query = query.Where(b => b.StatusId == (int)BookingStatusEnum.Completed);
            }
            if (isPaid.HasValue)
            {
                query = query.Where(b => b.IsPaid == isPaid.Value);
            }
            if (page.HasValue && rows.HasValue)
            {
                query = query.Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }

            return query.ToList();
        }

        public int GetCount(string treatmentId, DateTime? dateFrom, DateTime? dateTo, bool isDone = false)
        {
            IQueryable<Booking> query = _context.Bookings;

            if (!string.IsNullOrEmpty(treatmentId))
            {
                int treatId = int.Parse(treatmentId);
                query = query.Where(c => c.TreatmentId == treatId);
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(c => c.BookDateTime >= dateFrom);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(c => c.BookDateTime <= dateTo);
            }
            if (isDone)
            {
                query = query.Where(b => b.StatusId == (int)BookingStatusEnum.Completed);
            }

            return query.Count();
        }

        public List<Booking> GetAllByUser(int userId)
        {
            return
                _context.Bookings
                    .Include("Treatment")
                    .Include("Clinic")
                    .Include("User")
                    .OrderByDescending(b => b.Id)
                    .Where(t => t.UserId == userId)
                    .ToList();
        }

        public Booking GetById(int id)
        {
            return _context.Bookings
                    .Include("Payments")
                    .Include("PaidToClinicByUser")
                    .FirstOrDefault(t => t.Id == id);
        }

        public int? CreateOrUpdate(Booking booking)
        {
            //check booking datetime:
            var clinic = _clinicManager.GetById(booking.ClinicId);
            
            var clinicHours = clinic.Hours.FirstOrDefault(h => h.Day == booking.BookDateTime.DayOfWeek);
            if (clinicHours == null || !clinicHours.BookingOpenHour.HasValue || !clinicHours.BookingCloseHour.HasValue 
                || booking.BookDateTime.TimeOfDay > clinicHours.BookingCloseHour.Value.TimeOfDay
                || booking.BookDateTime.TimeOfDay < clinicHours.BookingOpenHour.Value.TimeOfDay)
            {
                return null;
            }

            if (booking.Id == 0)
            {
                booking.ApproveKey = Guid.NewGuid();
                booking.DeclineKey = Guid.NewGuid();

                _context.Bookings.Add(booking);
            }
            else
            {
                var oldBooking = _context.Bookings.FirstOrDefault(t => t.Id == booking.Id);

                oldBooking.BookDateTime = booking.BookDateTime;
                oldBooking.StatusId = booking.StatusId;
                oldBooking.Comments = booking.Comments;
                oldBooking.UserId = booking.UserId;
                oldBooking.ClinicId = booking.ClinicId;
                oldBooking.TreatmentId = booking.TreatmentId;
                oldBooking.FirstName = booking.FirstName;
                oldBooking.LastName = booking.LastName;
                oldBooking.ClientEmail = booking.ClientEmail;
                oldBooking.ClientPhone = booking.ClientPhone;
                oldBooking.StaffComments = booking.StaffComments;
            }

            _context.SaveChanges();

            return booking.Id;
        }

        public bool Delete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(c => c.Id == id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
            _context.SaveChanges();

            return true;
        }

        public Booking ApproveOrDeclineBooking(Guid key)
        {
            try
            {
                if (key == Guid.Empty)
                {
                    return null;
                }

                bool isApprove = true;
                var booking =
                    _context.Bookings.Include("Treatment").Include("Clinic").FirstOrDefault(t => t.ApproveKey == key);
                if (booking == null)
                {
                    isApprove = false;
                    booking =
                        _context.Bookings.Include("Treatment")
                            .Include("Clinic")
                            .FirstOrDefault(t => t.DeclineKey == key);
                }
                if (booking == null)
                {
                    return null;
                }
                booking.StatusId = isApprove
                    ? (int)BookingStatusEnum.Confirmed
                    : (int)BookingStatusEnum.DeclinedByDoctor;

                _context.SaveChanges();
                return booking;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
                return null;
            }
        }

        public void SetPaid(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(t => t.Id == id);
            if (booking != null)
            {
                booking.IsPaid = true;
                _context.SaveChanges();
            }
        }

        public void Cancel(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(t => t.Id == id);
            if (booking != null)
            {
                booking.StatusId = (int)BookingStatusEnum.DeclinedByClient;
                _context.SaveChanges();
            }
        }

        public bool IsBookingBelongToUser(int userId, int bookingId)
        {
            return _context.Bookings.Any(b => b.Id == bookingId && b.UserId == userId);
        }

        public void SetBookingsCompleted()
        {
            var bookingsToBeCompleted =
                _context.Bookings.Where(b => b.IsPaid &&
                                             b.StatusId == (int) BookingStatusEnum.Created &&
                                             b.BookDateTime < DateTime.UtcNow).ToList();
            foreach (var booking in bookingsToBeCompleted)
            {
                booking.StatusId = (int) BookingStatusEnum.Completed;
            }
            _context.SaveChanges();
        }

        public void CompleteBooking(int id, int userId, DateTime? paidOn)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking != null)
            {
                booking.PaidToClinicOn = paidOn.Value.ToUniversalTime();
                booking.PaidToClinicByUserId = userId;
                booking.StatusId = (int)BookingStatusEnum.PaidToCLinic;
                _context.SaveChanges();
            }
        }

        public void UndoBookingComplete(int id)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking != null)
            {
                booking.PaidToClinicOn = null;
                booking.PaidToClinicByUserId = null;
                booking.StatusId = (int)BookingStatusEnum.Completed;
                _context.SaveChanges();
            }
        }

        public List<DateTime> GetBookedDates(int clinicId, DateTimeOffset from, DateTimeOffset to)
        {
            return
                _context.Bookings.Where(b => b.ClinicId == clinicId && b.BookDateTime >= from && b.BookDateTime <= to &&
                    b.StatusId != (int)BookingStatusEnum.DeclinedByClient && b.StatusId != (int)BookingStatusEnum.DeclinedByDoctor)
                    .Select(b => b.BookDateTime)
                    .ToList();
        }
    }
}
