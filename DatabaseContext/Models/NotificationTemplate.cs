using System.ComponentModel.DataAnnotations;

namespace DatabaseContext.Models
{
    public class NotificationTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Type { get; set; }

        [MaxLength(2000)]
        public string Template { get; set; }
    }
}
