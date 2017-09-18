using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DatabaseContext.Managers;
using Microsoft.AspNet.Identity;
using PureSmileUI.Models.Dto;
using System;
using System.Collections.Generic;
using DatabaseContext.Models.Enums;

namespace PureSmileUI.Controllers
{
    public class PaymentsController : BaseController
    {
        PaymentManager _manager = new PaymentManager();
        BookingManager bookingManager = new BookingManager();
        ClinicManager clinicManager = new ClinicManager();
        TreatmentManager treatmentManager = new TreatmentManager();
        TreatmentCategoryManager treatmentCategoryManager = new TreatmentCategoryManager();

        // GET: Payments
        public ActionResult View(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        // GET: All payments
        public ActionResult AllPaymentsView()
        {
            var treatmentList = treatmentManager.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .OrderBy(c => c.Text)
                .ToList();

            ViewBag.TreatmentList = treatmentList;

            var treatmentCategoryList = treatmentCategoryManager.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                })
                .OrderBy(c => c.Text)
                .ToList();

            ViewBag.TreatmentCategoryList = treatmentCategoryList;

            var clinicList = clinicManager.GetAll()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.BusinessName
                })
                .OrderBy(c => c.Text)
                .ToList();

            ViewBag.ClinicList = clinicList;

            return View("AllPayments");
        }

        [HttpGet]
        [Authorize(Roles = "SuperUser,Staff")]
        public JsonResult GetAdminPaymentsList(int page, int rows, int? id, int? treatmentId, int? treatmentCategoryId, int? clinicId, DateTime? dateFrom, DateTime? dateTo)
        {
            if (id.HasValue)
            {
                if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
                {
                    var payments = _manager.GetByBookingId(id.Value).Select(Mapper.Map<PaymentItemView>);
                    return Json(payments, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
                {
                    var paymentList = _manager.GetAll(page, rows);
                    var paymentCount = _manager.GetPaymentCount();

                    if (treatmentId.HasValue)
                    {
                        paymentList = paymentList.Where(p => p.Booking.TreatmentId == treatmentId).ToList();
                    }
                    if (treatmentCategoryId.HasValue)
                    {
                        paymentList = paymentList.Where(p => p.Booking.Treatment.TreatmentCategoryId == treatmentCategoryId).ToList();
                    }
                    if (clinicId.HasValue)
                    {
                        paymentList = paymentList.Where(p => p.Booking.ClinicId == clinicId).ToList();
                    }
                    if (dateFrom.HasValue)
                    {
                        paymentList = paymentList.Where(p => p.Booking.BookDateTime >= dateFrom).ToList();
                    }
                    if (dateTo.HasValue)
                    {
                        paymentList = paymentList.Where(p => p.Booking.BookDateTime <= dateTo).ToList();
                    }

                    var payments = paymentList.Select(Mapper.Map<PaymentItemView>);

                    var jsonData = new
                    {
                        total = Math.Ceiling(paymentCount / (double)rows),
                        page = page,
                        records = paymentCount,
                        rows = payments
                    };

                    return Json(jsonData, JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }

        [HttpGet]
        [Authorize(Roles = "SuperUser,Staff,Client")]
        public JsonResult GetPaymentsList(int? id)
        {
            if (id.HasValue)
            {
                if (User.IsInRole("Client"))
                {
                    if (!bookingManager.IsBookingBelongToUser(User.Identity.GetUserId<int>(), id.Value))
                    {
                        return null;
                    }
                    var payments = _manager.GetByBookingId(id.Value).Select(Mapper.Map<PaymentItemView>);
                    return Json(payments.Select(p => new
                    {
                        p.Id,
                        StatusId = ((PaymentStatus)p.StatusId).ToString(),
                        p.CreatedOn,
                        p.IsRefund,
                        p.BookingId,
                        p.UserId,
                        p.RefundByUserId
                    }), JsonRequestBehavior.AllowGet);
                }
            }
            return null;
        }
    }
}