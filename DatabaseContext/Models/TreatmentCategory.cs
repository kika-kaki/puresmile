using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Category of treatment
    /// </summary>
    public class TreatmentCategory
    {
        /// <summary>
        /// Identifier of category
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of category
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Description of category
        /// </summary>
        [MaxLength(200)]
        public string Description { get; set; }

        /// <summary>
        /// Url of category picture
        /// </summary>
        [MaxLength(200)]
        public string PictureUrl { get; set; }

        /// <summary>
        /// List of treatments having this category
        /// </summary>
        [InverseProperty("TreatmentCategory")]
        public virtual List<Treatment> Treatments { get; set; }

        /// <summary>
        /// Sign of activity/inactivity
        /// </summary>
        public bool IsActive { get; set; }

        public ICollection<Clinic> Clinics { get; set; }
    }
}
