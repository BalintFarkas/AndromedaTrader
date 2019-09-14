using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Andromeda.Common
{
    public static class DistanceCalculator
    {
        public static double GetDistance(double x1, double y1, double x2, double y2)
        {
            double a = Math.Abs(x1 - x2);
            double b = Math.Abs(y1 - y2);
            double c = Math.Sqrt(a * a + b * b);
            return c;
        }
    }
}