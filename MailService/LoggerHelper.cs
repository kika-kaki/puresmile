using System;
using log4net;

namespace MailService
{
    public class LoggerHelper
    {
        private static readonly ILog _appLogger = LogManager.GetLogger("AppLogger");
        
        public static void LogException(Exception ex, string extMessage = null)
        {
            if (string.IsNullOrEmpty(extMessage))
            {
                _appLogger.Error(ex);
            }
            else
            {
                _appLogger.Error(extMessage, ex);
            }
        }

        public static void LogException(string message)
        {
            _appLogger.ErrorFormat("{0}Application error: {1}{0}", Environment.NewLine, message);
        }
    }
}