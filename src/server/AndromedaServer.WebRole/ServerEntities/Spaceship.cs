using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Andromeda.Common;
using Andromeda.Data;
using Andromeda.EventLog;
using Andromeda.EventLogEntries;

namespace Andromeda.ServerEntities
{
    /// <summary>
    /// Summary description for Andromeda
    /// </summary>
    public partial class Spaceship
    {
        private AndromedaDataContext db = DataContextFactory.GetAndromedaDataContext();

        /// <summary>
        /// Checks whether this ship has arrived at its destination and updates status variables, if it has.
        /// Basically this is the "time has passed, check if you are there yet" event.
        /// </summary>
        public void UpdateTravelStatus()
        {
            //If the launch date is null, the ship is currently docked - nothing to do
            if (LaunchDate == null)
            {
                return;
            }
            //Otherwise the ship is on its way
            else
            {
                //Calculate the distance already covered
                var currentStar = db.Stars.Single(i => i.Id == CurrentStarId);
                var targetStar = db.Stars.Single(i => i.Id == TargetStarId);

                double distanceBetweenStars = DistanceCalculator.GetDistance(currentStar.X.Value, currentStar.Y.Value, targetStar.X.Value, targetStar.Y.Value);
                double distanceTraveled = SpeedInLightYearsPerMinute * (TimeGetter.GetLocalTime() - LaunchDate.Value).TotalMinutes;

                //If the ship is still en route, nothing to do
                if (distanceTraveled < distanceBetweenStars)
                {
                    return;
                }
                //The ship has arrived - update status
                else
                {
                    CurrentStarId = TargetStarId;
                    LaunchDate = null;
                    TargetStarId = null;
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// Gets the current coordinates of the ship (taking into account that it might be
        /// currently travelling between stars).
        /// </summary>
        public Tuple<double, double> Coordinates
        {
            get
            {
                //If the launch date is null, the ship is currently docked
                if (LaunchDate == null)
                {
                    var currentStar = AndromedaCache.Stars.Single(i => i.Id == CurrentStarId);
                    return new Tuple<double, double>(currentStar.X.Value, currentStar.Y.Value);
                }
                //Otherwise the ship is on its way
                else
                {
                    //Calculate the distance already covered
                    var currentStar = AndromedaCache.Stars.Single(i => i.Id == CurrentStarId);
                    var targetStar = AndromedaCache.Stars.Single(i => i.Id == TargetStarId);

                    double distanceBetweenStars = DistanceCalculator.GetDistance(currentStar.X.Value, currentStar.Y.Value, targetStar.X.Value, targetStar.Y.Value);
                    double distanceTraveled = SpeedInLightYearsPerMinute * (TimeGetter.GetLocalTime() - LaunchDate.Value).TotalMinutes;

                    //If the ship is still en route, shift our coordinates proportionally
                    if (distanceTraveled < distanceBetweenStars)
                    {
                        double fractionDistanceTraveled = distanceTraveled / distanceBetweenStars;

                        double totalXDelta = targetStar.X.Value - currentStar.X.Value;
                        double totalYDelta = targetStar.Y.Value - currentStar.Y.Value;

                        double coveredXDelta = fractionDistanceTraveled * totalXDelta;
                        double coveredYDelta = fractionDistanceTraveled * totalYDelta;

                        return new Tuple<double, double>(
                            currentStar.X.Value + coveredXDelta,
                            currentStar.Y.Value + coveredYDelta);
                    }
                    //The ship already arrived, but the simulator might not yet have noticed and updated its status - 
                    //it is at the target star
                    else
                    {
                        return new Tuple<double, double>(targetStar.X.Value, targetStar.Y.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the speed of the spaceship in light years per minute.
        /// This depends on the current equipment loadout of the ship.
        /// </summary>
        public double SpeedInLightYearsPerMinute
        {
            get
            {
                return Constants.BaseSpeed + DriveCount.Value * Constants.DriveBonusInSpeed;
            }
        }

        /// <summary>
        /// Gets the sensor range of the spaceship in light years.
        /// This depends on the current equipment loadout of the ship.
        /// </summary>
        public double SensorRangeInLightYears
        {
            get
            {
                return Constants.BaseRange + SensorCount.Value * Constants.SensorBonusInRange;
            }
        }

        /// <summary>
        /// Gets the free hold capacity of the ship.
        /// </summary>
        public int FreeCapacity
        {
            get
            {
                return TotalCapacity - OccupiedCapacity;
            }
        }

        /// <summary>
        /// Gets the occupied hold capacity of the ship.
        /// </summary>
        public int OccupiedCapacity
        {
            get
            {
                return CommodityInHolds.Sum(i => (int?)i.Count ?? 0) +
                    SensorCount.Value * Constants.SensorCostInCapacity +
                    DriveCount.Value * Constants.DriveCostInCapacity +
                    CannonCount.Value * Constants.CannonCostInCapacity +
                    ShieldCount.Value * Constants.ShieldCostInCapacity;
            }
        }

        /// <summary>
        /// Gets the total hold space of the ship.
        /// </summary>
        public int TotalCapacity
        {
            get
            {
                return Constants.ModelTypes[this.ShipModel.Value].Capacity;
            }
        }

        /// <summary>
        /// Gets whether the ship is flying.
        /// </summary>
        public bool IsInTransit
        {
            get
            {
                return !(LaunchDate == null);
            }
        }
    }
}