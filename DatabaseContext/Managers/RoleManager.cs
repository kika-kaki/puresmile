using Microsoft.AspNet.Identity;

namespace DatabaseContext.Models
{
    public class RoleManager<TRole> : RoleManager<TRole, int> where TRole : class, IRole<int>
    {
        public RoleManager(IRoleStore<TRole, int> store) : base(store)
        {
        }
    }
}
