using System.Collections.Generic;
using System.ComponentModel;
using DatabaseContext.Models;

namespace PureSmileUI.Models.Dto
{
    public class ClinicItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        [DisplayName("Work hours")]
        public string WorkHours { get; set; }

        public List<ClinicHours> OpenClinicHourses { get; set; }

        [DisplayName("Is active")]
        public bool IsActive { get; set; }

        [DisplayName("Location name")]
        public string ShortName { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Email { get; set; }

        [DisplayName("Contact person")]
        public string ContactPerson { get; set; }

        public string Location { get; set; }

        public string Comments { get; set; }

        [DisplayName("Bank account name")]
        public string BankAccountName { get; set; }

        [DisplayName("Bank bsb")]
        public string BankBsb { get; set; }

        [DisplayName("Bank account number")]
        public string BankAccountNumber { get; set; }

        [DisplayName("Latitude")]
        public double? Lat { get; set; }

        [DisplayName("Longitude")]
        public double? Lng { get; set; }

        public List<int> TreatmentCategories { get; set; }

        public string TreatmentCategoriesString
        {
            get
            {
                if (TreatmentCategories == null)
                {
                    return "[]";
                }
                return "[" + string.Join(", ", TreatmentCategories) + "]";
            }
        }
    }
}