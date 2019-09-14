using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Andromeda.Data;
using Andromeda.EventLogEntries;
using Andromeda.EventLog;
using System.Data.Linq;
using Andromeda.Common;

namespace Andromeda.Scheduler
{
    public static class ScheduledTasks
    {
        private static AndromedaDataContext db = DataContextFactory.GetAndromedaDataContext();

        private static DateTime IncreaseCommodityQuantities_LastRunTime = DateTime.MinValue;
        private static TimeSpan IncreaseCommodityQuantities_MinimumInterval = TimeSpan.FromMinutes(30);

        /// <summary>
        /// If enough time has elapsed since the last increase, increases the stock count for all commodities.
        /// </summary>
        public static void IncreaseCommodityQuantities()
        {
            #region Log update
            bool shouldBeLogged = false;
            bool alreadyLogged = false;
            StockIncrease logEntry = new StockIncrease()
            {
                Guid = Guid.Empty,
                Player = string.Empty,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            int exceptionCount = 0;
            try
            {
                if (IncreaseCommodityQuantities_LastRunTime + IncreaseCommodityQuantities_MinimumInterval <= TimeGetter.GetLocalTime())
                {
                    #region Log update
                    shouldBeLogged = true;
                    #endregion
                    IncreaseCommodityQuantities_LastRunTime = TimeGetter.GetLocalTime();

                    var commodityAtStarsIds = db.CommodityAtStars.ToList().Select(i => new { i.Id });

                    //Increase all commodities
                    //All done in separate data contexts to help successfully continue after an optimistic concurrency exception
                    foreach (var commodityAtStarId in commodityAtStarsIds)
                    {
                        try
                        {
                            using (var db2 = DataContextFactory.GetAndromedaDataContext())
                            {
                                var commodityAtStar = db2.CommodityAtStars.Single(i => i.Id == commodityAtStarId.Id);

                                if (commodityAtStar.Stock.Value + commodityAtStar.ProductionRate.Value > commodityAtStar.MaxCapacity)
                                {
                                    commodityAtStar.Stock = commodityAtStar.MaxCapacity;
                                }
                                else
                                {
                                    commodityAtStar.Stock += commodityAtStar.ProductionRate.Value;
                                }

                                db2.SubmitChanges();
                            }
                        }
                        catch
                        {
                            exceptionCount++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                #region Log update
                if (shouldBeLogged)
                {
                    logEntry.Error = ex.ToString();
                    Logger.Log(logEntry);
                    alreadyLogged = true;
                }
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (shouldBeLogged && !alreadyLogged)
                {
                    logEntry.Error = exceptionCount.ToString() + " exceptions";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }
    }
}