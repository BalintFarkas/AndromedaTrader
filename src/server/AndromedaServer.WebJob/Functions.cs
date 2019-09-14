using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.Net;

namespace AndromedaServer.WebJob
{
    public class Functions
    {
        /// <summary>
        /// The commodity stocks in the galaxy need to be increased periodically. Incrementing is done by
        /// the Scheduler.aspx page; it just needs to be called regularly. The simplest and most reliable way
        /// to do it is using a worker role.
        /// </summary>
        [FunctionName("TimerTrigger")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            (new WebClient()).DownloadString(ConfigurationManager.AppSettings["RootUrl"] + "/Scheduler.aspx");

            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }
            log.LogInformation($"Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
