using DatabaseContext.Models;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseContext.Managers
{
    public class TreatmentCategoryManager
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        public List<TreatmentCategory> GetAll()
        {
            return _context.TreatmentCategories.ToList();
        }

        public TreatmentCategory GetById(int id)
        {
            return _context.TreatmentCategories.FirstOrDefault(c => c.Id == id);
        }

        public List<TreatmentCategory> GetByIds(IEnumerable<int> ids)
        {
            return _context.TreatmentCategories.Where(x => ids.Contains(x.Id)).ToList();
        }

        public int CreateOrUpdate(TreatmentCategory treatmentCategory)
        {
            if (treatmentCategory.Id == 0)
            {
                _context.TreatmentCategories.Add(treatmentCategory);
            }
            else
            {
                var oldTreatmentCategory = _context.TreatmentCategories
                    .FirstOrDefault(c => c.Id == treatmentCategory.Id);

                oldTreatmentCategory.Name = treatmentCategory.Name;
                oldTreatmentCategory.Description = treatmentCategory.Description;
                oldTreatmentCategory.PictureUrl = treatmentCategory.PictureUrl;
                oldTreatmentCategory.IsActive = treatmentCategory.IsActive;
            }

            _context.SaveChanges();

            return treatmentCategory.Id;
        }

        public bool Delete(int id)
        {
            var treatmentCategory = _context.TreatmentCategories.Include("Clinics").FirstOrDefault(c => c.Id == id);
            if (treatmentCategory.Clinics != null)
            {
                foreach (var clinic in treatmentCategory.Clinics.ToList())
                {
                    treatmentCategory.Clinics.Remove(clinic);
                }
            }
            if (treatmentCategory != null)
            {
                _context.TreatmentCategories.Remove(treatmentCategory);
            }            
            _context.SaveChanges();

            return true;
        }
    }
}
