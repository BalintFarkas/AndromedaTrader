using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a spaceship launch.
    /// </summary>
    public class Launch : EventLogEntryBase
    {
        public string From { get; set; }
        public int FromId { get; set; }
        public string To { get; set; }
        public int ToId { get; set; }
    }
}