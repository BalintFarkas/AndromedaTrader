using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.Configuration;

namespace AndromedaServer.WorkerRole
{
    /// <summary>
    /// The commodity stocks in the galaxy need to be increased periodically. Incrementing is done by
    /// the Scheduler.aspx page; it just needs to be called regularly. The simplest and most reliable way
    /// to do it is using a worker role.
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            while (true)
            {
                (new WebClient()).DownloadString(ConfigurationManager.AppSettings["RootUrl"] + "/Scheduler.aspx");
                Thread.Sleep(30 * 60000);
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            return base.OnStart();
        }
    }
}
