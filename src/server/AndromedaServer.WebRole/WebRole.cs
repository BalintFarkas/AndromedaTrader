using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace AndromedaServer.WebRole
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            //Set up CPU and memory monitoring
            DiagnosticMonitorConfiguration config = DiagnosticMonitor.GetDefaultInitialConfiguration();
            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                SampleRate = TimeSpan.FromSeconds(5),
                CounterSpecifier = @"\Processor(_Total)\% Processor Time"
            });
            config.PerformanceCounters.DataSources.Add(new PerformanceCounterConfiguration()
            {
                SampleRate = TimeSpan.FromSeconds(5),
                CounterSpecifier = @"\Memory\Available MBytes"
            });
            config.PerformanceCounters.ScheduledTransferPeriod = TimeSpan.FromMinutes(1);
            DiagnosticMonitor.Start("Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString", config);

            return base.OnStart();
        }
    }
}
