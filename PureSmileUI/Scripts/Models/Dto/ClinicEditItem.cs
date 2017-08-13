using PureSmileUI.Scripts.Models.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PureSmileUI.Models.Dto
{
    public class ClinicEditItem
    {
        public int Id { get; set; }

        [Required, MaxLength(200), DisplayName("Location name")]
        public string LocationName { get; set; }

        [DisplayName("Date of listing")]
        public DateTime? DateOfListing { get; set; }

        [Required, MaxLength(200), DisplayName("Business name")]
        public string BusinessName { get; set; }

        [MaxLength(2048)]
        [DisplayName("Business address")]
        public string BusinessAddress { get; set; }

        [MaxLength(16)]
        [DisplayName("ABN")]
        public string ABN { get; set; }



        [Required, DisplayName("Is active")]
        public bool IsActive { get; set; }

        [Required, MaxLength(200), DisplayName("Address")]
        public string Address { get; set; }

        [Required, MaxLength(100), DisplayName("City")]
        public string City { get; set; }

        [Required, MaxLength(50), DisplayName("State")]
        public string State { get; set; }

        [MaxLength(200), DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [MaxLength(200), DisplayName("Work hours")]
        public string WorkHours { get; set; }

        [DataType(DataType.EmailAddress)]
        [MaxLength(200)]
        public string Email { get; set; }

        [MaxLength(200), DisplayName("Contact persons name")]
        public string ContactPerson { get; set; }

        [MaxLength(256)]
        public string Location { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(2000)]
        public string Comments { get; set; }

        [MaxLength(200), DisplayName("Account name")]
        public string BankAccountName { get; set; }

        [MaxLength(200), DisplayName("BSB")]
        public string BankBsb { get; set; }

        [MaxLength(64), DisplayName("Account number")]
        public string BankAccountNumber { get; set; }

        [DisplayName("Payment ratio (PS/clinic)")]
        public int? PaymentRatio { get; set; }

        public int? PaymentRatioDif
        {
            get
            {
                return PaymentRatio.HasValue ? 100 - PaymentRatio : 0;
            }
        }

        [DisplayName("Contract")]
        public byte[] ContractFileContent { get; set; }

        public string ContractFileName { get; set; }

        [DisplayName("Opening hours")]
        public List<ClinicHourEditItem> OpeningHours { get; set; }

        [MaxLength(64)]
        [DisplayName("Owner's name")]
        public string OwnersName { get; set; }

        [MaxLength(128)]
        [DisplayName("Owner's email address")]
        public string OwnersEmailAddress { get; set; }

        [MaxLength(32)]
        [DisplayName("Owner's phone number")]
        public string OwnersPhoneNumber { get; set; }

        [MaxLength(128)]
        [DisplayName("Business website address")]
        public string BusinessWebsiteAddress { get; set; }

        [MaxLength(64)]
        [DisplayName("Contact email address - this is the email where we will send notifications")]
        public string EmailAddressForOnlineBookings { get; set; }

        [MaxLength(32)]
        [DisplayName("Direct phone to the location")]
        public string DirectPhoneToTheLocation { get; set; }

        [MaxLength(128)]
        [DisplayName("Store manager phone number")]
        public string StoreManagerPhoneNumber { get; set; }

        [MaxLength(128)]
        [DisplayName("Website location address")]
        public string WebsiteLocationAddress { get; set; }

        [MaxLength(2048)]
        [DisplayName("How to find")]
        public string HowToFind { get; set; }

        [DisplayName("Is contract signed")]
        public bool IsContractSigned { get; set; }

        [DisplayName("Is trained")]
        public bool IsTrained { get; set; }

        [DisplayName("Is locations details spreadsheets completed")]
        public bool IsLocationsDetailsSpreadsheetsCompleted { get; set; }

        [DisplayName("Is taken client through wholesale")]
        public bool IsTakenClientThroughWholesale { get; set; }

        [DisplayName("Is client wholesale login set up")]
        public bool IsClientWholesaleLoginSetuped { get; set; }

        [DisplayName("Latitude")]
        public double? Lat { get; set; }

        [DisplayName("Longitude")]
        public double? Lng { get; set; }

        public bool HasBookings { get; set; }

        public List<TreatmentCategoryItem> TreatmentCategories { get; set; }

        public List<TreatmentCategoryItem> AllTreatmentCategories { get; set; }

        public List<ClinicTreatmentCategoryEditItem> IndicatedTreatmentCategories { get; set; } = new List<ClinicTreatmentCategoryEditItem>();

        public class TreatmentCategoryDescription
        {
            public bool IsActive { get; set; }

            public string Name { get; set; }
        }

        internal TreatmentCategoryDescription getTreatmentCategoryDescription(string name, bool isActive)
        {
            return new TreatmentCategoryDescription
            {
                IsActive = isActive,
                Name = name
            };
        }
    }
}