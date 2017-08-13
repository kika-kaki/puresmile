using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class ClientComment
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        [Required ForeignKey("Author")]
        public int AuthorId { get; set; }
        public virtual User Author { get; set; }
    }
}
