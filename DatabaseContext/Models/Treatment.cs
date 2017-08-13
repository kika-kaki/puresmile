using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Treatment
    /// </summary>
    public class Treatment
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of treatment
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Price (coast) of treatment
        /// </summary>
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int? DurationId { get; set; }

        /// <summary>
        /// Description of treatment
        /// </summary>
        [Required, MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Url of treatment's picture
        /// </summary>
        [MaxLength(200)]
        public string PictureUrl { get; set; }

        /// <summary>
        /// Identifier of category which constaint this treatment
        /// </summary>
        [Required, ForeignKey("TreatmentCategory")]
        public int TreatmentCategoryId { get; set; }

        /// <summary>
        /// Category which constaint this treatment
        /// </summary>
        public TreatmentCategory TreatmentCategory { get; set; }

        /// <summary>
        /// Sign of activity/inactivity
        /// </summary>
        public bool IsActive { get; set; }
    }
}
