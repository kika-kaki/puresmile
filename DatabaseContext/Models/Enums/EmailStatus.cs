namespace DatabaseContext.Models.Enums
{
    /// <summary>
    /// Enumerable of booking statuses.
    /// </summary>
    public enum EmailStatus
    {
        /// <summary>
        /// Just created
        /// </summary>
        New = 0,

        /// <summary>
        /// Send
        /// </summary>
        Sent = 1,

        /// <summary>
        /// There is error of sent
        /// </summary>
        Error = 2
    }
}
