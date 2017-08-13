using Microsoft.AspNet.Identity.EntityFramework;

namespace DatabaseContext.Models
{
    public class UserStore : UserStore<User, Role, int, UserLogin, UserRole, UserClaim>
    {
        public UserStore(ApplicationDbContext context) : base(context)
        {
        }
    }
}
