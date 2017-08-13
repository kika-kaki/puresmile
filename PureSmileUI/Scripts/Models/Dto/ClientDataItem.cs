using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class ClientDataItem
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200), DisplayName("Last name")]
        public string LastName { get; set; }

        [MaxLength(200), DisplayName("First name")]
        public string FirstName { get; set; }
    }
}