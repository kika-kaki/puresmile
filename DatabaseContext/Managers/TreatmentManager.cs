using DatabaseContext.Models;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseContext.Managers
{
    public class TreatmentManager
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        public List<Treatment> GetAll()
        {
            return _context.Treatments.Include("TreatmentCategory").ToList();
        }

        public List<Treatment> GetAllActive()
        {
            return _context.Treatments.Include("TreatmentCategory").Where(x => x.IsActive && x.TreatmentCategory.IsActive).ToList();
        }

        public List<Treatment> GetAllByCatogory(int categoryId)
        {
            return _context.Treatments.Where(t => t.TreatmentCategoryId == categoryId).ToList();
        }

        public Treatment GetById(int id)
        {
            return _context.Treatments.FirstOrDefault(t => t.Id == id);
        }

        public int CreateOrUpdate(Treatment treatment)
        {
            if (treatment.Id == 0)
            {
                _context.Treatments.Add(treatment);
            }
            else
            {
                var oldTreatment = _context.Treatments.FirstOrDefault(t => t.Id == treatment.Id);

                oldTreatment.Name = treatment.Name;
                oldTreatment.Description = treatment.Description;
                oldTreatment.TreatmentCategoryId = treatment.TreatmentCategoryId;
                oldTreatment.PictureUrl = treatment.PictureUrl;
                oldTreatment.Price = treatment.Price;
                oldTreatment.IsActive = treatment.IsActive;
                if (treatment.DurationId != null) oldTreatment.DurationId = treatment.DurationId.Value;
            }

            _context.SaveChanges();

            return treatment.Id;
        }

        public bool Delete(int id)
        {
            var treatment = _context.Treatments.FirstOrDefault(c => c.Id == id);
            if (treatment != null)
            {
                _context.Treatments.Remove(treatment);
            }            
            _context.SaveChanges();

            return true;
        }

        public decimal GetPriceById(int treatmentId)
        {
            return _context.Treatments.Where(t => t.Id == treatmentId).Select(t => t.Price).FirstOrDefault();
        }

        public decimal GetPriceByBookingId(int bookingId)
        {
            return _context.Bookings.Where(b => b.Id == bookingId).Select(b => b.Treatment.Price).FirstOrDefault();
        }

        public bool IsAnyInCategory(int id)
        {
            return _context.Treatments.Any(t => t.TreatmentCategoryId == id);
        }

        public bool HasBookings(int id)
        {
            return _context.Bookings.Any(b => b.TreatmentId == id);
        }
    }
}
