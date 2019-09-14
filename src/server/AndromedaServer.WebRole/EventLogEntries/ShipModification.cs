using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a ship modification.
    /// </summary>
    public class ShipModification : EventLogEntryBase
    {
        public int ComponentId { get; set; } //0 - Drive, 1 - Sensor, 2 - Cannon, 3 - Shield
        public bool IsIncrease { get; set; }
        public int NewQuantity { get; set; }
    }
}