using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a ship upgrade.
    /// </summary>
    public class ShipPurchase : EventLogEntryBase
    {
        public int NewCount { get; set; }
    }
}