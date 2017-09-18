using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace PureSmileUI.App_Start
{
    public static class ConfigurationManager
    {
        public static string MerchantId
        {
            get
            {
                return WebConfigurationManager.AppSettings["MerchantId"];
            }
        }

        public static string PublicKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["PublicKey"];
            }
        }

        public static string PrivateKey
        {
            get
            {
                return WebConfigurationManager.AppSettings["PrivateKey"];
            }
        }

        public static string Enviroment
        {
            get
            {
                return WebConfigurationManager.AppSettings["Enviroment"];
            }
        }

        public static string MaxRadiusForClosestClinicsOnMapInMeters
        {
            get
            {
                return WebConfigurationManager.AppSettings["MaxRadiusForClosestClinicsOnMapInMeters"];
            }
        }

        public static int MinimalMinutesFromNowToBook
        {
            get
            {
                int mins;
                if (int.TryParse(WebConfigurationManager.AppSettings["MinimalMinutesFromNowToBook"], out mins))
                {
                    return mins;
                }
                return 120;
            }
        }

        public static int StepInMinutesToBook
        {
            get
            {
                int mins;
                if (int.TryParse(WebConfigurationManager.AppSettings["StepInMinutesToBook"], out mins))
                {
                    return mins;
                }
                return 30;
            }
        }

        public static int BlockTimeForBookingInMinutes
        {
            get
            {
                int mins;
                if (int.TryParse(WebConfigurationManager.AppSettings["BlockTimeForBookingInMinutes"], out mins))
                {
                    return mins;
                }
                return 90;
            }
        }

        public static string ImageStoragePath
        {
            get { return WebConfigurationManager.AppSettings["ImageStoragePath"]; }
        }

        public static string AllowedImageFormats
        {
            get { return WebConfigurationManager.AppSettings["AllowedImageFormats"]; }
        }

        public static string BookingUrl
        {
            get { return WebConfigurationManager.AppSettings["BookingUrl"].ToLower(); }
        }

        public static string ClientUrl
        {
            get { return WebConfigurationManager.AppSettings["ClientUrl"].ToLower(); }
        }

        public static string AdminUrl
        {
            get { return WebConfigurationManager.AppSettings["AdminUrl"].ToLower(); }
        }
    }
}