using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Base class for EventLogEntries.
    /// </summary>
    public abstract class EventLogEntryBase
    {
        public DateTime Timestamp { get; set; }
        public string Player { get; set; }
        public Guid Guid { get; set; }
        public string Error { get; set; }
    }
}