using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DatabaseContext.Managers;
using DatabaseContext.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using PureSmileUI;
using PureSmileUI.Enums;

namespace GoogleCalendarApi.Controllers
{
   //todo:uncomment for admin [Authorize] and comment attribute below for booking
    [AllowAnonymous]
    public class CalendarController : Controller
    {
        private GoogleManager gm = new GoogleManager();
        private TreatmentManager TreatmentManager = new TreatmentManager();
        private AppointmentBlockManager blockManager = new AppointmentBlockManager();
       

        private async Task<UserCredential> GetCredentialForApiAsync(int clinicId)
        {
            var initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = GoogleSecrets.ClientId,
                    ClientSecret = GoogleSecrets.ClientSecret,
                },
                Scopes = GoogleRequestedScopes.Scopes,
            };
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            var identity = await HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(
                DefaultAuthenticationTypes.ApplicationCookie);
            var userId = gm.GetProviderKeyByClinicId(clinicId);
            var token = await gm.GetAsync(userId);
           
            return new UserCredential(flow, userId, token);
        }

        // GET: /Calendar/Events
        public async Task<ActionResult> GetEvents(string dateTime, int treatmentId, int clinicId)
        {
           
            DateTimeOffset selectedStart = Convert.ToDateTime(dateTime);
            DateTimeOffset selectedEnd = new DateTimeOffset();
            DateTimeOffset? eventStart = new DateTimeOffset();
            DateTimeOffset? eventEnd = new DateTimeOffset();

            ViewBag.IsBookingUrl = false;
            Treatment treatment = new Treatment();
            treatment = TreatmentManager.GetById(treatmentId);
            if (treatment.DurationId != null)
            {
                selectedEnd = selectedStart.AddMinutes((int)treatment.DurationId);
            }

            //checking for appointment blocks by selected time
            if (!blockManager.CheckForBlocks(selectedStart, clinicId, treatmentId))
            {
                return Json(new { StatusId = EEventStatus.Busy });
            }
            try
            {
                var credential = await GetCredentialForApiAsync(clinicId);

                var initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar MVC",
                };
                var service = new CalendarService(initializer);

                // Fetch the list of calendars.
                var calendars = await service.CalendarList.List().ExecuteAsync();
                var res = calendars.Items.First().TimeZone;
                // Define parameters of request. If you want to use another calendar, change "primary" to the calendar Id. You could find it in the calendars variable. 
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = selectedStart.Date;
                request.TimeMax = selectedStart.Date.AddDays(1);
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                // List events.
                Events events = request.Execute();

                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        eventStart = DateTimeOffset.Parse(eventItem.Start.DateTimeRaw, CultureInfo.InvariantCulture);
                        eventEnd = DateTimeOffset.Parse(eventItem.End.DateTimeRaw, CultureInfo.InvariantCulture);

                        if (selectedStart.DateTime >= eventStart.Value.DateTime && selectedStart.DateTime < eventEnd.Value.DateTime)
                        {
                            return Json(new { StatusId = EEventStatus.Busy});
                        } 
                        if (selectedEnd.DateTime <= eventEnd.Value.DateTime && selectedEnd.DateTime > eventStart.Value.DateTime)
                        {
                            return Json(new { StatusId = EEventStatus.Busy});
                        }
                    }
                }
                else
                {
                    return Json(new { StatusId = EEventStatus.NoData});
                }

                return Json(new { StatusId = EEventStatus.Free});
            }
            catch (Exception ex)
            {

                return Json(new { StatusId = EEventStatus.Error });
            }
        }

        public DateTime DateConverter(DateTime date, string timeZone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var offset = timeZoneInfo.GetUtcOffset(date);
            var dto = new DateTimeOffset(date, offset);

            if (DateTime.Now > dto.LocalDateTime)
                dto = dto.AddDays(1);

            return dto.LocalDateTime;
        }

        public async Task<ActionResult> CreateEvent(int clinicId, int treatmentId, DateTime dateTime, string firstName, string lastName, string email)
        {
           
            EventDateTime startDateTime = new EventDateTime {DateTime = dateTime.ToLocalTime().ToUniversalTime()};

            EventDateTime endDateTime = new EventDateTime();
            
            Treatment treatment = TreatmentManager.GetById(treatmentId);
            if (treatment.DurationId != null)
            {
                endDateTime.DateTime = dateTime.AddMinutes((int)treatment.DurationId).ToLocalTime().ToUniversalTime();

            }
            else
            {
                endDateTime.DateTime = dateTime.AddMinutes(60).ToLocalTime().ToUniversalTime();
            }
            try
            {
                var credential = await GetCredentialForApiAsync(clinicId);

                var initializer = new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar MVC",
                };
                var service = new CalendarService(initializer);

                // Fetch the list of calendars.
                var calendars = await service.CalendarList.List().ExecuteAsync();
                
                // Define parameters of request. If you want to use another calendar, change "primary" to the calendar Id. You could find it in the calendars variable. 
                Event newEvent = new Event
                {
                    Summary = treatment.Name,
                    Description = String.Format("Client info: First Name: {0}, LastName: {1}, Email: {2}", firstName, lastName, email),
                    Start = startDateTime,
                    End = endDateTime,
                    
                };

                var result = await service.Events.Insert(newEvent, "pure.smile.test1@gmail.com").ExecuteAsync();

               return Json(new { StatusId = "Event was created successfully!" }, JsonRequestBehavior.AllowGet);
                

            }
            catch (Exception ex)
            {

                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }

        }

        public void CreateBlock(string date, string time, int clinicId, int treatmentId)
        {
            DateTime selectedTime = Convert.ToDateTime(date + " " + time);
            blockManager.SetBlockAsync(selectedTime, clinicId, treatmentId);
           
        }

    }
}