using DatabaseContext.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace DatabaseContext.Managers
{
    public class ClinicManager
    {
        private ApplicationDbContext _context = new ApplicationDbContext();

        public List<Clinic> GetAll()
        {
            return _context.Clinics.ToList();
        }

        public List<Clinic> GetAllWithHours()
        {
            return _context.Clinics.Include("Hours").ToList();
        }

        public List<Clinic> GetAllActive()
        {
            return _context.Clinics.Include("TreatmentCategories").Where(c => c.IsActive).ToList();
        }

        public List<Clinic> GetSummary(string treatmentId, DateTime? dateFrom, DateTime? dateTo, int? page, int? rows)
        {
            IQueryable<Clinic> clinics = _context.Clinics
                .Include("Bookings")
                .Include("Bookings.User")
                .Include("Bookings.Treatment")
                .Where(c => c.IsActive)
                .OrderBy(c => c.Id);

            if (!string.IsNullOrEmpty(treatmentId))
            {
                int treatId = int.Parse(treatmentId);
                clinics = clinics.Where(c => c.Bookings.Select(b => b.TreatmentId).ToList().Contains(treatId));
            }
            if (dateFrom.HasValue)
            {
                clinics = clinics.Where(c => c.Bookings.Select(b => b.BookDateTime).ToList().Any(t => t >= dateFrom));
            }
            if (dateTo.HasValue)
            {
                clinics = clinics.Where(c => c.Bookings.Select(b => b.BookDateTime).ToList().Any(t => t <= dateTo));
            }
            if (page.HasValue && rows.HasValue)
            {
                clinics = clinics.Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }

            return clinics.ToList();
        }

        public int GetSummaryCount(string treatmentId, DateTime? dateFrom, DateTime? dateTo)
        {
            IQueryable<Clinic> clinics = _context.Clinics.Where(c => c.IsActive);
            if (!string.IsNullOrEmpty(treatmentId))
            {
                int treatId = int.Parse(treatmentId);
                clinics = clinics.Where(c => c.Bookings.Select(b => b.TreatmentId).ToList().Contains(treatId));
            }
            if (dateFrom.HasValue)
            {
                clinics = clinics.Where(c => c.Bookings.Select(b => b.BookDateTime).ToList().Any(t => t >= dateFrom));
            }
            if (dateTo.HasValue)
            {
                clinics = clinics.Where(c => c.Bookings.Select(b => b.BookDateTime).ToList().Any(t => t <= dateTo));
            }
            return clinics.Count();
        }

        public Clinic GetById(int id)
        {
            var clinic = _context.Clinics.Include("Hours").Include("TreatmentCategories").FirstOrDefault(c => c.Id == id);
            if (clinic.Hours.Count == 0)
            {
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Sunday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Monday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Tuesday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Wednesday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Thursday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Friday });
                clinic.Hours.Add(new ClinicHours { ClinicId = clinic.Id, Day = DayOfWeek.Saturday });
            }
            return clinic;
        }

        public int CreateOrUpdate(Clinic clinic)
        {
            if (clinic.Id == 0)
            {
                if (clinic.TreatmentCategories == null)
                    clinic.TreatmentCategories = new List<TreatmentCategory>();
                var clinicTreatmentCategoriesIds = clinic.TreatmentCategories.Select(x => x.Id);
                clinic.TreatmentCategories = _context.TreatmentCategories.Where(x => clinicTreatmentCategoriesIds.Contains(x.Id)).ToList();
                _context.Clinics.Add(clinic);
            }
            else
            {
                var oldClinic = _context.Clinics.Include("Hours").Include("TreatmentCategories").FirstOrDefault(c => c.Id == clinic.Id);

                oldClinic.BusinessName = clinic.BusinessName;
                oldClinic.ShortName = clinic.ShortName;
                oldClinic.IsActive = clinic.IsActive;
                oldClinic.Address = clinic.Address;
                oldClinic.State = clinic.State;
                oldClinic.City = clinic.City;
                oldClinic.Phone = clinic.Phone;
                oldClinic.WorkHours = clinic.WorkHours;
                oldClinic.Email = clinic.Email;
                oldClinic.ContactPerson = clinic.ContactPerson;
                oldClinic.Location = clinic.Location;
                oldClinic.Comments = clinic.Comments;
                oldClinic.BankAccountName = clinic.BankAccountName;
                oldClinic.BankBsb = clinic.BankBsb;
                oldClinic.BankAccountNumber = clinic.BankAccountNumber;
                oldClinic.PaymentRatio = clinic.PaymentRatio;
                oldClinic.ContractFileContent = clinic.ContractFileContent;
                oldClinic.ContractFileName = clinic.ContractFileName;

                oldClinic.HowToFind = clinic.HowToFind;
                oldClinic.EmailAddressForOnlineBookings = clinic.EmailAddressForOnlineBookings;
                oldClinic.OwnersEmailAddress = clinic.OwnersEmailAddress;
                oldClinic.BusinessWebsiteAddress = clinic.BusinessWebsiteAddress;
                oldClinic.ABN = clinic.ABN;
                oldClinic.OwnersName = clinic.OwnersName;
                oldClinic.OwnersPhoneNumber = clinic.OwnersPhoneNumber;
                oldClinic.WebsiteLocationAddress = clinic.WebsiteLocationAddress;
                oldClinic.IsContractSigned = clinic.IsContractSigned;
                oldClinic.IsTrained = clinic.IsTrained;
                oldClinic.IsTakenClientThroughWholesale = clinic.IsTakenClientThroughWholesale;
                oldClinic.IsClientWholesaleLoginSetuped = clinic.IsClientWholesaleLoginSetuped;
                oldClinic.IsLocationsDetailsSpreadsheetsCompleted = clinic.IsLocationsDetailsSpreadsheetsCompleted;
                oldClinic.DateOfListing = clinic.DateOfListing;
                oldClinic.BusinessAddress = clinic.BusinessAddress;
                oldClinic.StoreManagerPhoneNumber = clinic.StoreManagerPhoneNumber;
                oldClinic.DirectPhoneToTheLocation = clinic.DirectPhoneToTheLocation;
                oldClinic.Lat = clinic.Lat;
                oldClinic.Lng = clinic.Lng;

                CreateOrUpdateTreatmentCategories(oldClinic, clinic);

                clinic.Hours.ForEach(oh => CreateOrUpdateHours(oldClinic.Hours, oh));
            }
            _context.SaveChanges();

            return clinic.Id;
        }

        private void CreateOrUpdateTreatmentCategories(Clinic oldClinic, Clinic newClinic)
        {
            var oldCategories = oldClinic.TreatmentCategories;
            var oldIds = oldCategories.Select(x => x.Id).ToList();
            var newIds = newClinic.TreatmentCategories.Select(y => y.Id).ToList();
            var newCategories = _context.TreatmentCategories.Where(x => newIds.Contains(x.Id));

            var catsToDelete = oldCategories.Where(x => oldIds.Except(newIds).Contains(x.Id)).ToList();
            foreach (var item in catsToDelete)
            {
                oldCategories.Remove(item);
            }
            foreach (var cat in newCategories)
            {
                if (!oldCategories.Select(x => x.Id).Contains(cat.Id))
                {
                    oldCategories.Add(cat);
                }
            }

        }

        private void CreateOrUpdateHours(List<ClinicHours> oldOpeningHours, ClinicHours openingHours)
        {
            var openHour = oldOpeningHours.FirstOrDefault(ooh => ooh.Day == openingHours.Day);
            if (openHour != null)
            {
                openHour.OpenHour = openingHours.OpenHour;
                openHour.CloseHour = openingHours.CloseHour;
                openHour.ClinicId = openingHours.ClinicId;
                openHour.BookingOpenHour = openingHours.BookingOpenHour;
                openHour.BookingCloseHour = openingHours.BookingCloseHour;
            }
            else
            {
                oldOpeningHours.Add(new ClinicHours
                {
                    Day = openingHours.Day,
                    OpenHour = openingHours.OpenHour,
                    CloseHour = openingHours.CloseHour,
                    ClinicId = openingHours.ClinicId,
                    BookingOpenHour = openingHours.BookingOpenHour,
                    BookingCloseHour = openingHours.BookingCloseHour
                });
            }
        }

        public bool Delete(int id)
        {
            var clinic = _context.Clinics.Include("Hours").Include("TreatmentCategories").FirstOrDefault(c => c.Id == id);
            if (clinic != null)
            {
                foreach (var tc in _context.TreatmentCategories)
                {
                    if (tc.Clinics != null)
                    {
                        tc.Clinics.Remove(clinic);
                    }
                }
                _context.ClinicHours.RemoveRange(clinic.Hours);
                _context.Clinics.Remove(clinic);
            }
            _context.SaveChanges();

            return true;
        }

        public List<ClinicHours> GetClinicHoursList(int clinicId)
        {
            return _context.ClinicHours.Include("Clinic")
                                              .Where(oh => oh.ClinicId == clinicId)
                                              .ToList();
        }

        public ClinicHours GetClinicHoursListOnDay(int clinicId, DayOfWeek day)
        {
            return _context.ClinicHours.Include("Clinic")
                                              .Where(oh => oh.ClinicId == clinicId && oh.Day == day)
                                              .FirstOrDefault();
        }

        public bool IsClinicHasBookings(int id)
        {
            return _context.Bookings.Any(b => b.ClinicId == id);
        }
    }
}
