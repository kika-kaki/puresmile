using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PureSmileUI.Scripts.Models.Dto
{
    public class ClinicTreatmentCategoryEditItem
    {
        public ClinicTreatmentCategoryEditItem() { }

        public ClinicTreatmentCategoryEditItem(int id, string name, bool isActive)
        {
            Id = id;
            Name = name;
            IsActive = isActive;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}