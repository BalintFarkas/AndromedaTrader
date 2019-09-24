using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.WebServices
{
    /// <summary>
    /// This object is a local class used when communicating with the AJAX map in the user's browser.
    /// </summary>
    public class MapObject
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public double SensorRange { get; set; }
        public double CannonRange { get; set; }
    }
}