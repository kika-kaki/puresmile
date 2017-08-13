using System;
using System.Collections.Generic;
using DatabaseContext.Managers;
using Microsoft.AspNet.Identity;
using PureSmileUI.Models.Dto;
using System.Linq;
using System.Web.Mvc;
using DatabaseContext.Models;

namespace PureSmileUI.Controllers
{
    public class NotificationController : BaseController
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        EmailManager Manager = new EmailManager();

        public ActionResult NotificationList()
        {
            return View("NotificationListView");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("NotificationList");
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

            return View("NotificationView", emailItemView);
        }

        [HttpGet]
        public JsonResult GetNotificationList(int page, int rows, DateTime? from, DateTime? to, string text)
        {
            var emails = new List<Email>();
            int emailCount = 0;

            if (User.IsInRole("Client"))
            {
                emails = Manager.GetMy(User.Identity.GetUserId<int>(), text, from, to, page, rows);
                emailCount = Manager.GetMyCount(User.Identity.GetUserId<int>(), text, from, to);
            }
            if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
            {
                emails = Manager.GetAll(text, from, to, page, rows);
                emailCount = Manager.GetAllCount(text, from, to);
            }

            var emailList = emails.Select(t => new EmailItem
            {
                Id = t.Id,
                Subject = t.Subject,
                RecipientEmail = t.RecipientEmail,
                Status = t.Status,
                CreatedOn = t.CreatedOn,
                SentOn = t.SentOn
            })
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
    }
}