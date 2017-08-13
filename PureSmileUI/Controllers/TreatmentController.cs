using System;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using PureSmileUI.Models.Dto;
using System.Linq;
using System.Web.Mvc;
using PureSmileUI.App_Start;
using PureSmileUI.Enums;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class TreatmentController : BaseController
    {
        TreatmentManager Manager = new TreatmentManager();

        public ActionResult TreatmentList()
        {
            return View("AdminTreatmentListView");
        }

        public ActionResult Edit(int id)
        {
            TreatmentEditItem treatmentItem = new TreatmentEditItem();
            if (id != 0)
            {
                var treatment = Manager.GetById(id);
                if (treatment != null)
                {
                    treatmentItem.Id = treatment.Id;
                    treatmentItem.Name = treatment.Name;
                    treatmentItem.Description = treatment.Description;
                    treatmentItem.PictureUrl = treatment.PictureUrl;
                    treatmentItem.Price = treatment.Price;
                    treatmentItem.TreatmentCategoryId = treatment.TreatmentCategoryId;
                    treatmentItem.IsActive = treatment.IsActive;
                    treatmentItem.DurationId = treatment.DurationId.HasValue?(ETreatmentDuration)Enum.ToObject(typeof(ETreatmentDuration), treatment.DurationId.Value):ETreatmentDuration.Sixty ;
                }
            }
            else
            {
                treatmentItem.DurationId = ETreatmentDuration.Sixty;
            }
            
            treatmentItem.TreatmentCategoryList = GetTreatmentCategoryList();
            treatmentItem.HasBookings = Manager.HasBookings(id);

            return View("TreatmentEditView", treatmentItem);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(TreatmentEditItem treatment)
        {
            if (ModelState.IsValid)
            {
                if (!FileHelper.ValidateIfImage(ControllerContext.HttpContext.Request.Files))
                {
                    ModelState.AddModelError(string.Empty, $"Input image should be in ({ConfigurationManager.AllowedImageFormats}) formats only.");
                    treatment.TreatmentCategoryList = GetTreatmentCategoryList();
                    return View("TreatmentEditView", treatment);
                }
                var oldPictureUrl = treatment.PictureUrl;

                var newTreatment = new Treatment
                {
                    Id = treatment.Id,
                    Name = treatment.Name,
                    Description = treatment.Description,
                    Price = treatment.Price,
                    TreatmentCategoryId = treatment.TreatmentCategoryId,
                    IsActive = treatment.IsActive,
                    DurationId = (int)treatment.DurationId
                };

                bool hasError = false;
                try
                {
                    if (ControllerContext.HttpContext.Request.Files.Count > 0 &&
                        ControllerContext.HttpContext.Request.Files[0] != null &&
                        ControllerContext.HttpContext.Request.Files[0].ContentLength > 0)
                    // new file posted. Remove old one.
                    {
                        newTreatment.PictureUrl = FileHelper.SaveFile(ControllerContext);
                        FileHelper.DeleteFile(oldPictureUrl);
                    }
                    else
                    {
                        newTreatment.PictureUrl = treatment.PictureUrl;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    hasError = true;
                }

                Manager.CreateOrUpdate(newTreatment);
                if (hasError)
                {
                    treatment.TreatmentCategoryList = GetTreatmentCategoryList();
                    return View("TreatmentEditView", treatment);
                }
                return RedirectToAction("TreatmentList", "Treatment");
            }
            var categoryManager = new TreatmentCategoryManager();

            treatment.TreatmentCategoryList = categoryManager.GetAll().Select(c => new ValueItem
            {
                Id = c.Id,
                Name = c.Name
            }).ToArray();

            return View("TreatmentEditView", treatment);
        }

        [HttpGet]
        public JsonResult GetTreatmentList()
        {
            var treatmentList = Manager.GetAll()
                .Select(t => new TreatmentItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    PictureUrl = t.PictureUrl,
                    Price = t.Price,
                    TreatmentCategoryName = t.TreatmentCategory.Name,
                    TreatmentCategoryId = t.TreatmentCategory.Id,
                    IsActive = t.IsActive
                })
                .OrderBy(c => c.Name)
                .ToList();

            return Json(treatmentList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTreatment(int id)
        {
            var treatment = Manager.GetById(id);
            var treatmentItem = new TreatmentEditItem
            {
                Id = treatment.Id,
                Name = treatment.Name,
                Description = treatment.Description,
                PictureUrl = treatment.PictureUrl,
                Price = treatment.Price,
                TreatmentCategoryId = treatment.TreatmentCategoryId,
                TreatmentCategoryName = treatment.TreatmentCategory.Name,
                HasBookings = Manager.HasBookings(id),
				IsActive = treatment.IsActive,
                DurationId = treatment.DurationId.HasValue ? (ETreatmentDuration)Enum.ToObject(typeof(ETreatmentDuration), treatment.DurationId.Value) : ETreatmentDuration.Sixty 
        };

            return Json(treatmentItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            bool isDeleted = Manager.Delete(id);

            return RedirectToAction("TreatmentList");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("TreatmentList");
        }

        private ValueItem[] GetTreatmentCategoryList()
        {
            var categoryManager = new TreatmentCategoryManager();
            return categoryManager.GetAll().Select(c => new ValueItem
            {
                Id = c.Id,
                Name = c.Name
            })
            .OrderBy(c => c.Name)
            .ToArray();
        }
    }
}