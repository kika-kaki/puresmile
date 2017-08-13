using System.Web.Mvc;
using PureSmileUI.App_Start;

namespace PureSmileUI.Controllers
{
    public class JsLogController : BaseController
    {
        [AcceptVerbs(HttpVerbs.Get|HttpVerbs.Post)]
        public ActionResult Log()
        {
            LoggerHelper.ProcessJSLog(Request);
            return null;
        }
    }
}