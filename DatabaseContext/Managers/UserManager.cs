using System;
using DatabaseContext.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DatabaseContext.Managers
{
    public class UserManager
    {
        ApplicationDbContext _context = new ApplicationDbContext();
        ApplicationRoleManager _roleManager = new ApplicationRoleManager();

        public List<User> GetAll(int? page, int? rows)
        {
            var users = _context.Users.Include("ClientData").Include("Roles");
            //var adminRoles = _context.Roles.FirstOrDefault(r => r.Name == "SuperUser");
            //users.RemoveAll(u => u.Roles.Any(r => r.RoleId == adminRoles.Id));

            if (page.HasValue && rows.HasValue)
            {
                users = users.OrderBy(u => u.Id).Skip((page.Value - 1) * rows.Value).Take(rows.Value);
            }

            return users.ToList();
        }

        public int GetAllCount()
        {
            return _context.Users.Count();
        }

        public List<User> GetAllActive()
        {
            var users = _context.Users.Include("ClientData").Include("Roles").ToList();
            //var adminRoles = _context.Roles.FirstOrDefault(r => r.Name == "SuperUser");
            //users.RemoveAll(u => u.Roles.Any(r => r.RoleId == adminRoles.Id));
            var now = DateTime.Now.ToUniversalTime();
            return users.Where(u => !u.LockoutEnabled || (u.LockoutEnabled && u.LockoutEndDateUtc > now)).ToList();
        }

        public List<User> GetAdmins()
        {
            var adminRoleIds = _roleManager.GetAdminRoleList().Select(r => r.Id).ToList();
            var admins = _context.Users.Where(u => adminRoleIds.Contains(u.Roles.Select(x => x.RoleId).FirstOrDefault())).ToList();

            return admins;
        }

        public User GetById(int id)
        {
            return _context.Users.Include("ClientData").FirstOrDefault(c => c.Id == id);
        }

        public int CreateOrUpdate(User user, string role)
        {
            var UserManager = new UserManager<User, int>(new UserStore(_context));
            if (user.Id == 0)
            {
                var passwordHash = new PasswordHasher();
                user.PasswordHash = passwordHash.HashPassword("123123");
                UserManager.Create(user);
                if (!string.IsNullOrEmpty(role))
                {
                    UserManager.AddToRole(user.Id, role);
                }
            }
            else
            {
                if (user.ClientData != null)
                {
                    if (user.ClientData.Id != 0)
                    {
                        var oldClientData = _context.ClientDatas.FirstOrDefault(c => c.Id == user.ClientData.Id);

                        oldClientData.FirstName = user.ClientData.FirstName;
                        oldClientData.LastName = user.ClientData.LastName;                        
                    }
                    else
                    {
                        user.ClientData = _context.ClientDatas.Add(user.ClientData);
                        _context.SaveChanges();
                    }
                }
                
                var oldUser = _context.Users.Include("ClientData").FirstOrDefault(c => c.Id == user.Id);

                oldUser.UserName = user.UserName;
                oldUser.Email = user.Email;
                oldUser.EmailConfirmed = user.EmailConfirmed;
                oldUser.PhoneNumber = user.PhoneNumber;
                oldUser.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                oldUser.City = user.City;
                oldUser.AvatarName = user.AvatarName;
                oldUser.LockoutEnabled = user.LockoutEnabled;
                oldUser.LockoutEndDateUtc = user.LockoutEndDateUtc;
                oldUser.ClientDataId = user.ClientData != null ? user.ClientData.Id : (int?)null;

                if (!_roleManager.IsUserInRole(user.Id, role))
                {
                    _roleManager.RemoveAllRoles(user.Id);
                    UserManager.AddToRole(user.Id, role);
                }
            }

            _context.SaveChanges();
        
            return user.Id;
        }

        public bool Delete(int id)
        {
            var user = _context.Users.FirstOrDefault(c => c.Id == id);

            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return true;
        }

        public List<Role> GetAllRoles()
        {
            return _context.Roles.ToList();
        }

        public List<User> GetAllActiveUsers()
        {
            var users = _context.Users.Include("ClientData").Include("Roles").ToList();
            var adminRoles = _context.Roles.FirstOrDefault(r => r.Name == "SuperUser" || r.Name == "Staff");
            users.RemoveAll(u => u.Roles.Any(r => r.RoleId == adminRoles.Id));
            var now = DateTime.Now.ToUniversalTime();
            return users.Where(u => !u.LockoutEnabled || (u.LockoutEnabled && u.LockoutEndDateUtc > now)).ToList();
        }

        public bool IsLockedOut(string login)
        {
            return _context.Users.Any(u => u.UserName == login && u.LockoutEnabled);
        }

        public bool ConfirmEmail(int userId, Guid guid)
        {
            var emailConfirmation = _context.EmailConfirmation.FirstOrDefault(ec => ec.UserId == userId && ec.Guid == guid);
            var confirmed = emailConfirmation != null;
            if (confirmed)
            {
                _context.EmailConfirmation.Remove(emailConfirmation);
                var user = GetById(userId);
                user.LockoutEnabled = false;
                _context.SaveChanges();
            }
            return confirmed;
        }

        public Guid CreateEmailConfirmation(int id)
        {
            var confirmation = new EmailConfirmation()
            {
                UserId = id,
                Guid = Guid.NewGuid()
            };
            _context.EmailConfirmation.Add(confirmation);
            _context.SaveChanges();
            return confirmation.Guid;
        }

        public User GetByLogin(string userName)
        {
            return _context.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public void SetLockOut(string userName, bool isLockout)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                user.LockoutEnabled = isLockout;
                _context.SaveChanges();
            }
        }
    }
}
