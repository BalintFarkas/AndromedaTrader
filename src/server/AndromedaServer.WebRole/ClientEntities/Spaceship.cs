using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.ClientEntities
{
    public class Spaceship
    {
        public long Money { get; set; }
        public int FreeCapacity { get; set; }
        public int TotalCapacity { get; set; }
        public bool IsInTransit { get; set; }
        public List<CommodityInHold> Cargo { get; set; }
        public double SpeedInLightYearsPerMinute { get; set; }
        public double SensorRangeInLightYears { get; set; }
        public int DriveCount { get; set; }
        public int SensorCount { get; set; }
        public int ModificationsRemaining { get; set; }
        public int CannonCount { get; set; }
        public int ShieldCount { get; set; }
        public DateTime LastRaided { get; set; }
        public int TotalShipsOwnedByPlayer { get; set; }
    }
}