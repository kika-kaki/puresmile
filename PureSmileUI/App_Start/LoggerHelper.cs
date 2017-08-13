using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.Services;
using log4net;

namespace PureSmileUI.App_Start
{
    public class LoggerHelper
    {
        private enum LogType
        {
            /// <summary>
            /// Лог для ручного логинга ошибок
            /// </summary>
            Error = 1,

            /// <summary>
            /// Просто лог
            /// </summary>
            Log = 2
        }

        private static readonly ILog _jsLogger = LogManager.GetLogger("JSLogger");
        private static readonly ILog _appLogger = LogManager.GetLogger("AppLogger");

        public static void ProcessJSLog(HttpRequestBase request)
        {
            LogType logType = LogType.Error;

            var logMessageEncoded = request["Message"] ?? string.Empty;
            var logMessage = HttpUtility.HtmlDecode(logMessageEncoded);

            switch (logType)
            {
                case LogType.Error:
                    var logFile = request["File"] ?? string.Empty;
                    var logLine = request["Line"] ?? string.Empty;

                    string messageForLoggng = string.Format("{0}JS Error{0}{1}{0}at {2} on line {3}", Environment.NewLine, logMessage, logFile, logLine);
                    Log(messageForLoggng);
                    break;

                case LogType.Log:
                    Log(logMessage);
                    break;
            }
        }

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

        private static void Log(string message)
        {
            _jsLogger.ErrorFormat("{0}{1}{0}", Environment.NewLine, message);
        }
    }
}