using System.ComponentModel;

namespace DatabaseContext.Models.Enums
{
    /// <summary>
    /// Enumerable of booking statuses.
    /// </summary>
    public enum BookingStatusEnum
    {
        /// <summary>
        /// Just created
        /// </summary>
        [Description("Created")]
        Created = 0,

        /// <summary>
        /// Сonfirmed by doctor
        /// </summary>
        [Description("Confirmed")]
        Confirmed = 1,

        /// <summary>
        /// The service is provided
        /// </summary>
        [Description("Completed")]
        Completed = 2,

        /// <summary>
        /// Services declined by client
        /// </summary>
        [Description("Declided by client")]
        DeclinedByClient = 3,

        /// <summary>
        /// Services declined by doctor
        /// </summary>
        [Description("Declined by doctor")]
        DeclinedByDoctor = 4,

        /// <summary>
        /// Paid to Clinic
        /// </summary>
        [Description("Paid to clinic")]
        PaidToCLinic = 5
    }
}