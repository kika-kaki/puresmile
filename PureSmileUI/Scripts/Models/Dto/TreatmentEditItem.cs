using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;
using PureSmileUI.Enums;

namespace PureSmileUI.Models.Dto
{
    public class TreatmentEditItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price should be greater than 0.")]
        public decimal Price { get; set; }
        [DisplayName("Treatment Duration")]
        public ETreatmentDuration DurationId { get; set; }


        [Required, MaxLength(500)]
        public string Description { get; set; }

        [MaxLength(200)]
        [DisplayName("Picture url")]
        [DataType(DataType.ImageUrl)]
        public string PictureUrl { get; set; }

        [Required]
        public int TreatmentCategoryId { get; set; }

        [DisplayName("Category")]
        public string TreatmentCategoryName { get; set; }

        public ValueItem[] TreatmentCategoryList { get; set; }
        public bool HasBookings { get; set; }

        public bool IsActive { get; set; }
    }
}