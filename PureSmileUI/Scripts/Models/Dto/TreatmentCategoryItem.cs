using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class TreatmentCategoryItem
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public string PictureUrl { get; set; }

        public bool IsActive { get; set; }
    }
}