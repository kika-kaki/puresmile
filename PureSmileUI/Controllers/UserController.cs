using DatabaseContext.Managers;
using DatabaseContext.Models;
using Microsoft.AspNet.Identity;
using PureSmileUI.Models.Dto;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        UserManager Manager = new UserManager();
        ApplicationRoleManager RoleManager = new ApplicationRoleManager();

        public ActionResult UserList()
        {
            return View("AdminUserListView");
        }

        public ActionResult Edit(int id)
        {
            UserEditItem userItem = new UserEditItem();
            if (id != 0)
            {
                var user = Manager.GetById(id);
                if (user != null)
                {
                    userItem.Id = user.Id;
                    userItem.UserName = user.UserName;
                    userItem.Email = user.Email;
                    userItem.EmailConfirmed = user.EmailConfirmed;
                    userItem.PhoneNumber = user.PhoneNumber;
                    userItem.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                    userItem.LockoutEnabled = user.LockoutEnabled;
                    userItem.LockoutEndDateUtc = user.LockoutEndDateUtc;
                    userItem.City = user.City;
                    userItem.AvatarName = user.AvatarName;
                    userItem.ClientDataId = user.ClientDataId;
                    userItem.ClientData = user.ClientDataId.HasValue
                        ? new ClientDataItem
                        {
                            Id = user.ClientDataId.Value,
                            FirstName = user.ClientData.FirstName,
                            LastName = user.ClientData.LastName
                        }
                        : new ClientDataItem();

                    if (user.Roles.Count() > 0)
                    {
                        var role = RoleManager.GetRoleById(user.Roles.FirstOrDefault().RoleId);
                        userItem.Type = role != null ? role.Name : userItem.Type = "Client";
                    }
                    else
                    {
                        userItem.Type = "Client";
                    }
                }
            }
            else
            {
                userItem.LockoutEndDateUtc = DateTime.Today;
            }

            return View("AdminUserEditView", userItem);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperUser")]
        public ActionResult Save(UserEditItem user)
        {
            if (ModelState.IsValid)
            {
                var newUser = new User
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDateUtc = user.LockoutEndDateUtc,
                    City = user.City,
                    AvatarName = user.AvatarName,
                    ClientDataId = user.ClientDataId,
                    ClientData = user.ClientData != null
                        ? new ClientData
                        {
                            Id = 0,
                            FirstName = user.ClientData.FirstName,
                            LastName = user.ClientData.LastName
                        }
                        : null
                };

                Manager.CreateOrUpdate(newUser, Request["Type"]);
                return RedirectToAction("UserList", "User");
            }

            return View("AdminUserEditView", user);
        }

        [Authorize(Roles = "SuperUser")]
        public ActionResult Delete(int id)
        {
            bool isDeleted = Manager.Delete(id);

            return RedirectToAction("UserList");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("UserList");
        }

        [HttpGet]
        public JsonResult GetUserList(int page, int rows)
        {
            var users = Manager.GetAll(page, rows);
            var userCount = Manager.GetAllCount();

            var userList = users.Select(user => new UserItem
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDate = user.LockoutEndDateUtc.HasValue
                        ? user.LockoutEndDateUtc.Value.ToShortDateString()
                        : string.Empty,
                    City = user.City,
                    AvatarName = user.AvatarName,
                    FirstName = user.ClientDataId.HasValue ? user.ClientData.FirstName : string.Empty,
                    LastName = user.ClientDataId.HasValue ? user.ClientData.LastName : string.Empty,
                    Type = user.Roles.Count() > 0 ? RoleManager.GetRoleByUserId(user.Id) : "None"
                })
                .OrderByDescending(c => c.Id)
                .ToList();

            var jsonData = new
            {
                total = Math.Ceiling(userCount / (double)rows),
                page = page,
                records = userCount,
                rows = userList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ActiveUserList()
        {
            var userList = Manager.GetAllActive()
                .Select(user => new UserItem
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDate = user.LockoutEndDateUtc.HasValue
                        ? user.LockoutEndDateUtc.Value.ToShortDateString()
                        : string.Empty,
                    City = user.City,
                    AvatarName = user.AvatarName,
                    FirstName = user.ClientDataId.HasValue ? user.ClientData.FirstName : string.Empty,
                    LastName = user.ClientDataId.HasValue ? user.ClientData.FirstName : string.Empty
                })
                .OrderBy(c => c.UserName)
                .ToList();

            return Json(userList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetUser(int id)
        {
            var user = Manager.GetById(id);
            var userItem = new UserEditItem
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEndDateUtc = user.LockoutEndDateUtc,
                City = user.City,
                AvatarName = user.AvatarName,
                ClientDataId = user.ClientDataId,
                ClientData = user.ClientDataId.HasValue
                    ? new ClientDataItem
                    {
                        Id = user.ClientDataId.Value,
                        FirstName = user.ClientData.FirstName,
                        LastName = user.ClientData.LastName
                    }
                    : new ClientDataItem(),
            };

            if (user.Roles.Count == 0)
            {
                userItem.Type = "Client";
            }
            else
            {
                var role = RoleManager.GetRoleById(user.Roles.FirstOrDefault().RoleId);
                userItem.Type = role != null ? role.Name : userItem.Type = "Client";
            }

            return Json(userItem, JsonRequestBehavior.AllowGet);
        }
    }
}