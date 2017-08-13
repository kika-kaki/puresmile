using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DatabaseContext.Logger
{
    public static class LoggerHelper
    {
        private static bool _initialized = false;
        public static void InitLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private static readonly ILog _logger = LogManager.GetLogger("DBContextLogger");

        public static void LogException(Exception ex)
        {
            Initialize();
            _logger.Error(ex);
        }

        public static void LogException(DbEntityValidationException ex)
        {
            List<string> errorMessages = new List<string>();
            foreach (DbEntityValidationResult validationResult in ex.EntityValidationErrors)
            {
                string entityName = validationResult.Entry.Entity.GetType().Name;
                foreach (DbValidationError error in validationResult.ValidationErrors)
                {
                    errorMessages.Add(entityName + "." + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
            LogException(string.Join(Environment.NewLine, errorMessages.ToArray()), ex);
        }

        public static void LogException(string message, Exception ex)
        {
            Initialize();
            _logger.Error(message, ex);
        }

        public static void LogException(string message)
        {
            Initialize();
            _logger.ErrorFormat("{0}DataContext error: {1}{0}", Environment.NewLine, message);
        }

        private static void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            InitLog4Net();
        }
    }
}
