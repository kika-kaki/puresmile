using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        public int? NotificationType { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public User Author { get; set; }

        [ForeignKey("EmailNotification")]
        public int EmailNotificationId { get; set; }
        public Email EmailNotification { get; set; }

        [ForeignKey("NotificationTemplate")]
        public int NotificationTemplateId { get; set; }
        public NotificationTemplate NotificationTemplate { get; set; }
    }
}
