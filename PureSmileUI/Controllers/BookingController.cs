using DatabaseContext.Managers;
using DatabaseContext.Models;
using DatabaseContext.Models.Enums;
using Microsoft.AspNet.Identity;
using PureSmileUI.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Braintree;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using PureSmileUI.App_Start;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class BookingController : BaseController
    {
        private BookingManager Manager = new BookingManager();
        private TreatmentManager TreatmentManager = new TreatmentManager();
        private TreatmentCategoryManager TreatmentCategoryManager = new TreatmentCategoryManager();
        private ClinicManager ClinicManager = new ClinicManager();
        private UserManager UserManager = new UserManager();
        private PaymentManager PaymentManager = new PaymentManager();
        private BraintreeHelper brainTreetHelper = new BraintreeHelper();
        private EmailManager emailManager = new EmailManager();
        private ApplicationUserManager _appUserManager;

        public ApplicationUserManager AppUserManager
        {
            get { return _appUserManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _appUserManager = value; }
        }


        [Authorize(Roles = "SuperUser, Staff")]
        public ActionResult AdminBookingList()
        {
            return View("AdminBookingListView");
        }

        [Authorize(Roles = "Client")]
        public ActionResult UserBookingList()
        {
            return View("UserBookingListView");
        }

        [AllowAnonymous]
        public ActionResult Edit(int id)
        {
            var bookingItem = GetBookingEditItem(id);
            if (User.Identity.GetUserId<int>() != bookingItem.UserId)
            {
                return RedirectToHome();
            }
            return View("BookingEditView", bookingItem);
        }

        private ActionResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

        private BookingEditItem GetBookingEditItem(int id)
        {
            BookingEditItem bookingItem = new BookingEditItem();

            bookingItem.TreatmentList = TreatmentManager.GetAllActive().Select(c => new TreatmentItem
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(c => c.Name).ToArray();

            bookingItem.ClinicList = ClinicManager.GetAllActive().Select(c => new ClinicItem
            {
                Id = c.Id,
                Name = c.BusinessName
            }).OrderBy(c => c.Name).ToArray();

            bookingItem.TimeList = new ValueItem[0];

            if (id != 0)
            {
                var booking = Manager.GetById(id);
                if (booking != null)
                {
                    var bookingDateTime = booking.BookDateTime.ToLocalTime();

                    bookingItem.Id = booking.Id;
                    bookingItem.BookDate = bookingDateTime.Date;
                    bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                        bookingDateTime.TimeOfDay.ToString("mm"));
                    bookingItem.Status = (BookingStatusEnum)booking.StatusId;
                    bookingItem.Comments = booking.Comments;
                    bookingItem.ClinicId = booking.ClinicId;
                    bookingItem.TreatmentId = booking.TreatmentId;
                    bookingItem.UserId = booking.UserId;
                    bookingItem.FirstName = booking.FirstName;
                    bookingItem.LastName = booking.LastName;
                    bookingItem.ClientEmail = booking.ClientEmail;
                    bookingItem.ClientPhone = booking.ClientPhone;
                    bookingItem.IsPaid = booking.IsPaid;
                    bookingItem.HasPayments = booking.Payments.Any();

                    var hoursList = ClinicManager.GetClinicHoursListOnDay(booking.ClinicId, bookingDateTime.Date.DayOfWeek);

                    if (hoursList != null)
                    {
                        bookingItem.TimeList = GetHoursList(hoursList, bookingItem.BookDate).ToArray();
                    }
                }
            }
            else
            {
                var bookingDateTime = DateTime.Now;

                bookingItem.Status = BookingStatusEnum.Created;
                bookingItem.UserId = User.Identity.GetUserId<int>();
                var currentUser = UserManager.GetById(bookingItem.UserId);
                if (currentUser != null)
                {
                    bookingItem.ClientEmail = currentUser.Email;
                    if (currentUser.ClientData != null)
                    {
                        bookingItem.FirstName = currentUser.ClientData.FirstName;
                        bookingItem.LastName = currentUser.ClientData.LastName;
                    }
                    else
                    {
                        bookingItem.FirstName = currentUser.UserName;
                    }
                    bookingItem.ClientPhone = currentUser.PhoneNumber;
                }
                bookingItem.BookDate = bookingDateTime;
                bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                        bookingDateTime.TimeOfDay.ToString("mm"));
            }
            return bookingItem;
        }

        [AllowAnonymous]
        public ActionResult BookingStep2(int id)
        {
            ViewBag.ClientToken = GenerateToken();

            return View("BookingStep2", GetBookingViewItem(id));
        }

        [Authorize(Roles = "SuperUser,Staff,Client")]
        private BookingViewItem GetBookingViewItem(int id)
        {
            BookingViewItem bookingItem = new BookingViewItem();

            var booking = Manager.GetById(id);
            if (booking != null)
            {
                bookingItem.Id = booking.Id;
                bookingItem.Status = (BookingStatusEnum)booking.StatusId;
                bookingItem.Comments = booking.Comments;
                bookingItem.ClinicId = booking.ClinicId;
                bookingItem.TreatmentId = booking.TreatmentId;
                bookingItem.UserId = booking.UserId;
                bookingItem.FirstName = booking.FirstName;
                bookingItem.LastName = booking.LastName;
                bookingItem.ClientEmail = booking.ClientEmail;
                bookingItem.ClientPhone = booking.ClientPhone;
                bookingItem.BookDateTime = booking.BookDateTime.ToLocalTime();
                bookingItem.ClinicId = booking.ClinicId;
                bookingItem.TreatmentId = booking.TreatmentId;
                bookingItem.ClinicName = ClinicManager.GetById(booking.ClinicId).BusinessName;
                var treatment = TreatmentManager.GetById(booking.TreatmentId);
                bookingItem.TreatmentName = treatment.Name;

                bookingItem.PaymentAmount = treatment.Price;
                bookingItem.IsPaid = booking.IsPaid;
            }
            return bookingItem;
        }

        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult AdminEdit(int id)
        {
            BookingEditItem bookingItem = new BookingEditItem();

            bookingItem.TreatmentList = TreatmentManager.GetAll().Select(c => new TreatmentItem
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price
            }).OrderBy(c => c.Name).ToArray();

            bookingItem.ClinicList = ClinicManager.GetAllActive().Select(c => new ClinicItem
            {
                Id = c.Id,
                Name = c.BusinessName
            }).OrderBy(c => c.Name).ToArray();

            var userList = new List<ValueItem> { new ValueItem { Id = 0, Name = "-" } };
            userList.AddRange(UserManager.GetAllActiveUsers().Select(c => new ValueItem
            {
                Id = c.Id,
                Name = c.Email
            }).ToList());
            bookingItem.UserList = userList.OrderBy(c => c.Name).ToArray();

            bookingItem.TimeList = new ValueItem[0];

            if (id != 0)
            {
                var booking = Manager.GetById(id);
                if (booking != null)
                {
                    var bookingDateTime = booking.BookDateTime.ToLocalTime();

                    bookingItem.Id = booking.Id;
                    bookingItem.BookDate = bookingDateTime.Date;
                    bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                        bookingDateTime.TimeOfDay.ToString("mm"));
                    bookingItem.Status = (BookingStatusEnum)booking.StatusId;
                    bookingItem.Comments = booking.Comments;
                    bookingItem.ClinicId = booking.ClinicId;
                    bookingItem.TreatmentId = booking.TreatmentId;
                    bookingItem.UserId = booking.UserId;
                    bookingItem.FirstName = booking.FirstName;
                    bookingItem.LastName = booking.LastName;
                    bookingItem.ClientEmail = booking.ClientEmail;
                    bookingItem.ClientPhone = booking.ClientPhone;
                    bookingItem.ApproveKey = booking.ApproveKey;
                    bookingItem.DeclineKey = booking.DeclineKey;
                    bookingItem.IsPaid = booking.IsPaid;
                    bookingItem.StaffComments = booking.StaffComments;
                    bookingItem.HasPayments = booking.Payments.Any();
                    if (booking.PaidToClinicOn.HasValue)
                    {
                        bookingItem.PaidToClinicOn = booking.PaidToClinicOn.Value.ToLocalTime();
                    }
                    else
                    {
                        bookingItem.PaidToClinicOn = DateTime.Now;
                    }
                    bookingItem.PaidToClinicUserName = booking.PaidToClinicByUser?.UserName;

                    var hoursList = ClinicManager.GetClinicHoursListOnDay(booking.ClinicId, bookingDateTime.Date.DayOfWeek);

                    if (hoursList != null)
                    {
                        bookingItem.TimeList = GetHoursList(hoursList, bookingDateTime.Date).ToArray();
                    }
                }
            }
            else
            {
                var bookingDateTime = DateTime.Now;

                bookingItem.Status = BookingStatusEnum.Created;
                bookingItem.BookDate = bookingDateTime;
                bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                    bookingDateTime.TimeOfDay.ToString("mm"));
            }

            ViewBag.MaxAmountToRefund = id == 0 ? "0" : PaymentManager.GetMaxAmountToRefund(id).ToString("G");
            return View("AdminBookingEditView", bookingItem);
        }

        [AllowAnonymous]
        public ActionResult ModalBookingEdit(int id)
        {
            BookingEditItem bookingItem = new BookingEditItem();

            bookingItem.TreatmentList = TreatmentManager.GetAllActive().Select(c => new TreatmentItem
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                PictureUrl = c.PictureUrl,
                TreatmentCategoryId = c.TreatmentCategory.Id
            }).OrderBy(c => c.Name).ToArray();

            bookingItem.TreatmentCategoriesList = TreatmentCategoryManager.GetAll().Where(x => x.IsActive).Select(t => new TreatmentCategoryItem
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                PictureUrl = t.PictureUrl,
            }).OrderBy(c => c.Name).ToArray();

            bookingItem.ClinicList = ClinicManager.GetAllActive().Select(c => new ClinicItem
            {
                Id = c.Id,
                Name = c.BusinessName,
                Address = c.Address,
                City = c.City,
                State = c.State,
                OpenClinicHourses = c.Hours,
                Lat = c.Lat,
                Lng = c.Lng,
                TreatmentCategories = c.TreatmentCategories.Select(x => x.Id).ToList()
            }).OrderBy(c => c.Name).ToArray();

            var userList = new List<ValueItem> { new ValueItem { Id = 0, Name = "-" } };
            userList.AddRange(UserManager.GetAllActiveUsers().Select(c => new ValueItem
            {
                Id = c.Id,
                Name = c.Email
            }).ToList());
            bookingItem.UserList = userList.OrderBy(c => c.Name).ToArray();

            bookingItem.TimeList = new ValueItem[0];

            if (id != 0)
            {
                var booking = Manager.GetById(id);
                if (booking != null)
                {
                    var bookingDateTime = booking.BookDateTime.ToLocalTime();

                    bookingItem.Id = booking.Id;
                    bookingItem.BookDate = bookingDateTime.Date;
                    bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                        bookingDateTime.TimeOfDay.ToString("mm"));
                    bookingItem.Status = (BookingStatusEnum)booking.StatusId;
                    bookingItem.Comments = booking.Comments;
                    bookingItem.ClinicId = booking.ClinicId;
                    bookingItem.TreatmentId = booking.TreatmentId;
                    bookingItem.UserId = booking.UserId;
                    bookingItem.FirstName = booking.FirstName;
                    bookingItem.LastName = booking.LastName;
                    bookingItem.ClientEmail = booking.ClientEmail;
                    bookingItem.ClientPhone = booking.ClientPhone;
                    bookingItem.ApproveKey = booking.ApproveKey;
                    bookingItem.DeclineKey = booking.DeclineKey;
                    bookingItem.IsPaid = booking.IsPaid;
                    bookingItem.StaffComments = booking.StaffComments;
                    bookingItem.HasPayments = booking.Payments.Any();

                    var hoursList = ClinicManager.GetClinicHoursListOnDay(booking.ClinicId, bookingDateTime.Date.DayOfWeek);

                    if (hoursList != null)
                    {
                        bookingItem.TimeList = GetHoursList(hoursList, bookingItem.BookDate.Date).ToArray();
                    }
                }
            }
            else
            {
                var bookingDateTime = DateTime.Now;

                bookingItem.Status = BookingStatusEnum.Created;
                bookingItem.BookDate = bookingDateTime;
                bookingItem.BookTime = string.Format("{0}:{1}", bookingDateTime.TimeOfDay.ToString("hh"),
                    bookingDateTime.TimeOfDay.ToString("mm"));
                if (User.Identity.IsAuthenticated)
                {
                    var user = UserManager.GetById(User.Identity.GetUserId<int>());
                    bookingItem.ClientEmail = user.Email;
                    bookingItem.ClientPhone = user.PhoneNumber;
                    if (user.ClientData != null)
                    {
                        bookingItem.FirstName = user.ClientData.FirstName;
                        bookingItem.LastName = user.ClientData.LastName;
                    }
                }
            }

            ViewBag.MaxAmountToRefund = id == 0 ? "0" : PaymentManager.GetMaxAmountToRefund(id).ToString("G");

            ViewBag.ClientToken = GenerateToken();
            return View("ModalBookingEdit", bookingItem);
        }

        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult TreatmentModalSelect()
        {
            return PartialView("TreatmentModalSelectView");
        }

        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult ClinicModalSelect()
        {
            return PartialView("ClinicModalSelectView");
        }

        [Authorize(Roles = "SuperUser,Staff,Client")]
        public ActionResult View(Booking booking)
        {
            BookingItem bookingItem = new BookingItem();

            if (booking != null)
            {
                var bookingDateTime = booking.BookDateTime.ToLocalTime();

                bookingItem.Id = booking.Id;
                bookingItem.BookDateTime = bookingDateTime.ToLocalTime();
                bookingItem.Status = (BookingStatusEnum)booking.StatusId;
                bookingItem.Comments = booking.Comments;
                bookingItem.ClinicId = booking.ClinicId;
                bookingItem.TreatmentId = booking.TreatmentId;
                bookingItem.UserId = booking.UserId;
                bookingItem.FirstName = booking.FirstName;
                bookingItem.LastName = booking.LastName;
                bookingItem.ClientEmail = booking.ClientEmail;
                bookingItem.ClientPhone = booking.ClientPhone;
                bookingItem.IsPaid = booking.IsPaid;
            }

            return View("BookingView", bookingItem);
        }

        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult ChangeStatusBooking(Guid key)
        {
            var booking = Manager.ApproveOrDeclineBooking(key);
            if (booking == null)
            {
                return View("ChangeStatusBooking", null);
            }

            var bookingItem = new BookingItem
            {
                FirstName = booking.FirstName,
                LastName = booking.LastName,
                BookDateTime = booking.BookDateTime.ToLocalTime(),
                Status = (BookingStatusEnum)booking.StatusId,
                Comments = booking.Comments,
                ClinicName = booking.Clinic.BusinessName,
                TreatmentName = booking.Treatment.Name
            };
            return View("BookingView", bookingItem);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Save(BookingEditItem booking)
        {
            TimeSpan bookTime = TimeSpan.Parse(booking.BookTime);
            DateTime bookDateTime = booking.BookDate;
            bookDateTime = new DateTime(booking.BookDate.Year, booking.BookDate.Month, booking.BookDate.Day,
                bookTime.Hours, bookTime.Minutes, 0).ToUniversalTime();

            if (ModelState.IsValid && IsValidBookingDateTime(booking, bookDateTime))
            {
                bool isForgottenAccount = false;

                // not registered
                if (booking.UserId == 0)
                {
                    var user = UserManager.GetByLogin(booking.ClientEmail);
                    isForgottenAccount = user != null;
                    CheckUserByBooking(booking, user);
                }

                bool isNew = booking.Id == 0;
                var newBooking = new Booking
                {
                    Id = booking.Id,
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    ClientEmail = booking.ClientEmail,
                    ClientPhone = booking.ClientPhone,
                    StatusId = (int)booking.Status,
                    BookDateTime = bookDateTime.ToUniversalTime(),
                    Comments = booking.Comments,
                    ClinicId = booking.ClinicId,
                    TreatmentId = booking.TreatmentId,
                    UserId = booking.UserId
                };

                bool isChanged = false;
                if (!isNew)
                {
                    var oldBooking = Manager.GetById(booking.Id);
                    isChanged = (oldBooking.FirstName != booking.FirstName ||
                                 oldBooking.LastName != booking.LastName ||
                                 oldBooking.ClientEmail != booking.ClientEmail ||
                                 oldBooking.ClientPhone != booking.ClientPhone ||
                                 oldBooking.BookDateTime != bookDateTime ||
                                 oldBooking.Comments != booking.Comments ||
                                 oldBooking.ClinicId != booking.ClinicId ||
                                 oldBooking.TreatmentId != booking.TreatmentId ||
                                 oldBooking.UserId != booking.UserId);
                }

                int? id = Manager.CreateOrUpdate(newBooking);
                if (!id.HasValue)
                {
                    ModelState.AddModelError("Error", "Wrong booking time. Please select another time.");

                    FillListsForBookingEdit(booking);

                    return View("BookingEditView", booking);
                }
                if (isNew)
                {
                    emailManager.CreateNotifications(newBooking, (action, controller, perms) =>
                    {
                        return Url.Action(action, controller, perms, Request.Url.Scheme);
                    }, false, isForgottenAccount);
                }
                else
                {
                    if (isChanged)
                    {
                        emailManager.CreateNotifications(newBooking, (action, controller, perms) =>
                        {
                            return Url.Action(action, controller, perms, Request.Url.Scheme);
                        }, true);
                    }
                }
                return RedirectToAction("BookingStep2", new { id = newBooking.Id });
            }

            FillListsForBookingEdit(booking);

            return View("BookingEditView", booking);
        }

        private void CheckUserByBooking(BookingEditItem booking, User user)
        {
            string password = null;
            if (user == null) //не нашли
            {
                user = new User { UserName = booking.ClientEmail, Email = booking.ClientEmail };
                password = string.Format("U{0}{1}", Membership.GeneratePassword(6, 3),
                    DateTime.Now.Millisecond.ToString().PadLeft(3, '0'));
                var result = AppUserManager.Create(user, password);
                if (!string.IsNullOrEmpty(booking.FirstName) || string.IsNullOrEmpty(booking.LastName))
                {
                    user.ClientData = new ClientData()
                    {
                        FirstName = booking.FirstName,
                        LastName = booking.LastName
                    };
                }
                AppUserManager.AddToRole(user.Id, "Client");
                // новый пользователь нужно подтвердить email
                var callbackUrl = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, code = UserManager.CreateEmailConfirmation(user.Id) }, Request.Url.Scheme);

                var loginUrl = Url.Action("Login", "Account", null, Request.Url.Scheme);
                emailManager.CreateRegisterNotification(booking.ClientEmail, callbackUrl, booking.FirstName,
                    booking.LastName, password, loginUrl);
            }
            else
            {
                TempData["NotAuthorizedButFound"] = true;
            }
            booking.UserId = user.Id;
        }

        private bool IsValidBookingDateTime(BookingEditItem booking, DateTime bookDateTime)
        {
            var minBookTime = DateTime.Now.AddMinutes(ConfigurationManager.MinimalMinutesFromNowToBook);
            FillListsForBookingEdit(booking);
            if (bookDateTime.ToLocalTime() > minBookTime)
            {
                return true;
            }
            else
            {
                ModelState.AddModelError("Error",
                    $"Wrong booking time. Please select another time greater than {minBookTime.ToShortTimeString()}");
                return false;
            }
        }

        [HttpPost, ActionName("AdminEdit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult AdminSave(BookingEditItem booking)
        {
            TimeSpan bookTime = TimeSpan.Parse(booking.BookTime);
            DateTime bookDateTime = booking.BookDate;

            bookDateTime = new DateTime(booking.BookDate.Year, booking.BookDate.Month, booking.BookDate.Day,
                bookTime.Hours, bookTime.Minutes, 0).ToUniversalTime();

            if (ModelState.IsValid && IsValidBookingDateTime(booking, bookDateTime))
            {
                var newBooking = new Booking
                {
                    Id = booking.Id,
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    ClientEmail = booking.ClientEmail,
                    ClientPhone = booking.ClientPhone,
                    StatusId = (int)booking.Status,
                    BookDateTime = bookDateTime.ToUniversalTime(),
                    Comments = booking.Comments,
                    ClinicId = booking.ClinicId,
                    TreatmentId = booking.TreatmentId,
                    UserId = booking.UserId != 0 ? booking.UserId : User.Identity.GetUserId<int>(),
                    IsPaid = booking.IsPaid,
                    StaffComments = booking.StaffComments
                };

                Manager.CreateOrUpdate(newBooking);
                return BackToList();
            }

            booking.TreatmentList = TreatmentManager.GetAll().Select(c => new TreatmentItem
            {
                Id = c.Id,
                Name = c.Name
            }).ToArray();

            booking.ClinicList = ClinicManager.GetAllActive().Select(c => new ClinicItem
            {
                Id = c.Id,
                Name = c.BusinessName
            }).ToArray();

            var userList = new List<ValueItem> { new ValueItem { Id = 0, Name = "-" } };
            userList.AddRange(UserManager.GetAllActive().Select(c => new ValueItem
            {
                Id = c.Id,
                Name = c.Email
            }).ToList());
            booking.UserList = userList.OrderBy(c => c.Name).ToArray();

            var hoursList = ClinicManager.GetClinicHoursListOnDay(booking.ClinicId, booking.BookDate.DayOfWeek);
            if (hoursList != null)
            {
                booking.TimeList = GetHoursList(hoursList, bookDateTime.Date).ToArray();
            }

            return View("AdminBookingEditView", booking);
        }

        [Authorize(Roles = "SuperUser,Staff")]
        public ActionResult PaymentReceived(BookingEditItem booking)
        {
            Manager.SetPaid(booking.Id);
            return RedirectToAction("AdminEdit", new { id = booking.Id });
        }

        /// <summary>
        /// Display payment status depend on transacations of booking
        /// </summary>
        /// <param name="bookingId"></param>
        /// <returns></returns>
        public string CheckPaymentStatus(int bookingId)
        {
            List<Payment> allTranscations = PaymentManager.GetByBookingId(bookingId);
            List<Payment> SuccessTransactions = allTranscations.Where(x => x.IsSuccessTransaction).ToList();

            if (SuccessTransactions.Count > 0)
                return "true"; //True - there success transcation

            if (allTranscations.Count > 0 && SuccessTransactions.Count == 0) return "false"; //False - there transactions, but no any success

            return ""; //Empty  - no any transacation
        }

        [HttpGet]
        [Authorize(Roles = "SuperUser,Staff")]
        public JsonResult GetAdminBookingList(int page, int rows, DateTime? bookDate, int? paidStatus, string text)
        {
            if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
            {
                var bookingListDB = Manager.GetAll(null, null, null, page, rows).AsQueryable();
                var bookingsCount = Manager.GetCount(null, null, null);

                bookingListDB = FilterBookings(bookDate, paidStatus, text, bookingListDB);

                var bookingList = bookingListDB.ToList().Select(b => new BookingItem
                {
                    Id = b.Id,
                    FirstName = b.FirstName,
                    LastName = b.LastName,
                    ClientEmail = b.ClientEmail,
                    ClientPhone = b.ClientPhone,
                    BookDateTime = b.BookDateTime.ToLocalTime(),
                    Status = (BookingStatusEnum)b.StatusId,
                    Comments = b.Comments,
                    ClinicId = b.ClinicId,
                    ClinicName = b.Clinic.BusinessName,
                    TreatmentId = b.TreatmentId,
                    TreatmentName = b.Treatment.Name,
                    TreatmentCategory = TreatmentCategoryManager.GetById(b.Treatment.TreatmentCategoryId).Name,
                    UserId = b.UserId,
                    UserName = b.User.UserName,
                    IsPaid = b.IsPaid,
                    PaymentSuccess = CheckPaymentStatus(b.Id),
                    PaymentDetailsUrl = Url.Action("View", "Payments", new { id = b.Id }, Request.Url.Scheme),
                    PaidToClinicOn = b.PaidToClinicOn,
                    PaidToClinicByUserName = b.PaidToClinicByUser?.UserName
                })
                .OrderByDescending(c => c.Id)
                .ToList();

                var jsonData = new
                {
                    total = Math.Ceiling(bookingsCount / (double)rows),
                    page = page,
                    records = bookingsCount,
                    rows = bookingList
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [HttpGet]
        [Authorize(Roles = "SuperUser,Staff,Client")]
        public JsonResult GetUserBookingList(DateTime? bookDate, int? paidStatus, string text)
        {

            var userId = User.Identity.GetUserId<int>();
            var bookingListDB = Manager.GetAllByUser(userId).AsQueryable();

            bookingListDB = FilterBookings(bookDate, paidStatus, text, bookingListDB);

            var bookingList = bookingListDB.ToList().Select(b => new BookingItem
            {
                Id = b.Id,
                FirstName = b.FirstName,
                LastName = b.LastName,
                ClientEmail = b.ClientEmail,
                ClientPhone = b.ClientPhone,
                Status = (BookingStatusEnum)b.StatusId,
                Comments = b.Comments,
                BookDateTime = b.BookDateTime.ToLocalTime(),
                ClinicId = b.ClinicId,
                ClinicName = b.Clinic.BusinessName,
                TreatmentId = b.TreatmentId,
                TreatmentName = b.Treatment.Name,
                TreatmentCategory = TreatmentCategoryManager.GetById(b.Treatment.TreatmentCategoryId).Name,
                UserId = userId,
                IsPaid = b.IsPaid,
                PaymentSuccess = CheckPaymentStatus(b.Id)
            })
            .OrderByDescending(c => c.Id);


            return Json(bookingList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Apply filter to bookings
        /// </summary>
        /// <param name="bookDate"></param>
        /// <param name="paidStatus"></param>
        /// <param name="text"></param>
        /// <param name="bookingListDB"></param>
        /// <returns></returns>
        private static IQueryable<Booking> FilterBookings(DateTime? bookDate, int? paidStatus, string text, IQueryable<Booking> bookingListDB)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = text.ToLower().Trim();

                bookingListDB = bookingListDB.Where(x => x.Treatment.Name.ToLower().Contains(text) ||
                x.Clinic.BusinessName.ToLower().Contains(text) ||
                x.ClientEmail.ToLower().Contains(text)
                );
            }

            if (paidStatus.HasValue)
            {
                switch (paidStatus.Value)
                {

                    case 0: // Show not paid
                        bookingListDB = bookingListDB.Where(x => !x.IsPaid);
                        break;
                    case 1: //Show only paid
                        bookingListDB = bookingListDB.Where(x => x.IsPaid);
                        break;
                    default:
                        // no filter - show all

                        break;
                }
            }



            if (bookDate.HasValue)
            {
                DateTime fixedDate = new DateTime(bookDate.Value.Year, bookDate.Value.Month, bookDate.Value.Day, bookDate.Value.Hour, bookDate.Value.Minute, 0, 0);

                if (fixedDate.Hour == 0 && fixedDate.Minute == 0)
                    bookingListDB = bookingListDB.Where(x => x.BookDateTime >= fixedDate && x.BookDateTime < fixedDate.AddDays(1));
                else
                    bookingListDB = bookingListDB.Where(x => x.BookDateTime == fixedDate);


            }

            return bookingListDB;
        }

        [Authorize(Roles = "SuperUser,Staff,Client")]
        public ActionResult Delete(int id)
        {
            bool isDeleted = Manager.Delete(id);
            return BackToList();
        }

        public ActionResult Cancel(int id)
        {
            if (id != 0)
            {
                Manager.Cancel(id);
                var booking = Manager.GetById(id);
                emailManager.CreateCancelNotifications(booking);
                return RedirectToAction(User.IsInRole("SuperUser") || User.IsInRole("Staff") ? "AdminEdit" : "Edit", new { id = id });
            }
            return BackToList();
        }

        [Authorize(Roles = "SuperUser,Staff,Client")]
        public ActionResult BackToList()
        {
            if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
            {
                return RedirectToAction("AdminBookingList");
            }
            return RedirectToAction("UserBookingList");
        }

        private string GenerateToken()
        {
            try
            {
                var gateway = brainTreetHelper.GetGateway();
                var clientToken = gateway.ClientToken.generate();
                return clientToken;
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex, "GenerateToken error.");
            }
            return null;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult BookingFinish(int id)
        {
            decimal amount = 0;
            amount = TreatmentManager.GetPriceByBookingId(id);
            var booking = GetBookingViewItem(id);
            if (booking.IsPaid)
            {
                ViewBag.ResultText = "Booking is already paid.";
                ViewBag.ClientToken = GenerateToken();
                ViewBag.IsSuccess = true;
                return View("BookingStep2", booking);
            }
            int userId = booking.UserId;

            string nonce;
            string payload = Request["payload"];
            JObject jobj = JObject.Parse(payload);
            nonce = jobj.GetValue("nonce").Value<string>();

            try
            {
                var result = ProcessPaymentInternal(nonce, amount, id, userId);

                if (result.IsSuccess())
                {
                    var originBooking = Manager.GetById(booking.Id);
                    emailManager.CreateReceiptPaymentNotifications(originBooking);

                    ViewBag.ResultText = "Successful payment.";
                    booking.IsPaid = true; //todo: костыль. поправить надо 
                    ViewBag.ClientToken = GenerateToken();
                    return View("BookingStep2", booking);
                }
                else
                {
                    ViewBag.ResultText = string.Format("Payment error. Details: {0}", result.Message);
                    ViewBag.ClientToken = GenerateToken();
                    return View("BookingStep2", booking);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
                // TODO: return payment exception view
            }
            return null;
        }

        [HttpGet]
        public ActionResult ConfirmBooking(Guid key)
        {
            if (Manager.ApproveOrDeclineBooking(key) == null)
            {
                return View("ErrorBooking");
            }
            return View("ConfirmBooking");
        }

        [HttpGet]
        public ActionResult DeclineBooking(Guid key)
        {
            if (Manager.ApproveOrDeclineBooking(key) == null)
            {
                return View("ErrorBooking");
            }
            return View("DeclineBooking");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult BookingLoading()
        {
            return View("BookingLoading");
        }

        [HttpPost, ActionName("Refund")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Refund(BookingEditItem booking)
        {
            PaymentManager.Refund(booking.Id, booking.RefundSum, User.Identity.GetUserId<int>());
            emailManager.CreateRefundNotification(Manager.GetById(booking.Id), booking.RefundSum);
            return RedirectToAction("View", "Payments", new { id = booking.Id });
        }

        [HttpPost, ActionName("Completed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Completed(BookingEditItem booking)
        {
            if (ModelState.IsValid)
            {
                if (!booking.PaidToClinicOn.HasValue)
                {
                    ModelState.AddModelError("Error", "Wrong Paid to clinic on date.");
                }
                else
                {
                    Manager.CompleteBooking(booking.Id, User.Identity.GetUserId<int>(), booking.PaidToClinicOn);
                }
            }
            return RedirectToAction("AdminEdit", "Booking", new { id = booking.Id });
        }

        [HttpPost, ActionName("Undo")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Undo(BookingEditItem booking)
        {
            Manager.UndoBookingComplete(booking.Id);
            return RedirectToAction("AdminEdit", "Booking", new { id = booking.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetTimeList(int clinic, DateTime date, bool? isFirstTime)
        {
            List<HoursListJson> _list = new List<HoursListJson>();
            int i = 0;
            List<DateTime> bookedDates = Manager.GetBookedDates(clinic, date, date.AddDays(7));
            do
            {
                ClinicHours hoursList = ClinicManager.GetClinicHoursListOnDay(clinic, date.DayOfWeek);
                _list = hoursList == null ? new List<HoursListJson>() : GetHoursList(hoursList, date, bookedDates);
                if (_list.Count == 0)
                {
                    date = date.AddDays(1);
                }
            } while (isFirstTime.HasValue && isFirstTime.Value && _list.Count(t => !t.IsBooked) == 0 && i++ < 7);

            object jsonData;
            if (isFirstTime.HasValue && isFirstTime.Value)
            {
                jsonData = new
                {
                    list = _list,
                    date = date.ToString("yyyy-MM-dd")
                };
            }
            else
            {
                jsonData = new
                {
                    list = _list
                };
            }

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveBookingAsync(BookingEditItem booking)
        {
            try
            {
                bool isForgottenAccount = false;

                TimeSpan bookTime = TimeSpan.Parse(booking.BookTime);
                DateTime bookDateTime = booking.BookDate;
                bookDateTime = new DateTime(booking.BookDate.Year, booking.BookDate.Month, booking.BookDate.Day,
                    bookTime.Hours, bookTime.Minutes, 0);

                // not registered
                if (!User.Identity.IsAuthenticated || booking.UserId == 0)
                {
                    var user = UserManager.GetByLogin(booking.ClientEmail);
                    isForgottenAccount = user != null;
                    CheckUserByBooking(booking, user);
                }

                var newBooking = new Booking
                {
                    Id = booking.Id,
                    FirstName = booking.FirstName,
                    LastName = booking.LastName,
                    ClientEmail = booking.ClientEmail,
                    ClientPhone = booking.ClientPhone,
                    StatusId = (int)booking.Status,
                    BookDateTime = bookDateTime,
                    Comments = booking.Comments,
                    ClinicId = booking.ClinicId,
                    TreatmentId = booking.TreatmentId,
                    UserId = booking.UserId
                };

                int? id = Manager.CreateOrUpdate(newBooking);
                if (!id.HasValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Wrong booking time. Please select another time."
                    }, JsonRequestBehavior.AllowGet);
                }

                emailManager.CreateNotifications(newBooking, (action, controller, perms) =>
                {
                    return Url.Action(action, controller, perms, Request.Url.Scheme);
                }, false, isForgottenAccount);

                return Json(new
                {
                    success = true,
                    message = "Booking has been created successfully",
                    bookingId = newBooking.Id
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex, "SaveBookingAsync error. ");
                return Json(new
                {
                    success = false,
                    message = "Error occured while processing your request. Please try again later.",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult PayAsync(string payload, int bookingId)
        {
            try
            {
                decimal amount = 0;
                amount = TreatmentManager.GetPriceByBookingId(bookingId);
                var booking = GetBookingViewItem(bookingId);
                if (booking.IsPaid)
                {
                    throw new Exception($"PayAsync error. Booking #{bookingId} is already paid.");
                }
                int userId = booking.UserId;

                var result = ProcessPaymentInternal(payload, amount, bookingId, userId);
                if (result.IsSuccess())
                {
                    return Json(new
                    {
                        success = true,
                        message = "Booking has been created successfully",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Error occured while processing your request. Please try again later",
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex, "SaveBookingAsync error. ");
                return Json(new
                {
                    success = false,
                    message = "Error occured while processing your request. Please try again later",
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<HoursListJson> GetHoursList(ClinicHours hours, DateTimeOffset dateToBook, List<DateTime> bookedDates = null)
        {
            var now = DateTime.Now;
            var returnList = new List<HoursListJson>();
            if (!hours.BookingOpenHour.HasValue || !hours.BookingCloseHour.HasValue)
            {
                return returnList;
            }
            var minHourToBook = now.AddMinutes(ConfigurationManager.MinimalMinutesFromNowToBook);
            DateTime time = new DateTime(dateToBook.Year, dateToBook.Month, dateToBook.Day,
                hours.BookingOpenHour.Value.Hour, hours.BookingOpenHour.Value.Minute,
                hours.BookingOpenHour.Value.Second);
            int id = 0;
            int minutesStep = ConfigurationManager.StepInMinutesToBook;
            var closeHour = new DateTime(dateToBook.Year, dateToBook.Month, dateToBook.Day,
                hours.BookingCloseHour.Value.Hour, hours.BookingCloseHour.Value.Minute,
                hours.BookingCloseHour.Value.Second);

            while (time < closeHour)
            {
                id++;
                if (time > minHourToBook)
                {
                    var value = string.Format("{0}", time.ToString("H:mm"));
                    returnList.Add(new HoursListJson()
                    {
                        Id = value,
                        Name = value,
                        IsBooked = IsDateBooked(time, bookedDates)
                    });
                }
                time = time.AddMinutes(minutesStep);
            };
            return returnList;
        }

        private bool IsDateBooked(DateTime dateToBook, List<DateTime> bookedDates)
        {
            if (bookedDates == null || !bookedDates.Any())
            {
                return false;
            }
            int minutesToBlock = ConfigurationManager.BlockTimeForBookingInMinutes;
            return bookedDates.Any(b => dateToBook >= b && dateToBook < b.AddMinutes(minutesToBlock));
        }

        private void FillListsForBookingEdit(BookingEditItem booking)
        {
            booking.TreatmentList = TreatmentManager.GetAllActive().Select(c => new TreatmentItem
            {
                Id = c.Id,
                Name = c.Name
            }).ToArray();

            booking.ClinicList = ClinicManager.GetAllActive().Select(c => new ClinicItem
            {
                Id = c.Id,
                Name = c.BusinessName
            }).ToArray();

            var hoursList = ClinicManager.GetClinicHoursListOnDay(booking.ClinicId, booking.BookDate.DayOfWeek);
            if (hoursList != null)
            {
                booking.TimeList = GetHoursList(hoursList, booking.BookDate.Date).ToArray();
            }
        }

        private Result<Transaction> ProcessPaymentInternal(string nonce, decimal amount, int bookingId, int userId)
        {
            var gateway = brainTreetHelper.GetGateway();

            var request = new TransactionRequest
            {
                PaymentMethodNonce = nonce,
                Amount = amount
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);
            ViewBag.IsSuccess = result.IsSuccess();
            var transaction = result.Transaction ?? result.Target;
            if (transaction != null)
            {
                var payment = new Payment
                {
                    BookingId = bookingId,
                    CreatedOn = DateTime.Now,
                    IsRefund = false,
                    RefundByUserId = null,
                    StatusId = (int)(result.IsSuccess() ? PaymentStatus.Success : PaymentStatus.Failed),
                    Total = amount,
                    UserId = userId,
                    TransactionCode = transaction.ProcessorResponseCode,
                    TransactionResponseText = transaction.ProcessorResponseText,
                    TransactionId = transaction.Id,
                    Message = result.Message,
                    OrderId = transaction.OrderId,
                    CreatedAt = transaction.CreatedAt,
                    IsSuccessTransaction = result.IsSuccess(),
                    Status = transaction.Status.ToString()
                };

                if (transaction.PayPalDetails != null)
                {
                    payment.PaypalDebugId = transaction.PayPalDetails.DebugId;
                    payment.PayPalPaymentId = transaction.PayPalDetails.PaymentId;
                    payment.PayPalTransactionFeeAmount = transaction.PayPalDetails.TransactionFeeAmount;
                    payment.PayPalPayeeEmail = transaction.PayPalDetails.PayeeEmail;
                    payment.PayPalPayerEmail = transaction.PayPalDetails.PayerEmail;
                }

                if (transaction.CreditCard != null)
                {
                    payment.CardBin = transaction.CreditCard.Bin;
                    payment.CardCardholderName = transaction.CreditCard.CardholderName;
                    payment.CardType = transaction.CreditCard.CardType.ToString();
                    payment.CardExpirationDate = transaction.CreditCard.ExpirationDate;
                    payment.CardLastFour = transaction.CreditCard.LastFour;
                }

                PaymentManager.CreatePayment(payment);
            }
            else
            {
                var payment = new Payment
                {
                    BookingId = bookingId,
                    CreatedOn = DateTime.Now,
                    IsRefund = false,
                    RefundByUserId = null,
                    StatusId = (int)PaymentStatus.Failed,
                    Total = amount,
                    UserId = userId,
                    Message = result.Message
                };
                LoggerHelper.LogException(
                    string.Format(
                        "Payment exception. No transaction data returned from paymentProcessor. Message: {0}",
                        result.Message));
                PaymentManager.CreatePayment(payment);
            }

            return result;
        }
       
    }

    public class HoursListJson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsBooked { get; set; }
    }
}