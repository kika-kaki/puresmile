using System.Diagnostics;
using MailService.Framework;
using System.ServiceProcess;
using System.Threading;


namespace MailService
{
    /// <summary>
    /// The actual implementation of the windows service goes here...
    /// </summary>
    [WindowsService("MailService",
        DisplayName = "MailService",
        Description = "The description of the MailService service.",
        EventLogSource = "MailService",
        StartMode = ServiceStartMode.Automatic)]
    public class ServiceImplementation : IWindowsService
    {
        private Thread workerThread;
        private Thread completeCheckerWorkerThread;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            LoggerHelper.LogException("Mail service disposed.");
            MailSender.RequestStop();
        }

        /// <summary>
        /// This method is called when the service gets a request to start.
        /// </summary>
        /// <param name="args">Any command line arguments</param>
        public void OnStart(string[] args)
        {
            LoggerHelper.LogException("Mail service started.");
            workerThread = new Thread(MailSender.StartMailSenderSheduler);
            workerThread.Start();
            
            completeCheckerWorkerThread = new Thread(CompleteChecker.StartMailSenderSheduler);
            completeCheckerWorkerThread.Start();
        }

        /// <summary>
        /// This method is called when the service gets a request to stop.
        /// </summary>
        public void OnStop()
        {
            LoggerHelper.LogException("Mail service stopped.");
            MailSender.RequestStop();
            CompleteChecker.RequestStop();
        }

        /// <summary>
        /// This method is called when a service gets a request to pause,
        /// but not stop completely.
        /// </summary>
        public void OnPause()
        {
            LoggerHelper.LogException("Mail service paused.");
            MailSender.RequestStop();
            CompleteChecker.RequestStop();
        }

        /// <summary>
        /// This method is called when a service gets a request to resume 
        /// after a pause is issued.
        /// </summary>
        public void OnContinue()
        {
            LoggerHelper.LogException("Mail service continue.");
            MailSender.RequestContinue();
            CompleteChecker.RequestContinue();
        }

        /// <summary>
        /// This method is called when the machine the service is running on
        /// is being shutdown.
        /// </summary>
        public void OnShutdown()
        {
            LoggerHelper.LogException("Mail service shutdown.");
            MailSender.RequestStop();
            CompleteChecker.RequestStop();
        }

        /// <summary>
        /// This method is called when a custom command is issued to the service.
        /// </summary>
        /// <param name="command">The command identifier to execute.</param >
        public void OnCustomCommand(int command)
        {
        }
    }
}