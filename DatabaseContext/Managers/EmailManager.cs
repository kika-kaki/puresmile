using DatabaseContext.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Data.Entity.Validation;
using DatabaseContext.Logger;
using DatabaseContext.Models.Enums;

namespace DatabaseContext.Managers
{
    public class EmailManager
    {
        private ApplicationDbContext _context = new ApplicationDbContext();
        private ClinicManager _clinicManager = new ClinicManager();
        private TreatmentManager _treatmentManager = new TreatmentManager();
        private UserManager _userManager = new UserManager();
        private PaymentManager _paymentManager = new PaymentManager();

        public List<Email> GetAll(int userId, int? page, int? rows)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return new List<Email>();
            }

            IQueryable<Email> query = _context.Emails.Include("Recepient.ClientData").OrderByDescending(b => b.Id);

            if (page.HasValue && rows.HasValue)
            {
                query = query.Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }

            return query.ToList();
        }

        public List<Email> GetAll(string text, DateTime? from, DateTime? to, int? page, int? rows)
        {
            var query = _context.Emails.AsQueryable();
            query = query.Where(e => string.IsNullOrEmpty(text) || e.Subject.Contains(text) || e.Body.Contains(text));
            query = query.Where(e => !from.HasValue || e.CreatedOn > from.Value);
            if (to.HasValue)
            {
                DateTime _to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59, 999);
                query = query.Where(e => e.CreatedOn < _to);
            }
            query = query.OrderByDescending(d => d.Id);

            if (page.HasValue && rows.HasValue)
            {
                query = query.Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }

            return query.ToList();
        }

        public List<Email> GetMy(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user == null
                 ? new List<Email>()
                 : _context.Emails.Where(e => e.RecipientId == userId).OrderByDescending(d => d.SentOn).ToList();
        }

        public List<Email> GetMy(int userId, string text, DateTime? from, DateTime? to, int? page, int? rows)
        {
            var query = _context.Emails.AsQueryable();
            query = query.Where(e => e.RecipientId == userId);
            query = query.Where(e => string.IsNullOrEmpty(text) || e.Subject.Contains(text) || e.Body.Contains(text));
            query = query.Where(e => !from.HasValue || e.CreatedOn > from.Value);
            if (to.HasValue)
            {
                DateTime _to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59, 999);
                query = query.Where(e => e.CreatedOn < _to);
            }
            query = query.OrderByDescending(d => d.Id);

            if (page.HasValue && rows.HasValue)
            {
                query = query.Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }
            return query.ToList();
        }

        public Email GetById(int id)
        {
            return _context.Emails.FirstOrDefault(c => c.Id == id);
        }

        public int GetAllCount(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return user == null ? 0 : _context.Emails.Count();
        }

        public int GetMyCount(int userId, string text, DateTime? from, DateTime? to)
        {
            var query = _context.Emails.AsQueryable();
            query = query.Where(e => e.RecipientId == userId);
            query = query.Where(e => string.IsNullOrEmpty(text) || e.Subject.Contains(text) || e.Body.Contains(text));
            query = query.Where(e => !from.HasValue || e.CreatedOn > from.Value);
            if (to.HasValue)
            {
                DateTime _to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59, 999);
                query = query.Where(e => e.CreatedOn < _to);
            }
            return query.Count();
        }

        public int GetAllCount(string text, DateTime? from, DateTime? to)
        {
            var query = _context.Emails.AsQueryable();
            query = query.Where(e => string.IsNullOrEmpty(text) || e.Subject.Contains(text) || e.Body.Contains(text));
            query = query.Where(e => !from.HasValue || e.CreatedOn > from.Value);
            if (to.HasValue)
            {
                DateTime _to = new DateTime(to.Value.Year, to.Value.Month, to.Value.Day, 23, 59, 59, 999);
                query = query.Where(e => e.CreatedOn < _to);
            }
            return query.Count();
        }

        public int CreateOrUpdate(Email email)
        {
            if (email.Id == 0)
            {
                email.Status = EmailStatus.New;
                email.CreatedOn = DateTime.Now.ToUniversalTime();

                _context.Emails.Add(email);
            }
            else
            {
                var oldEmail = _context.Emails.FirstOrDefault(c => c.Id == email.Id);

                oldEmail.RecipientEmail = email.RecipientEmail;
                oldEmail.Subject = email.Subject;
                oldEmail.Body = email.Body;
            }

            _context.SaveChanges();

            return email.Id;
        }

        public int Sent(int emailId)
        {
            var oldEmail = _context.Emails.FirstOrDefault(c => c.Id == emailId);

            oldEmail.Status = EmailStatus.Sent;
            oldEmail.SentOn = DateTime.Now.ToUniversalTime();

            _context.SaveChanges();

            return oldEmail.Id;
        }

        public bool Delete(int id)
        {
            var email = _context.Emails.FirstOrDefault(c => c.Id == id);
            if (email != null)
            {
                _context.Emails.Remove(email);
            }
            _context.SaveChanges();

            return true;
        }

        public List<Email> GetAllToSend()
        {
            return _context.Emails.Where(e => e.SentOn == null).OrderByDescending(b => b.Id).ToList();
        }

        public void SaveAll(List<Email> emailsToSend)
        {
            _context.SaveChanges();
        }

        public void CreateNotifications(Booking booking, Func<string, string, object, string> funcUrl, bool isChanged, bool isForgottenAccount = false)
        {
            //Notification

            var clinic = _clinicManager.GetById(booking.ClinicId);
            var treatment = _treatmentManager.GetById(booking.TreatmentId);
            var admins = _userManager.GetAdmins();

            try
            {
                //admins
                string subject = isChanged ? string.Format("Booking updated ({0} / {1} @ {2})",
                    clinic.BusinessName,
                    treatment.Name,
                    booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                    + booking.BookDateTime.ToLocalTime().ToShortTimeString()) :
                string.Format("New booking created ({0} / {1} @ {2})",
                    clinic.BusinessName,
                    treatment.Name,
                    booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                    + booking.BookDateTime.ToLocalTime().ToShortTimeString());
                string body = isChanged
                    ? string.Format(Properties.Resources.BookingUpdatedAdmin,
                        funcUrl("AdminEdit", "Booking", new { id = booking.Id }), clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName),
                        booking.ClientPhone, booking.ClientEmail)
                    : string.Format(Properties.Resources.BookingCreatedAdmin,
                        funcUrl("AdminEdit", "Booking", new { id = booking.Id }), clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName),
                        booking.ClientPhone, booking.ClientEmail);
                foreach (var admin in admins)
                {
                    CreateOrUpdate(new Email
                    {
                        Status = EmailStatus.New,
                        CreatedOn = DateTime.Now.ToUniversalTime(),

                        Subject = subject,
                        Body = body,
                        RecipientEmail = admin.Email,
                        RecipientId = admin.Id
                    });
                }

                //client
                body = isChanged
                    ? string.Format(Properties.Resources.BookingUpdatedClient,
                        funcUrl("Edit", "Booking", new { id = booking.Id }),
                        clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName), booking.ClientPhone,
                        booking.ClientEmail)
                    : (isForgottenAccount ?
                        string.Format(Properties.Resources.BookingCreatedClientForgotten,
                        funcUrl("Edit", "Booking", new { id = booking.Id }),
                        clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName), booking.ClientPhone,
                        booking.ClientEmail, funcUrl("Login", "Account", null), funcUrl("ForgotPassword", "Account", null))
                        :
                        string.Format(Properties.Resources.BookingCreatedClient,
                        funcUrl("Edit", "Booking", new { id = booking.Id }),
                        clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName), booking.ClientPhone,
                        booking.ClientEmail));
                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = string.Format(subject,
                        clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                     + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                    Body = body,
                    RecipientEmail = booking.ClientEmail,
                    RecipientId = booking.UserId
                });

                //clinic
                body = isChanged
                    ? string.Format(Properties.Resources.BookingUpdatedClinic, funcUrl("View", "Booking", booking.Id),
                        clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName), booking.ClientPhone,
                        booking.ClientEmail,
                        funcUrl("ConfirmBooking", "Booking", new { key = booking.ApproveKey }),
                        funcUrl("DeclineBooking", "Booking", new { key = booking.DeclineKey }))
                    : string.Format(Properties.Resources.BookingCreatedClinic, funcUrl("View", "Booking", booking.Id),
                        clinic.BusinessName,
                        booking.BookDateTime.ToShortDateString() + " " +
                        booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Concat(booking.FirstName, " ", booking.LastName), booking.ClientPhone,
                        booking.ClientEmail,
                        funcUrl("ConfirmBooking", "Booking", new { key = booking.ApproveKey }),
                        funcUrl("DeclineBooking", "Booking", new { key = booking.DeclineKey }));
                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = string.Format(subject,
                        clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                     + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                    Body = body,
                    RecipientEmail = clinic.EmailAddressForOnlineBookings
                });
            }
            catch (DbEntityValidationException ex)
            {
                LoggerHelper.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        public void CreateReceiptPaymentNotifications(Booking booking)
        {
            //Notification

            var clinic = _clinicManager.GetById(booking.ClinicId);
            var treatment = _treatmentManager.GetById(booking.TreatmentId);
            var admins = _userManager.GetAdmins();

            try
            {
                string subject = string.Format("Booking has been paid ({0}, {1} in {2})",
                    booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                        + booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                    clinic.BusinessName,
                    clinic.City);
                string body = string.Format(Properties.Resources.ReceiptPayment,
                        clinic.BusinessName,
                        clinic.Phone,
                        clinic.Address,
                        clinic.City,
                        clinic.State,
                        booking.BookDateTime.ToShortDateString() + " " + booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name,
                        clinic.Email,
                        treatment.Price,
                        booking.Payments.Sum(p => p.Total));

                //admins
                foreach (var admin in admins)
                {
                    CreateOrUpdate(new Email
                    {
                        Status = EmailStatus.New,
                        CreatedOn = DateTime.Now.ToUniversalTime(),
                        Subject = subject,
                        Body = body,
                        RecipientEmail = admin.Email,
                        RecipientId = admin.Id
                    });
                }

                //client
                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = string.Format(subject,
                        clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                     + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                    Body = body,
                    RecipientEmail = booking.ClientEmail,
                    RecipientId = booking.UserId
                });
            }
            catch (DbEntityValidationException ex)
            {
                LoggerHelper.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        public void CreateCancelNotifications(Booking booking)
        {
            if (booking != null)
            {
                //Cancel notification

                var clinic = _clinicManager.GetById(booking.ClinicId);
                var treatment = _treatmentManager.GetById(booking.TreatmentId);
                var admins = _userManager.GetAdmins();

                try
                {
                    //admins
                    string subject = string.Format("Booking has been cancelled ({0}; {1} in {2})",
                        booking.BookDateTime, clinic.BusinessName, clinic.City);
                    string body = string.Format(Properties.Resources.BookingCanceled, clinic.BusinessName, clinic.Phone,
                        clinic.Address, clinic.City, clinic.State,
                        booking.BookDateTime.ToLocalTime().ToShortDateString() + " " + booking.BookDateTime.ToLocalTime().ToShortTimeString(),
                        treatment.Name, string.Format("{0} {1}", booking.FirstName, booking.LastName),
                        booking.ClientPhone, booking.ClientEmail);
                    foreach (var admin in admins)
                    {
                        CreateOrUpdate(new Email
                        {
                            Status = EmailStatus.New,
                            CreatedOn = DateTime.Now.ToUniversalTime(),

                            Subject = subject,
                            Body = body,
                            RecipientEmail = admin.Email,
                            RecipientId = admin.Id
                        });
                    }

                    //client
                    CreateOrUpdate(new Email
                    {
                        Status = EmailStatus.New,
                        CreatedOn = DateTime.Now.ToUniversalTime(),
                        Subject = string.Format(subject,
                            clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                         + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                        Body = body,
                        RecipientEmail = booking.ClientEmail,
                        RecipientId = booking.UserId
                    });

                    //clinic
                    CreateOrUpdate(new Email
                    {
                        Status = EmailStatus.New,
                        CreatedOn = DateTime.Now.ToUniversalTime(),
                        Subject = string.Format(subject,
                            clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                         + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                        Body = body,
                        RecipientEmail = clinic.EmailAddressForOnlineBookings
                    });
                }
                catch (DbEntityValidationException ex)
                {
                    LoggerHelper.LogException(ex);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogException(ex);
                }
            }
        }

        public void CreateRefundNotification(Booking booking, decimal refundSum)
        {
            //Refund notification

            var clinic = _clinicManager.GetById(booking.ClinicId);
            var treatment = _treatmentManager.GetById(booking.TreatmentId);
            var admins = _userManager.GetAdmins();
            var payments = _paymentManager.GetByBookingId(booking.Id);

            try
            {
                //admins
                string subject = "The refund has been made";
                string body = string.Format(Properties.Resources.BookingRefund, clinic.BusinessName, clinic.Phone,
                    clinic.Address, clinic.City, clinic.State, booking.BookDateTime, treatment.Name,
                    treatment.Price, payments.Where(p => !p.IsSuccessTransaction).Sum(p => p.Total),
                    string.Format("{0} {1}", booking.FirstName, booking.LastName),
                    booking.ClientPhone, booking.ClientEmail, refundSum);
                foreach (var admin in admins)
                {
                    CreateOrUpdate(new Email
                    {
                        Status = EmailStatus.New,
                        CreatedOn = DateTime.Now.ToUniversalTime(),

                        Subject = subject,
                        Body = body,
                        RecipientEmail = admin.Email,
                        RecipientId = admin.Id
                    });
                }

                //client
                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = string.Format(subject,
                        clinic.BusinessName, treatment.Name, booking.BookDateTime.ToLocalTime().ToShortDateString() + " "
                                                     + booking.BookDateTime.ToLocalTime().ToShortTimeString()),
                    Body = body,
                    RecipientEmail = booking.ClientEmail,
                    RecipientId = booking.UserId
                });
            }
            catch (DbEntityValidationException ex)
            {
                LoggerHelper.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        public void CreateRegisterNotification(string email, string callbackUrl, string firstName, string lastName, string password = null, string loginLink = null)
        {
            try
            {
                string body = password == null
                    ? string.Format(Properties.Resources.EmailConfirmation, firstName, lastName, email, callbackUrl)
                    : string.Format(Properties.Resources.CreationAndEmailConfirmation, firstName, lastName, loginLink, email, password, callbackUrl);
                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = password == null ? "PureSmile account registration" : "PureSmile account has been created",
                    Body = body,
                    RecipientEmail = email
                });
            }
            catch (DbEntityValidationException ex)
            {
                LoggerHelper.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        public void CreateBookingNotification(string firstName, string lastName, string email, int clinicId, int treatmentId, string date, string time)
        {
            string clinic = _context.Clinics.FirstOrDefault(x => x.Id == clinicId).BusinessName;
            string treatment = _context.Treatments.FirstOrDefault(x => x.Id == treatmentId).Name;
            try
            {
                string body = string.Format(Properties.Resources.BookingNotofication, firstName, lastName, treatment, clinic, date, time);

                CreateOrUpdate(new Email
                {
                    Status = EmailStatus.New,
                    CreatedOn = DateTime.Now.ToUniversalTime(),
                    Subject = "PureSmile. Booking notification",
                    Body = body,
                    RecipientEmail = email
                });
            }
            catch (DbEntityValidationException ex)
            {
                LoggerHelper.LogException(ex);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }
    }
}