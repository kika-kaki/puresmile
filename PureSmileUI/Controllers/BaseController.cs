using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using PureSmileUI.App_Start;

namespace PureSmileUI.Controllers
{
    public class BaseController : Controller
    {
        private const string Anonymous = "Anonymous";
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            var url = context.HttpContext.Request.Url.ToString().ToLower();

            if (!CheckAdminUrl(url, context) ||
                !CheckBookingUrl(url, context) ||
                !CheckClientUrl(url, context))
            {
                return;
            }

            base.OnActionExecuting(context);
        }

        private bool CheckAdminUrl(string url, ActionExecutingContext context)
        {
            if (IsAdminUrl(url))
            {
                ViewBag.IsAdminUrl = true;
                if (!User.Identity.IsAuthenticated)
                {
                    var action = context.ActionDescriptor.ActionName;
                    var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var allowedActions = new List<ActionControllerPair>()
                    {
                        new ActionControllerPair() {Controller = "Account", Action = "Login"},
                        new ActionControllerPair() {Controller = "Account", Action = "LogOff"}
                    };

                    if (!IsAllowedAction(action, controller, allowedActions))
                    {
                        LogPermition(controller, action);
                        context.Result = RedirectToAction("Login", "Account");
                        return false;
                    }
                }
                else
                {
                    var action = context.ActionDescriptor.ActionName;

                    if (!User.IsInRole("SuperUser") && !User.IsInRole("Staff"))
                    {

                        var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                        var allowedActions = new List<ActionControllerPair>()
                        {
                            new ActionControllerPair() {Controller = "Account", Action = "LogOff"}
                        };

                        if (!IsAllowedAction(action, controller, allowedActions))
                        {
                            context.Result = RedirectToAction("LogOff", "Account");
                            return false;
                        }
                    }
                    else
                        if (action == "ModalBookingEdit")
                        context.Result = RedirectToAction("AdminBookingList", "Booking");

                }
            }
            else
            {
                ViewBag.IsAdminUrl = false;
            }
            return true;
        }

        private bool IsAdminUrl(string url)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AdminUrl) && url.Contains(ConfigurationManager.AdminUrl);
        }

        private bool CheckBookingUrl(string url, ActionExecutingContext context)
        {
            if (IsBookingUrl(url))
            {
                ViewBag.IsBookingUrl = true;
                //if (User.Identity.IsAuthenticated)
                //{
                //    context.Result = RedirectToAction("LogOff", "Account");
                //    return false;
                //}

                var action = context.ActionDescriptor.ActionName;
                var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;

                var allowedActions = new List<ActionControllerPair>()
                {
                    new ActionControllerPair() {Controller = "Booking", Action = "ModalBookingEdit"},
                    new ActionControllerPair() {Controller = "Booking", Action = "BookingFinish"},
                    new ActionControllerPair() {Controller = "Booking", Action = "GetTimeList"},
                    new ActionControllerPair() {Controller = "Booking", Action = "SaveBookingAsync"},
                    new ActionControllerPair() {Controller = "Booking", Action = "PayAsync"},
                    new ActionControllerPair() {Controller = "Account", Action = "LogOff"},
                    new ActionControllerPair() {Controller = "Account", Action="ExternalLoginConfirmation" },
                    /*Required to permit facebook, google login*/
                    new ActionControllerPair() {Controller = "Account", Action = "ExternalLogin" },
                    new ActionControllerPair() {Controller = "Account", Action = "Login"},
                    new ActionControllerPair() {Controller = "Account", Action = "ExternalLoginCallback"},
                    new ActionControllerPair() {Controller = "Booking", Action = "BookingLoading"},
                    new ActionControllerPair() {Controller = "User", Action = "GetUser"} 
                    /*Required to permit facebook, google login*/

                };

                if (url.ToLower().Contains("ismodal=true") || IsAdminUrl(url)) //If modal window opened, allow to open index action of home controller, for close modal window 
                    allowedActions.Add(new ActionControllerPair() { Controller = "Home", Action = "Index" });

                if (!IsAllowedAction(action, controller, allowedActions))
                {
                    LogPermition(controller, action);
                    context.Result = RedirectToAction("BookingLoading", "Booking", new { id = 0 });
                    return false;
                }
            }
            else
            {
                ViewBag.IsBookingUrl = false;
            }
            return true;
        }

        private bool IsBookingUrl(string url)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.BookingUrl) &&
                url.ToLower().Contains(ConfigurationManager.BookingUrl);
        }

        private void LogPermition(string controller, string action)
        {
            var user = User.Identity.IsAuthenticated ? User.Identity.Name : Anonymous;
            LoggerHelper.LogException($"BaseController. User does not have access to the functionality.  Controller={controller}, Action={action}, User={user}.");
        }

        private bool IsAllowedAction(string action, string controller, List<ActionControllerPair> allowedActions)
        {
            return allowedActions.Any(a => a.Action == action && a.Controller == controller);
        }

        private bool CheckClientUrl(string url, ActionExecutingContext context)
        {
            if (IsClientUrl(url))
            {
                ViewBag.IsClientUrl = true;
                if (!User.Identity.IsAuthenticated)
                {
                    var action = context.ActionDescriptor.ActionName;
                    var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var allowedActions = new List<ActionControllerPair>()
                    {
                        new ActionControllerPair() {Controller = "Account", Action = "Login"},
                        new ActionControllerPair() {Controller = "Account", Action = "LogOff"},
                        new ActionControllerPair() {Controller = "Account", Action = "ExternalLoginCallback"},
                        new ActionControllerPair() {Controller = "Account", Action = "ExternalLoginConfirmation"},
                        new ActionControllerPair() {Controller = "Account", Action = "SendCode"},
                        new ActionControllerPair() {Controller = "Account", Action = "ExternalLogin"},
                        new ActionControllerPair() {Controller = "Account", Action = "ResetPassword"},
                        new ActionControllerPair() {Controller = "Account", Action = "ForgotPasswordConfirmation"},
                        new ActionControllerPair() {Controller = "Account", Action = "ForgotPassword"},
                        new ActionControllerPair() {Controller = "Account", Action = "ConfirmEmail"},
                        new ActionControllerPair() {Controller = "Account", Action = "VerifyCode"},
                        new ActionControllerPair() {Controller = "Account", Action = "Register"}
                    };

                    if (!IsAllowedAction(action, controller, allowedActions))
                    {
                        LogPermition(controller, action);
                        context.Result = RedirectToAction("Login", "Account");
                        return false;
                    }
                }
                else
                {
                    var action = context.ActionDescriptor.ActionName;
                    if (!User.IsInRole("Client"))
                    {

                        var controller = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                        var allowedActions = new List<ActionControllerPair>()
                        {
                            new ActionControllerPair() {Controller = "Account", Action = "LogOff"}
                        };

                        if (!IsAllowedAction(action, controller, allowedActions))
                        {
                            context.Result = RedirectToAction("LogOff", "Account");
                            return false;
                        }
                    }
                    else
                        if (action == "ModalBookingEdit")
                        context.Result = RedirectToAction("UserBookingList", "Booking");
                }
            }
            else
            {
                ViewBag.IsClientUrl = false;
            }
            return true;
        }

        private bool IsClientUrl(string url)
        {
            return !string.IsNullOrEmpty(ConfigurationManager.ClientUrl) && url.Contains(ConfigurationManager.ClientUrl);
        }
    }
}