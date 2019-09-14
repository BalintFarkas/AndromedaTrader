using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.EventLogEntries
{
    /// <summary>
    /// Event log entry for a pirate raid.
    /// </summary>
    public class PirateRaid : EventLogEntryBase
    {
        public Guid TargetTransponder { get; set; }
        public string TargetPlayer { get; set; }
        public bool IsSuccessful { get; set; }
        public int AmountStolen { get; set; }
        public int AttackerCannons { get; set; }
        public int DefenderShields { get; set; }
        public double Distance { get; set; }
    }
}