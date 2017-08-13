using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DatabaseContext.Managers;
using MailService.Properties;

namespace MailService
{
    class CompleteChecker
    {
        private static bool stop = false;

        private static BookingManager bookingManager = new BookingManager();

        public static void StartMailSenderSheduler()
        {
            while (!stop)
            {
                try
                {
                    ProcessCompleteChecking();
                    Thread.Sleep(Settings.Default.RunCheckCompleteTaskFrequency);
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogException(ex);
                }
            }
        }

        private static void ProcessCompleteChecking()
        {
            try
            {
                bookingManager.SetBookingsCompleted();
            }
            catch (Exception ex)
            {
                LoggerHelper.LogException(ex);
            }
        }

        public static void RequestStop()
        {
            stop = true;
        }

        public static void RequestContinue()
        {
            stop = false;
        }
    }
}
