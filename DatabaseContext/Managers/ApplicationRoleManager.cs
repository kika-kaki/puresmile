using DatabaseContext.Models;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseContext.Managers
{
    public class ApplicationRoleManager
    {
        ApplicationDbContext _context = new ApplicationDbContext();

        public List<Role> GetAdminRoleList()
        {
            var roles = _context.Roles.Where(r => r.Name == "SuperUser" || r.Name == "Staff").ToList();
            return roles;
        }

        public Role GetRoleById(int id)
        {
            var role = _context.Roles.FirstOrDefault(r => r.Id == id);
            return role;
        }

        public bool IsUserInRole(int userId, string role)
        {
            var r = _context.Roles.FirstOrDefault(_r => _r.Name == role);
            if (r == null)
            {
                return false;
            }
            return _context.Users.Any(u => u.Id == userId && u.Roles.Any(_r => _r.RoleId == r.Id));
        }

        public void RemoveAllRoles(int id)
        {
            var query = _context.UserRoles.AsQueryable();
            query = query.Where(r=>r.UserId == id);
            _context.UserRoles.RemoveRange(query);
            _context.SaveChanges();
        }

        public string GetRoleByUserId(int userId)
        {
            var _r = _context.UserRoles.FirstOrDefault(ur => ur.UserId == userId);
            if (_r == null)
            {
                return "None";
            }
            var role = _context.Roles.FirstOrDefault(r => r.Id == _r.RoleId);
            if (role != null)
            {
                return role.Name;
            }
            return "None";
        }
    }
}
