using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Clinic's information
    /// </summary>
    public class Clinic
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of clinic
        /// </summary>
        [Required, MaxLength(200)]
        public string BusinessName { get; set; }

        /// <summary>
        /// Address of clinic
        /// </summary>
        [Required, MaxLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// Phone number of clinic
        /// </summary>
        [MaxLength(100)]
        public string Phone { get; set; }

        /// <summary>
        /// Date of listing
        /// </summary>
        public DateTime? DateOfListing { get; set; }

        /// <summary>
        /// Business Address
        /// </summary>
        [MaxLength(2048)]
        public string BusinessAddress { get; set; }

        /// <summary>
        /// ABN
        /// </summary>
        [MaxLength(16)]
        public string ABN { get; set; }

        /// <summary>
        /// Owner's Name
        /// </summary>
        [MaxLength(64)]
        public string OwnersName { get; set; }

        /// <summary>
        /// Owner's email address (Email address for information relevant to owners)
        /// </summary>
        [MaxLength(128)]
        public string OwnersEmailAddress { get; set; }

        /// <summary>
        /// Owner's phone number
        /// </summary>
        [MaxLength(32)]
        public string OwnersPhoneNumber { get; set; }

        /// <summary>
        /// Business website address
        /// </summary>
        [MaxLength(128)]
        public string BusinessWebsiteAddress { get; set; }

        /// <summary>
        /// Email address for online bookings
        /// </summary>
        [MaxLength(64)]
        public string EmailAddressForOnlineBookings { get; set; }

        /// <summary>
        /// Direct phone to the location
        /// </summary>
        [MaxLength(32)]
        public string DirectPhoneToTheLocation { get; set; }

        /// <summary>
        /// Store manager phone number
        /// </summary>
        [MaxLength(128)]
        public string StoreManagerPhoneNumber { get; set; }

        /// <summary>
        /// Website location address (how it will appear on the website)
        /// </summary>
        [MaxLength(128)]
        public string WebsiteLocationAddress { get; set; }

        /// <summary>
        /// Briefly describe how to best find your salon (around 30 words or less) - This information will be used in customer communication email/text messages 
        /// </summary>
        [MaxLength(2048)]
        public string HowToFind { get; set; }

        /// <summary>
        /// Email number of clinic (for send notifications)
        /// </summary>
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Contact person name of clinic
        /// </summary>
        [MaxLength(200)]
        public string ContactPerson { get; set; }

        /// <summary>
        /// Define how to find the location to the client on the email notification the clinic send them
        /// </summary>
        [MaxLength(256)]
        public string Location { get; set; }

        /// <summary>
        /// Comments of clinic
        /// </summary>
        [MaxLength(2000)]
        public string Comments { get; set; }

        /// <summary>
        /// Work hours of clinic
        /// </summary>
        [MaxLength(200)]
        public string WorkHours { get; set; }

        /// <summary>
        /// Indicates clinic's active
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Short name of clinic
        /// </summary>
        [Required, MaxLength(200)]
        public string ShortName { get; set; }

        /// <summary>
        /// City of clinic
        /// </summary>
        [Required, MaxLength(100)]
        public string City { get; set; }

        ///<summary>
        /// State of clinic
        /// </summary>
        [Required, MaxLength(50)]
        public string State { get; set; }

        /// <summary>
        /// Banking account name 
        /// </summary>
        [MaxLength(200)]
        public string BankAccountName { get; set; }

        /// <summary>
        /// Banking Bsb
        /// </summary>
        [MaxLength(200)]
        public string BankBsb { get; set; }

        /// <summary>
        /// Banking account number
        /// </summary>
        [MaxLength(64)]
        public string BankAccountNumber { get; set; }

        /// <summary>
        /// Payment ratio (PS/clinic)
        /// </summary>
        public int? PaymentRatio { get; set; }

        /// <summary>
        /// File of contract
        /// </summary>
        public byte[] ContractFileContent { get; set; }

        /// <summary>
        /// File of contract (Name)
        /// </summary>
        public string ContractFileName { get; set; }

        [InverseProperty("Clinic")]
        /// <summary>
        /// Booking for this clinic
        /// </summary>
        public virtual List<Booking> Bookings { get; set; }

        [InverseProperty("Clinic")]
        /// <summary>
        /// Opening hours of clinic booking
        /// </summary>
        public virtual List<ClinicHours> Hours { get; set; }

        /// <summary>
        /// Has the contract been signed by all client and PureSmile?
        /// </summary>
        public bool IsContractSigned { get; set; }

        /// <summary>
        /// Has training been completed?
        /// </summary>
        public bool IsTrained { get; set; }

        /// <summary>
        /// Has the locations details spreadsheet been completed?
        /// </summary>
        public bool IsLocationsDetailsSpreadsheetsCompleted { get; set; }

        /// <summary>
        /// Take client through wholesale login and wholesale ordering
        /// </summary>
        public bool IsTakenClientThroughWholesale { get; set; }

        /// <summary>
        /// Is the client's wholesale login set up and operational?
        /// </summary>
        public bool IsClientWholesaleLoginSetuped { get; set; }

        /// <summary>
        /// Latitude of clinic location
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// Longitude of clinic location
        /// </summary>
        public double? Lng { get; set; }

        public List<TreatmentCategory> TreatmentCategories { get; set; }
        //public List<AppointmentBlock> AppointmentBlocks { get; set; }
    }
}
