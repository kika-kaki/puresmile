using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class UserEditItem
    {
        [Key]
        public int Id { get; set; }

        [Required MaxLength(200) DisplayName("Login")]
        public string UserName { get; set; }

        [MaxLength(20) DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [MaxLength(200)]
        public string City { get; set; }

        [MaxLength(200) DisplayName("Avatar name")]
        public string AvatarName { get; set; }

        [Required MaxLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Email is confirmed")]
        public bool EmailConfirmed { get; set; }

        [DisplayName("Lockout is enabled")]
        public bool LockoutEnabled { get; set; }

        [DisplayName("Date of lockout enabled")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LockoutEndDateUtc { get; set; }

        [MaxLength(200) DisplayName("Phone number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [DisplayName("Phone number is confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        public int? ClientDataId { get; set; }

        public ClientDataItem ClientData { get; set; }
        
        public string Type { get; internal set; }
    }
}