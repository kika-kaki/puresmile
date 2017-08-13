using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseContext.Models
{
    public class GoogleCredentials
    {
        /// <summary>
        /// Identifier
        /// </summary>
        [Key]
        public int GoogleCredsId { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public System.DateTime? IssuedUtc { get; set; }
        public System.DateTime? Issued { get; set; }
        public int? ExpiresInSeconds { get; set; }
    }
}