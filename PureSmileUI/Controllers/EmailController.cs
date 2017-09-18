using System;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using Microsoft.AspNet.Identity;
using PureSmileUI.Models.Dto;
using System.Linq;
using System.Web.Mvc;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class EmailController : BaseController
    {
        EmailManager Manager = new EmailManager();

        public ActionResult EmailList()
        {
            return View("EmailListView");
        }

        public ActionResult Edit(int id)
        {
            EmailEditItem emailEditItem = new EmailEditItem();
            if (id != 0)
            {
                var emailItem = Manager.GetById(id);
                if (emailItem != null)
                {
                    emailEditItem.Id = emailItem.Id;
                    emailEditItem.RecipientEmail = emailItem.RecipientEmail;
                    emailEditItem.Subject = emailItem.Subject;
                    emailEditItem.Body = emailItem.Body;
                    emailEditItem.CreatedOn = emailItem.CreatedOn;
                    emailEditItem.SentOn = emailItem.SentOn;
                }
            }

            return View("EmailEditView", emailEditItem);
        }

        public ActionResult View(int id)
        {
            EmailItemView emailItemView = new EmailItemView();
            if (id != 0)
            {
                var emailItem = Manager.GetById(id);
                if (emailItem != null)
                {
                    emailItemView.Id = emailItem.Id;
                    emailItemView.RecipientEmail = emailItem.RecipientEmail;
                    emailItemView.Subject = emailItem.Subject;
                    emailItemView.Body = emailItem.Body;
                    emailItemView.CreatedOn = emailItem.CreatedOn;
                    emailItemView.SentOn = emailItem.SentOn;
                }
            }

            return View("EmailView", emailItemView);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(EmailEditItem emailEditItem)
        {
            if (ModelState.IsValid)
            {
                var newEmail = new Email
                {
                    Id = emailEditItem.Id,
                    RecipientEmail = emailEditItem.RecipientEmail,
                    Subject = emailEditItem.Subject,
                    Body = emailEditItem.Body,
                    CreatedOn = emailEditItem.CreatedOn,
                    SentOn = emailEditItem.SentOn
                };

                Manager.CreateOrUpdate(newEmail);
                return RedirectToAction("EmailListView", "Email");
            }

            return View("EmailEditView", emailEditItem);
        }

        [HttpGet]
        public JsonResult GetEmailList(int page, int rows)
        {
            var emails = Manager.GetAll(User.Identity.GetUserId<int>(), page, rows);
            var emailCount = Manager.GetAllCount(User.Identity.GetUserId<int>());

            var emailList = emails.Select(t => new EmailItem
                {
                    Id = t.Id,
                    Subject = t.Subject,
                    RecipientEmail = t.RecipientEmail,
                    RecipientName =
                        t.RecipientId.HasValue && t.Recepient.ClientDataId.HasValue
                            ? t.Recepient.ClientData.LastName + " " + t.Recepient.ClientData.FirstName
                            : " ",
                    Status = t.Status,
                    CreatedOn = TimeZone.CurrentTimeZone.ToLocalTime(t.CreatedOn),
                    SentOn = t.SentOn.HasValue ? (DateTime?)TimeZone.CurrentTimeZone.ToLocalTime(t.SentOn.Value) : null
                })
                .OrderByDescending(c => c.Id)
                .ToList();

            var jsonData = new
            {
                total = Math.Ceiling(emailCount / (double)rows),
                page = page,
                records = emailCount,
                rows = emailList
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetEmail(int id)
        {
            var email = Manager.GetById(id);
            var emailItem = new EmailItemView
            {
                Id = email.Id,
                Subject = email.Subject,
                RecipientEmail = email.RecipientEmail,
                Status = email.Status,
                CreatedOn = email.CreatedOn
            };

            return Json(emailItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            bool isDeleted = Manager.Delete(id);

            return RedirectToAction("EmailList");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("EmailList");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendBookingNotification(string name, string lastName, string email, int clinicId, int treatmentId, string date, string time)
        {
            Manager.CreateBookingNotification(name, lastName, email, clinicId, treatmentId, date, time);
            return Json(new { Status = "Notification Sended" }, JsonRequestBehavior.AllowGet);
        }
    }
}