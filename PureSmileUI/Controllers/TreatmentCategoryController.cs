using System;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using PureSmileUI.Models.Dto;
using System.Linq;
using System.Web.Mvc;
using PureSmileUI.App_Start;

namespace PureSmileUI.Controllers
{
    [Authorize]
    public class TreatmentCategoryController : BaseController
    {
        TreatmentCategoryManager Manager = new TreatmentCategoryManager();
        TreatmentManager TreatmentManager = new TreatmentManager();

        public ActionResult TreatmentCategoryList()
        {
            return View("AdminTreatmentCategoryListView");
        }

        public ActionResult Edit(int id)
        {
            TreatmentCategoryEditItem treatmentCategoryItem = new TreatmentCategoryEditItem();
            if (id != 0)
            {
                var treatmentCategory = Manager.GetById(id);
                if (treatmentCategory != null)
                {
                    treatmentCategoryItem.Id = treatmentCategory.Id;
                    treatmentCategoryItem.Name = treatmentCategory.Name;
                    treatmentCategoryItem.Description = treatmentCategory.Description;
                    treatmentCategoryItem.PictureUrl = treatmentCategory.PictureUrl;
                    treatmentCategoryItem.HasTreatments = TreatmentManager.IsAnyInCategory(treatmentCategory.Id);
                    treatmentCategoryItem.IsActive = treatmentCategory.IsActive;
                }
            }

            return View("TreatmentCategoryEditView", treatmentCategoryItem);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Save(TreatmentCategoryEditItem treatmentCategory)
        {
            if (ModelState.IsValid)
            {
                if (!FileHelper.ValidateIfImage(ControllerContext.HttpContext.Request.Files))
                {
                    ModelState.AddModelError(string.Empty, $"Input image should be in ({ConfigurationManager.AllowedImageFormats}) formats only.");
                    return View("TreatmentCategoryEditView", treatmentCategory);
                }

                var oldPictureUrl = treatmentCategory.PictureUrl;

                var newTreatmentCategory = new TreatmentCategory
                {
                    Id = treatmentCategory.Id,
                    Name = treatmentCategory.Name,
                    Description = treatmentCategory.Description,
                    IsActive = treatmentCategory.IsActive
                };

                bool hasError = false;
                try
                {
                    if (ControllerContext.HttpContext.Request.Files.Count > 0 &&
                        ControllerContext.HttpContext.Request.Files[0] != null &&
                        ControllerContext.HttpContext.Request.Files[0].ContentLength > 0)
                        // new file posted. Remove old one.
                    {
                        newTreatmentCategory.PictureUrl = FileHelper.SaveFile(ControllerContext);
                        FileHelper.DeleteFile(oldPictureUrl);
                    }
                    else
                    {
                        newTreatmentCategory.PictureUrl = treatmentCategory.PictureUrl;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"{ex.Message}");
                    hasError = true;
                }

                Manager.CreateOrUpdate(newTreatmentCategory);

                if (hasError)
                {
                    return View("TreatmentCategoryEditView", treatmentCategory);
                }

                return RedirectToAction("TreatmentCategoryList", "TreatmentCategory");
            }

            return View("TreatmentCategoryEditView", treatmentCategory);
        }

        [HttpGet]
        public JsonResult GetTreatmentCategoryList()
        {
            var treatmentCategoryList = Manager.GetAll()
                .Select(t => new TreatmentCategoryItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    PictureUrl = t.PictureUrl,
                    IsActive = t.IsActive
                })
                .OrderBy(c => c.Name)
                .ToList();

            return Json(treatmentCategoryList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTreatmentCategory(int id)
        {
            var treatmentCategory = Manager.GetById(id);
            var treatmentCategoryItem = new TreatmentCategoryEditItem
            {
                Id = treatmentCategory.Id,
                Name = treatmentCategory.Name,
                Description = treatmentCategory.Description,
                PictureUrl = treatmentCategory.PictureUrl,
                IsActive = treatmentCategory.IsActive
            };

            return Json(treatmentCategoryItem, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            bool isDeleted = Manager.Delete(id);

            return RedirectToAction("TreatmentCategoryList");
        }

        public ActionResult Cancel()
        {
            return RedirectToAction("TreatmentCategoryList");
        }
    }
}