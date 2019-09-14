using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Andromeda.Data;
using Andromeda.ServerEntities;

namespace Andromeda.Common
{
    /// <summary>
    /// Caches all stars in memory, since their positions do not change, and it is redundant to require DB accesses to get at their coordinates.
    /// This will speed up many things, such as the Coordinates call of Spaceship, which in turn speeds up GetShips() and targeting.
    /// </summary>
    public static class AndromedaCache
    {
        static AndromedaCache()
        {
            var db = DataContextFactory.GetAndromedaDataContext();
            Stars = db.Stars.ToList();
        }

        public static List<Star> Stars { get; set; }
    }
}