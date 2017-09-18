using System.Web.Mvc;

namespace PureSmileUI.Controllers
{
    [RequireHttps]
    public class HomeController : BaseController
    {
        public ActionResult Index(bool? isModal)
        {
            if (isModal.HasValue && isModal.Value)
            {
                ViewBag.isModal = true;
            }
            else
            {
                if (User.Identity.IsAuthenticated)
                {
                    if (User.IsInRole("SuperUser") || User.IsInRole("Staff"))
                    {
                        return RedirectToAction("AdminBookingList", "Booking");
                    }
                    else
                    {
                        return RedirectToAction("UserBookingList", "Booking");
                    }
                }
            }
            return RedirectToAction("Login", "Account", new { isModal = isModal });
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}