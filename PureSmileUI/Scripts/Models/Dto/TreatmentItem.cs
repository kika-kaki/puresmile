using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class TreatmentItem
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public decimal Price { get; set; }
        
        public string Description { get; set; }

        public string PictureUrl { get; set; }
        
        public string TreatmentCategoryName { get; set; }

        public int TreatmentCategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}