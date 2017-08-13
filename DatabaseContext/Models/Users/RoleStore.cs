using DatabaseContext.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DatabaseContext.Managers
{
    public class RoleStore : RoleStore<Role, int, UserRole>
    {
        public RoleStore(ApplicationDbContext context)
        : base(context)
        {
        }
    }
}
