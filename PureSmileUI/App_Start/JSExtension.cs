using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PureSmileUI.App_Start
{
    public static class JSExtension
    {
        public static string ToJsString(this decimal value, CultureInfo culture = null)
        {
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }
            return value.ToString(culture).Replace(',', '.');
        }
    }
}