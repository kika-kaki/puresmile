
using Google.Apis.Calendar.v3;

namespace PureSmileUI
{
    internal static class GoogleRequestedScopes
    {
        public static string[] Scopes
        {
            get
            {
                return new[] {
                    "openid",
                    "email",
                    CalendarService.Scope.Calendar,
                };
            }
        }
    }
}