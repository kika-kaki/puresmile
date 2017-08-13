using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    /// <summary>
    /// Additional client's data
    /// </summary>
    public class ClientData
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// First name of client
        /// </summary>
        [MaxLength(200)]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of client
        /// </summary>
        [MaxLength(200)]
        public string LastName { get; set; }
    }
}
