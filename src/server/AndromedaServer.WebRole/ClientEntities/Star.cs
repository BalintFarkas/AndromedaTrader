using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.ClientEntities
{
    public class Star
    {
        public Guid StarGuid { get; set; }
        public string Name { get; set; }
        public double DistanceInLightYears { get; set; }
        public List<CommodityAtStar> Commodities { get; set; }
    }
}