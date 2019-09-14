using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a ship upgrade.
    /// </summary>
    public class ShipUpgrade : EventLogEntryBase
    {
        public int OldModel { get; set; }
        public int NewModel { get; set; }
    }
}