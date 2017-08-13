using DatabaseContext.Managers;
using DatabaseContext.Models;
using DatabaseContext.Models.Enums;
using PureSmileUI.Models.Dto;
using PureSmileUI.Scripts.Models.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class ClinicController : BaseController
    {
        private ClinicManager _manager = new ClinicManager();
        private TreatmentManager _treatmentManager = new TreatmentManager();
        private TreatmentCategoryManager _treatmentCategoryManager = new TreatmentCategoryManager();
        private BookingManager _bookingManager = new BookingManager();

        public ActionResult ClinicList()
        {
            return View("AdminClinicListView");
        }

        public ActionResult AdminClinicSummary()
        {
            var treatmentList = _treatmentManager.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .OrderBy(c => c.Text)
                .ToList();

            ViewBag.TreatmentList = treatmentList;

            return View("AdminClinicSummaryView");
        }

        public ActionResult AdminClinicDetails()
        {
            var treatmentList = _treatmentManager.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .OrderBy(c => c.Text)
                .ToList();

            ViewBag.TreatmentList = treatmentList;

            return View("AdminClinicDetailsView");
        }

        public ActionResult Edit(int id)
        {
            ClinicEditItem clinicItem = new ClinicEditItem();
            if (id != 0)
            {
                var clinic = _manager.GetById(id);
                if (clinic != null)
                {
                    clinicItem.Id = clinic.Id;
                    clinicItem.BusinessName = clinic.BusinessName;
                    clinicItem.LocationName = clinic.ShortName;
                    clinicItem.Address = clinic.Address;
                    clinicItem.State = clinic.State;
                    clinicItem.City = clinic.City;
                    clinicItem.Phone = clinic.Phone;
                    clinicItem.WorkHours = clinic.WorkHours;
                    clinicItem.IsActive = clinic.IsActive;
                    clinicItem.Email = clinic.Email;
                    clinicItem.ContactPerson = clinic.ContactPerson;
                    clinicItem.Location = clinic.Location;
                    clinicItem.Comments = clinic.Comments;
                    clinicItem.BankAccountName = clinic.BankAccountName;
                    clinicItem.BankBsb = clinic.BankBsb;
                    clinicItem.BankAccountNumber = clinic.BankAccountNumber;
                    clinicItem.PaymentRatio = clinic.PaymentRatio;
                    clinicItem.ContractFileContent = clinic.ContractFileContent;
                    clinicItem.ABN = clinic.ABN;
                    clinicItem.BusinessAddress = clinic.BusinessAddress;
                    clinicItem.OwnersName = clinic.OwnersName;
                    clinicItem.OwnersEmailAddress = clinic.OwnersEmailAddress;
                    clinicItem.OwnersPhoneNumber = clinic.OwnersPhoneNumber;
                    clinicItem.BusinessWebsiteAddress = clinic.BusinessWebsiteAddress;
                    clinicItem.EmailAddressForOnlineBookings = clinic.EmailAddressForOnlineBookings;
                    clinicItem.DirectPhoneToTheLocation = clinic.DirectPhoneToTheLocation;
                    clinicItem.StoreManagerPhoneNumber = clinic.StoreManagerPhoneNumber;
                    clinicItem.WebsiteLocationAddress = clinic.WebsiteLocationAddress;
                    clinicItem.HowToFind = clinic.HowToFind;
                    clinicItem.IsContractSigned = clinic.IsContractSigned;
                    clinicItem.IsTrained = clinic.IsTrained;
                    clinicItem.IsLocationsDetailsSpreadsheetsCompleted = clinic.IsLocationsDetailsSpreadsheetsCompleted;
                    clinicItem.IsTakenClientThroughWholesale = clinic.IsTakenClientThroughWholesale;
                    clinicItem.IsClientWholesaleLoginSetuped = clinic.IsClientWholesaleLoginSetuped;
                    clinicItem.DateOfListing = clinic.DateOfListing;
                    clinicItem.ContractFileName = string.IsNullOrEmpty(clinic.ContractFileName) ? " " : clinic.ContractFileName;
                    clinicItem.Lat = clinic.Lat;
                    clinicItem.Lng = clinic.Lng;
                    clinicItem.HasBookings = _manager.IsClinicHasBookings(id);
                    clinicItem.TreatmentCategories = new List<TreatmentCategoryItem>(clinic.TreatmentCategories.Select(tc => new TreatmentCategoryItem { 
                        Id = tc.Id,
                        Description = tc.Description,
                        IsActive = tc.IsActive,
                        Name = tc.Name,
                        PictureUrl = tc.PictureUrl
                    }));
                    clinicItem.AllTreatmentCategories = new List<TreatmentCategoryItem>(_treatmentCategoryManager.GetAll().Select(tc => new TreatmentCategoryItem
                    {
                        Id = tc.Id,
                        Description = tc.Description,
                        IsActive = tc.IsActive,
                        Name = tc.Name,
                        PictureUrl = tc.PictureUrl
                    })); 

                    clinicItem.IndicatedTreatmentCategories = 
                        clinicItem.AllTreatmentCategories.Where(x => x.IsActive).Select(x => 
                            new ClinicTreatmentCategoryEditItem(
                                x.Id, 
                                x.Name, 
                                clinicItem.TreatmentCategories.Select(y => y.Id).Contains(x.Id))
                            ).ToList();

                    clinicItem.OpeningHours = clinic.Hours.Select(oh => new ClinicHourEditItem
                    {
                        Id = oh.Id,
                        CloseHour = oh.CloseHour.HasValue ? oh.CloseHour.Value.ToLocalTime() : oh.CloseHour,
                        OpenHour = oh.OpenHour.HasValue ? oh.OpenHour.Value.ToLocalTime() : oh.OpenHour,
                        BookingOpenHour = oh.BookingOpenHour.HasValue ? oh.BookingOpenHour.Value.ToLocalTime() : oh.BookingOpenHour,
                        BookingCloseHour = oh.BookingCloseHour.HasValue ? oh.BookingCloseHour.Value.ToLocalTime() : oh.BookingCloseHour,
                        Day = (int)oh.Day
                    }).ToList();
                }
            }
            else
            {
                clinicItem.ContractFileName = " ";

                clinicItem.OpeningHours = new List<ClinicHourEditItem>();

                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Sunday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Monday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Tuesday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Wednesday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Thursday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Friday });
                clinicItem.OpeningHours.Add(new ClinicHourEditItem { Day = (int)DayOfWeek.Saturday });

                clinicItem.AllTreatmentCategories = new List<TreatmentCategoryItem>(_treatmentCategoryManager.GetAll().Select(tc => new TreatmentCategoryItem
                {
                    Id = tc.Id,
                    Description = tc.Description,
                    IsActive = tc.IsActive,
                    Name = tc.Name,
                    PictureUrl = tc.PictureUrl
                }));

                clinicItem.IndicatedTreatmentCategories =
                    clinicItem.AllTreatmentCategories.Where(x => x.IsActive).Select(x =>
                        new ClinicTreatmentCategoryEditItem(
                            x.Id,
                            x.Name,
                            false)
                        ).ToList();

            }

            return View("ClinicEditView", clinicItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(ClinicEditItem clinic)
        {
            if (ModelState.IsValid)
            {
                var newClinic = new Clinic
                {
                    Id = clinic.Id,
                    BusinessName = clinic.BusinessName,
                    ShortName = clinic.LocationName,
                    Address = clinic.Address,
                    State = clinic.State,
                    City = clinic.City,
                    Phone = clinic.Phone,
                    WorkHours = clinic.WorkHours,
                    IsActive = clinic.IsActive,
                    Email = clinic.Email,
                    ContactPerson = clinic.ContactPerson,
                    Location = clinic.Location,
                    Comments = clinic.Comments,
                    BankAccountName = clinic.BankAccountName,
                    BankBsb = clinic.BankBsb,
                    BankAccountNumber = clinic.BankAccountNumber,
                    PaymentRatio = clinic.PaymentRatio,
                    Hours = clinic.OpeningHours.Select(oh => new ClinicHours
                    {
                        Id = oh.Id,
                        ClinicId = clinic.Id,
                        Day = (DayOfWeek)oh.Day,
                        OpenHour = oh.OpenHour.HasValue ? oh.OpenHour.Value.ToUniversalTime() : oh.OpenHour,
                        CloseHour = oh.CloseHour.HasValue ? oh.CloseHour.Value.ToUniversalTime() : oh.CloseHour,
                        BookingOpenHour = oh.BookingOpenHour.HasValue ? oh.BookingOpenHour.Value.ToUniversalTime() : oh.BookingOpenHour,
                        BookingCloseHour = oh.BookingCloseHour.HasValue ? oh.BookingCloseHour.Value.ToUniversalTime() : oh.BookingCloseHour
                    }).ToList(),
                    HowToFind = clinic.HowToFind,
                    EmailAddressForOnlineBookings = clinic.EmailAddressForOnlineBookings,
                    OwnersEmailAddress = clinic.OwnersEmailAddress,
                    BusinessWebsiteAddress = clinic.BusinessWebsiteAddress,
                    ABN = clinic.ABN,
                    OwnersName = clinic.OwnersName,
                    OwnersPhoneNumber = clinic.OwnersPhoneNumber,
                    WebsiteLocationAddress = clinic.WebsiteLocationAddress,
                    IsContractSigned = clinic.IsContractSigned,
                    IsTrained = clinic.IsTrained,
                    IsTakenClientThroughWholesale = clinic.IsTakenClientThroughWholesale,
                    IsClientWholesaleLoginSetuped = clinic.IsClientWholesaleLoginSetuped,
                    IsLocationsDetailsSpreadsheetsCompleted = clinic.IsLocationsDetailsSpreadsheetsCompleted,
                    DateOfListing = clinic.DateOfListing,
                    BusinessAddress = clinic.BusinessAddress,
                    StoreManagerPhoneNumber = clinic.StoreManagerPhoneNumber,
                    DirectPhoneToTheLocation = clinic.DirectPhoneToTheLocation,
                    Lat = clinic.Lat,
                    Lng = clinic.Lng,
                    TreatmentCategories = _treatmentCategoryManager.GetByIds(clinic.IndicatedTreatmentCategories.Where(x => x.IsActive).Select(x => x.Id))
                };

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file != null && file.ContentLength > 0)
                    {
                        try
                        {
                            byte[] contractFile = ConvertToByte(file);
                            newClinic.ContractFileContent = contractFile;
                            newClinic.ContractFileName = file.FileName;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                _manager.CreateOrUpdate(newClinic);
                return RedirectToAction("ClinicList", "Clinic");
            }

            return View("ClinicEditView", clinic);
        }

        private byte[] ConvertToByte(HttpPostedFileBase file)
        {
            byte[] imageByte = null;
            BinaryReader rdr = new BinaryReader(file.InputStream);
            imageByte = rdr.ReadBytes(file.ContentLength);
            return imageByte;
        }

        [HttpGet]
        public JsonResult GetClinicList()
        {
            var clinicList = _manager.GetAll()
                .Select(clinic => new ClinicItem
                {
                    Id = clinic.Id,
                    Name = clinic.BusinessName,
                    ShortName = clinic.ShortName,
                    Address = clinic.Address,
                    State = clinic.State,
                    City = clinic.City,
                    Phone = clinic.Phone,
                    WorkHours = clinic.WorkHours,
                    IsActive = clinic.IsActive,
                    Email = clinic.Email,
                    ContactPerson = clinic.ContactPerson,
                    Location = clinic.Location,
                    Comments = clinic.Comments,
                    BankAccountName = clinic.BankAccountName,
                    BankBsb = clinic.BankBsb,
                    BankAccountNumber = clinic.BankAccountNumber
                })
                .OrderByDescending(c => c.Id)
                .ToList();

            return Json(clinicList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClinicSummaryList(int page, int rows, string treatmentId, DateTime? dateFrom, DateTime? dateTo)
        {
            var clinics = _manager.GetSummary(treatmentId, dateFrom, dateTo, page, rows);
            var clinicCount = _manager.GetSummaryCount(treatmentId, dateFrom, dateTo);

            if (!string.IsNullOrEmpty(treatmentId))
            {
                int treatId = int.Parse(treatmentId);
                clinics.ForEach(c => c.Bookings = c.Bookings.Where(t => t.TreatmentId == treatId).ToList());
            }
            if (dateFrom.HasValue)
            {
                clinics.ForEach(c => c.Bookings = c.Bookings.Where(t => t.BookDateTime >= dateFrom).ToList());
            }
            if (dateTo.HasValue)
            {
                clinics.ForEach(c => c.Bookings = c.Bookings.Where(t => t.BookDateTime <= dateTo).ToList());
            }

            var clinicList = clinics.Select(s => new ClinicSummary
            {
                ClinicName = s.BusinessName,
                TreatmentCount = s.Bookings.Count(),
                TreatmentIds = s.Bookings.Select(b => b.TreatmentId).ToList(),
                TreatmentDateTimes = s.Bookings.Select(b => b.BookDateTime).ToList(),
                CustomersCount = s.Bookings.Select(b => b.User).Distinct().Count(),
                MoneySpent = s.Bookings.Select(b => b.Treatment.Price).Sum(),
                MoneyEarned = s.PaymentRatio.HasValue 
                    ? (decimal)(s.Bookings.Select(b => b.Treatment.Price).Sum() * s.PaymentRatio)
                    : s.Bookings.Select(b => b.Treatment.Price).Sum()
            }).ToList(); 

            var jsonData = new
            {
                total = Math.Ceiling(clinicCount / (double)rows),
                page = page,
                records = clinicCount,
                rows = clinicList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClinicDetailsList(int page, int rows, string treatmentId, DateTime? dateFrom, DateTime? dateTo)
        {
            var bookings = _bookingManager.GetAll(treatmentId, dateFrom, dateTo, page, rows, true, true);
            var bookingCount = _bookingManager.GetCount(treatmentId, dateFrom, dateTo, true);

            var bookingList = bookings.Select(s => new ClinicDetails
            {
                ClinicName = s.Clinic.BusinessName,
                TreatmentName = s.Treatment.Name,
                TreatmentId = s.TreatmentId,
                TreatmentDateTime = s.BookDateTime.ToLocalTime().ToShortDateString() + " "
                    + s.BookDateTime.ToLocalTime().ToShortTimeString(),
                BookingStatus = ((BookingStatusEnum)s.StatusId).ToString(),
                CustomerName = string.Format("{0} {1}", s.FirstName, s.LastName),
                CustomerFeedback = s.Comments,
                MoneySpent = s.Treatment.Price - s.Payments.Where(b => b.IsRefund).Select(x => x.Total).Sum(),
                MoneyEarned = s.Clinic.PaymentRatio.HasValue
                    ? (s.Treatment.Price - s.Payments.Where(b => b.IsRefund).Select(x => x.Total).Sum())
                        * (decimal)s.Clinic.PaymentRatio
                    : s.Treatment.Price - s.Payments.Where(b => b.IsRefund).Select(x => x.Total).Sum()
            }).ToList();

            var jsonData = new
            {
                total = Math.Ceiling(bookingCount / (double)rows),
                page = page,
                records = bookingCount,
                rows = bookingList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ActiveClinicList()
        {
            var clinicList = _manager.GetAllActive()
                .Select(clinic => new ClinicItem
                {
                    Id = clinic.Id,
                    Name = clinic.BusinessName,
                    ShortName = clinic.ShortName,
                    Address = clinic.Address,
                    State = clinic.State,
                    City = clinic.City,
                    Phone = clinic.Phone,
                    WorkHours = clinic.WorkHours,
                    IsActive = clinic.IsActive,
                    Email = clinic.Email,
                    ContactPerson = clinic.ContactPerson,
                    Location = clinic.Location,
                    Comments = clinic.Comments,
                    BankAccountName = clinic.BankAccountName,
                    BankBsb = clinic.BankBsb,
                    BankAccountNumber = clinic.BankAccountNumber
                })
                .OrderBy(c => c.Name)
                .ToList();

            return Json(clinicList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetClinic(int id)
        {
            var clinic = _manager.GetById(id);
            var clinicItem = new ClinicEditItem
            {
                Id = clinic.Id,
                BusinessName = clinic.BusinessName,
                LocationName = clinic.ShortName,
                Address = clinic.Address,
                State = clinic.State,
                City = clinic.City,
                Phone = clinic.Phone,
                WorkHours = clinic.WorkHours,
                IsActive = clinic.IsActive,
                Email = clinic.Email,
                ContactPerson = clinic.ContactPerson,
                Location = clinic.Location,
                Comments = clinic.Comments,
                BankAccountName = clinic.BankAccountName,
                BankBsb = clinic.BankBsb,
                BankAccountNumber = clinic.BankAccountNumber,
                PaymentRatio = clinic.PaymentRatio,
                ContractFileContent = clinic.ContractFileContent,
                ContractFileName = string.IsNullOrEmpty(clinic.ContractFileName) ? " " : clinic.ContractFileName
            };

            return Json(clinicItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            bool isDeleted = _manager.Delete(id);

            return RedirectToAction("ClinicList");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("ClinicList");
        }

        public FileContentResult DownloadContract(int clinicId)
        {
            var clinic = _manager.GetById(clinicId);
            if (clinic.ContractFileContent != null && clinic.ContractFileContent.Length != 0)
            {
                return File(clinic.ContractFileContent, "content/type", clinic.ContractFileName);
            }
            else
            {
                HttpNotFound();
                return null;
            }
        }

        public JsonResult GetClinicOpeningHoursList(int clinicId)
        {
            var clinicHours = _manager.GetClinicHoursList(clinicId)
                .Select(clinic => new ClinicHourEditItem
                {
                    Id = clinic.Id,
                    Day = (int)clinic.Day,
                    OpenHour = clinic.OpenHour.HasValue ? clinic.OpenHour.Value.ToLocalTime() : clinic.OpenHour,
                    CloseHour = clinic.CloseHour.HasValue ? clinic.CloseHour.Value.ToLocalTime() : clinic.CloseHour
                })
                .OrderBy(c => c.Day)
                .ToList();

            return Json(clinicHours, JsonRequestBehavior.AllowGet);
        }

    }
}