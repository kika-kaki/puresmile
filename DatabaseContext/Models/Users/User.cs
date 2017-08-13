using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatabaseContext.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        [MaxLength(200)]
        public string City { get; set; }

        [MaxLength(200)]
        public string AvatarName { get; set; }

        [ForeignKey("ClientData")]
        public int? ClientDataId { get; set; }
        public ClientData ClientData { get; set; }

        [InverseProperty("User")]
        public virtual List<Payment> PaymentUsers { get; set; }

        [InverseProperty("RefundByUser")]
        public virtual List<Payment> PaymentRefundUsers { get; set; }

        [InverseProperty("PaidToClinicByUser")]
        public virtual List<Booking> BookingPaidToClinic { get; set; }

        [InverseProperty("User")]
        public virtual List<ClientComment> ClientCommentUsers { get; set; }

        [InverseProperty("Author")]
        public virtual List<ClientComment> ClientCommentAuthors { get; set; }

        //public virtual GoogleCredentials GoogleCredential { get; set; }

        // public virtual List<GoogleCredentials> GoogleCredentials { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User, int> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(
                this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }
}
