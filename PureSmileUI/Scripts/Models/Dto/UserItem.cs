using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PureSmileUI.Models.Dto
{
    public class UserItem
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("User name")]
        public string UserName { get; set; }

        public string City { get; set; }

        [DisplayName("Avatar name")]
        public string AvatarName { get; set; }

        public string Email { get; set; }

        [DisplayName("Email is confirmed")]
        public bool EmailConfirmed { get; set; }

        [DisplayName("Lockout is enabled")]
        public bool LockoutEnabled { get; set; }

        [DisplayName("Date of lockout enabled")]
        public string LockoutEndDate { get; set; }

        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }

        [DisplayName("Phone number is confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Type")]
        public string Type { get; set; }
    }
}