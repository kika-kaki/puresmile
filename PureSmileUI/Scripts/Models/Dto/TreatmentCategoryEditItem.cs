using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class TreatmentCategoryEditItem
    {
        [Key]
        public int Id { get; set; }

        [Required MaxLength(200)]
        public string Name { get; set; }
        
        [Required MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(200)]
        [DisplayName("Picture url")]
        public string PictureUrl { get; set; }

        public bool HasTreatments { get; set; }

        public bool IsActive { get; set; }
    }
}