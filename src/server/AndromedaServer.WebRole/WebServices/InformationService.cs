using Andromeda.Common;
using Andromeda.EventLog;
using Andromeda.EventLogEntries;
using Andromeda.ServerEntities;
using AndromedaServer.WebRole;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Andromeda.WebServices
{
    /// <summary>
    /// This web service serves public information requests (i.e. the Map.aspx page).
    /// Another web service serves the requests of player scaffolds.
    /// </summary>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerCall)]
    public class InformationService : IInformationService
    {
        #region Get Leaderboard
        /// <summary>
        /// This method is called via AJAX. Returns current leaderboard.
        /// </summary>
        /// <returns></returns>
        public List<LeaderboardItem> GetLeaderboard()
        {
            return GetLeaderboardInternal(8);
        }

        /// <summary>
        /// This method is called via AJAX. Returns current entire leaderboard.
        /// </summary>
        /// <returns></returns>
        public string GetFullLeaderboard()
        {
            return GetLeaderboardAsString(int.MaxValue);
        }

        private List<LeaderboardItem> GetLeaderboardInternal(int count)
        {
            var db = DataContextFactory.GetAndromedaDataContext();

            return db.Players
                .Where(i => i.IsRunLocationEmulator == false)
                .Select(i => new LeaderboardItem()
                {
                    PlayerName = i.PlayerName,
                    NetWorth = i.PlayerMoney.Value + (i.Spaceships.Where(j => j.Deleted == false)
                        .Sum(j => j.CommodityInHolds.Sum(k => k.NetWorth) ?? 0))
                })
                .OrderByDescending(i => i.NetWorth)
                .Take(count).ToList();
        }
        #endregion

        #region Get Stars and Ships
        /// <summary>
        /// This method is called via AJAX. Returns stars to display on map.
        /// </summary>
        /// <returns></returns>
        public List<MapObject> GetStars()
        {
            var db = DataContextFactory.GetAndromedaDataContext();
            return db.Stars.ToList().Select(i => new MapObject()
            {
                Name = i.Name,
                Color = "#999999",
                X = (int)i.X.Value / 2,
                Y = (int)i.Y.Value / 2
            })
                .ToList();
        }

        /// <summary>
        /// This method is called via AJAX. Returns ships to display on map.
        /// </summary>
        /// <returns></returns>
        public List<MapObject> GetShips(string playerGuid)
        {
            var db = DataContextFactory.GetAndromedaDataContext();

            //Check if the user has a ship
            //Did we get a valid GUID?
            Guid? guid;
            Guid tempGuid;
            if (Guid.TryParse(playerGuid, out tempGuid)) guid = tempGuid; else guid = null;
            //Is there a ship with this GUID?
            if (guid != null)
            {
                if (!db.Spaceships.Any(i => i.PlayerGuid == guid)) guid = null;
            }

            //Return ships
            var ships = db.Spaceships.Where(i => i.Deleted == false).ToList();
            List<MapObject> mapObjects = new List<MapObject>();
            foreach (var i in ships)
            {
                var coordinates = i.Coordinates;

                mapObjects.Add(
                    new MapObject()
                    {
                        Name = Localization.Map_Spaceship + i.Player.PlayerName,
                        Color = (guid != null && i.Player.FirstShipGuid == guid) ? "lightgreen" : "red", //If the Guid is filled (the user has a ship) and this is the player's ship, mark it green, otherwise it's red
                        SensorRange = (guid != null && i.Player.FirstShipGuid == guid) ? i.SensorRangeInLightYears : 0d,
                        CannonRange = (guid != null && i.Player.FirstShipGuid == guid) ? (i.CannonCount ?? 0) * 20d : 0d,
                        X = (int)coordinates.Item1 / 2,
                        Y = (int)coordinates.Item2 / 2
                    }
                );
            }
            return mapObjects;
        }
        #endregion

        #region Get Textual Status for Map Page
        /// <summary>
        /// This method is called via AJAX. It returns the ticker and the user's detailed status info.
        /// </summary>
        /// <returns></returns>
        public List<string> GetStatus(string playerGuid)
        {
            return new List<string>()
            {
                GetTicker(),
                GetLeaderboardAsString(10),
                GetPilotStatus(playerGuid)
            };
        }

        /// <summary>
        /// Generates top 10 ticker entries for all users.
        /// </summary>
        /// <returns></returns>
        private string GetTicker()
        {
            return GetTicker(Guid.Empty, 100);
        }

        /// <summary>
        /// Generates ticker string for the specified user and with the specified count.
        /// If the guid is empty, ticker for all users is returned.
        /// </summary>
        /// <returns></returns>
        private string GetTicker(Guid userGuid, int count)
        {
            var db = DataContextFactory.GetAndromedaDataContext();

            var lastEvents = db.EventLogEntries.OrderByDescending(i => i.Timestamp)
                .Where(i => i.Error == "" && (i.EventType == "Launch" || i.EventType == "ShipModification" || i.EventType == "Trade" || i.EventType == "ShipUpgrade" || i.EventType == "PirateRaid" || i.EventType == "ShipPurchase"));

            if (userGuid != Guid.Empty)
            {
                string username = db.Players.Single(i => i.FirstShipGuid == userGuid).PlayerName;
                lastEvents = lastEvents.Where(i => i.Player == username);
            }

            lastEvents = lastEvents.Take(count);

            string ticker = "";

            bool shouldSeparatorBePlaced = lastEvents.Any(i => i.Timestamp > DateTime.Now - TimeSpan.FromSeconds(5));
            bool separatorPlacedYet = false;
            foreach (var evt in lastEvents)
            {
                if (shouldSeparatorBePlaced && !separatorPlacedYet && evt.Timestamp <= (DateTime.Now - TimeSpan.FromSeconds(5)))
                {
                    ticker += "<hr />";
                    separatorPlacedYet = true;
                }
                ticker += string.Format("<b>{0}:{1}:{2}:</b> ",
                    evt.Timestamp.Hour,
                    evt.Timestamp.Minute.ToString().PadLeft(2, '0'),
                    evt.Timestamp.Second.ToString().PadLeft(2, '0'))
                + ConvertLogToDetailString(evt) + "<br />";
            }

            return ticker;
        }

        /// <summary>
        /// Converts a raw log entry to Ticker-displayable string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string ConvertLogToDetailString(EventLogEntry input)
        {
            var logEntry = Logger.ConvertEventXmlToObject(input.EventType, input.Xml);

            if (input.EventType == "Launch")
            {
                var typedLogEntry = (Launch)logEntry;

                return string.Format(Localization.Log_Launch,
                    typedLogEntry.Player, typedLogEntry.From, typedLogEntry.To);
            }
            else if (input.EventType == "ShipModification")
            {
                var typedLogEntry = (ShipModification)logEntry;

                return string.Format(Localization.Log_ShipModification,
                    GetComponentName(typedLogEntry.ComponentId),
                    typedLogEntry.IsIncrease ?
                        Localization.Log_ShipModification_Added :
                        Localization.Log_ShipModification_Removed,
                    typedLogEntry.IsIncrease ?
                        Localization.Log_ShipModification_ToShip :
                        Localization.Log_ShipModification_FromShip,
                    typedLogEntry.Player,
                    typedLogEntry.NewQuantity);
            }
            else if (input.EventType == "Trade")
            {
                var typedLogEntry = (Trade)logEntry;

                if (typedLogEntry.Quantity == 0)
                {
                    return string.Format(Localization.Log_Trade_Failed,
                        typedLogEntry.Player,
                        typedLogEntry.IsPurchase ?
                            Localization.Log_Trade_Failed_Buy :
                            Localization.Log_Trade_Failed_Sell,
                        typedLogEntry.Commodity);
                }
                else
                {
                    return string.Format(Localization.Log_Trade_Successful,
                        typedLogEntry.Player,
                        typedLogEntry.IsPurchase ?
                            Localization.Log_Trade_Successful_Bought :
                            Localization.Log_Trade_Successful_Sold,
                        typedLogEntry.Quantity,
                        typedLogEntry.Commodity,
                        typedLogEntry.Star,
                        typedLogEntry.UnitPrice,
                        typedLogEntry.NewPlayerMoney.ToString("N", new NumberFormatInfo() { NumberDecimalDigits = 0, NumberGroupSeparator = "." }));
                }
            }
            else if (input.EventType == "ShipUpgrade")
            {
                var typedLogEntry = (ShipUpgrade)logEntry;

                return string.Format(Localization.Log_ShipUpgrade,
                    typedLogEntry.Player,
                    Constants.ModelTypes[typedLogEntry.NewModel].Name,
                    Constants.ModelTypes[typedLogEntry.NewModel].Capacity);
            }
            else if (input.EventType == "PirateRaid")
            {
                var typedLogEntry = (PirateRaid)logEntry;

                if (typedLogEntry.IsSuccessful)
                {
                    return string.Format(Localization.Log_PirateRaid_Successful,
                        typedLogEntry.Player,
                        typedLogEntry.TargetPlayer,
                        typedLogEntry.AmountStolen);
                }
                else
                {
                    return string.Format(Localization.Log_PirateRaid_Failed,
                        typedLogEntry.Player,
                        typedLogEntry.TargetPlayer);
                }
            }
            else if (input.EventType == "ShipPurchase")
            {
                var typedLogEntry = (ShipPurchase)logEntry;

                return string.Format(Localization.Log_ShipPurchase,
                    typedLogEntry.Player, typedLogEntry.NewCount);
            }
            else
            {
                return Localization.Log_Unknown;
            }
        }

        /// <summary>
        /// Returns component name for id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetComponentName(int id)
        {
            switch (id)
            {
                case 0:
                    return Localization.ComponentName_Drive;
                case 1:
                    return Localization.ComponentName_Sensor;
                case 2:
                    return Localization.ComponentName_Cannon;
                case 3:
                    return Localization.ComponentName_Shield;
                default:
                    return Localization.ComponentName_Unknown;
            }
        }

        /// <summary>
        /// Returns current leaderboard immediately formatted as HTML.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private string GetLeaderboardAsString(int count)
        {
            var items = GetLeaderboardInternal(count);

            string result = "";
            for (int i = 0; i < items.Count; i++)
            {
                result += string.Format(Localization.LeaderboardItem,
                    i + 1, items[i].PlayerName, items[i].NetWorthAsString);
            }
            return result;
        }

        /// <summary>
        /// Generates pilot status string.
        /// </summary>
        /// <returns></returns>
        private string GetPilotStatus(string playerGuid)
        {
            var db = DataContextFactory.GetAndromedaDataContext();

            //Check if the user has a ship
            //Did we get a valid GUID?
            Guid? guid;
            Guid tempGuid;
            if (Guid.TryParse(playerGuid, out tempGuid)) guid = tempGuid; else guid = null;
            //Is there a ship with this GUID?
            if (guid != null)
            {
                if (!db.Spaceships.Any(i => i.PlayerGuid == guid)) guid = null;
            }
            //Return if the guid was invalid or if there was no ship found
            if (guid == null) return "";

            //Generate value
            string result = "";
            var player = db.Players.Single(i => i.FirstShipGuid == guid);

            NumberFormatInfo nfi = new NumberFormatInfo() { NumberDecimalDigits = 0, NumberGroupSeparator = "." };
            result += string.Format(Localization.FleetInfo_ShipsAndCredits,
                player.PlayerMoney.Value.ToString("N", nfi),
                player.Spaceships.Count);
            result += "<br />";

            int shipIndex = 0;
            foreach (var spaceship in player.Spaceships)
            {
                shipIndex++;
                result += string.Format(Localization.FleetInfo_ShipN, shipIndex);

                spaceship.UpdateTravelStatus();

                string travelStatus;
                if (spaceship.IsInTransit)
                {
                    var currentStar = db.Stars.Single(i => i.Id == spaceship.CurrentStarId.Value);
                    var targetStar = db.Stars.Single(i => i.Id == spaceship.TargetStarId.Value);
                    var coordinates = spaceship.Coordinates;

                    travelStatus = string.Format(Localization.FleetInfo_ShipEnRoute,
                        currentStar.Name,
                        targetStar.Name,
                        DistanceCalculator.GetDistance(currentStar.X.Value, currentStar.Y.Value, targetStar.X.Value, targetStar.Y.Value).ToString("0.0"),
                        DistanceCalculator.GetDistance(targetStar.X.Value, targetStar.Y.Value, coordinates.Item1, coordinates.Item2).ToString("0.0"));
                }
                else
                {
                    travelStatus = string.Format(Localization.FleetInfo_ShipInPort,
                        db.Stars.Single(i => i.Id == spaceship.CurrentStarId.Value).Name);
                }

                string hardwareStatus = string.Format(Localization.FleetInfo_Hardware,
                    spaceship.FreeCapacity, spaceship.TotalCapacity, spaceship.SpeedInLightYearsPerMinute, spaceship.SensorRangeInLightYears, spaceship.DriveCount, spaceship.SensorCount, spaceship.CannonCount, spaceship.ShieldCount);
                if (spaceship.SensorCount > 0 || spaceship.DriveCount > 0 || spaceship.CannonCount > 0 || spaceship.ShieldCount > 0) hardwareStatus += string.Format(Localization.FleetInfo_SpaceOccupiedByComponents, spaceship.DriveCount * Constants.DriveCostInCapacity + spaceship.SensorCount * Constants.SensorCostInCapacity + spaceship.CannonCount * Constants.CannonCostInCapacity + spaceship.ShieldCount * Constants.ShieldCostInCapacity);

                string cargoStatus;
                if (spaceship.CommodityInHolds.Count == 0)
                {
                    cargoStatus = string.Format(Localization.FleetInfo_NoCargo);
                }
                else
                {
                    string cargoInHold = "";
                    foreach (var cargo in spaceship.CommodityInHolds)
                    {
                        cargoInHold += string.Format(Localization.FleetInfo_CargoItem,
                            cargo.Commodity.Name,
                            cargo.Count);
                    }

                    cargoStatus = string.Format(Localization.FleetInfo_CargoHeader, cargoInHold);
                }

                //This string does not need localization
                result += string.Format("{0}<br /><br />{1}<br /><br />{2}", travelStatus, hardwareStatus, cargoStatus);
            }

            return result;
        }

        /// <summary>
        /// Fetches log for player.
        /// </summary>
        /// <param name="playerGuid"></param>
        /// <returns></returns>
        public string GetLog(string playerGuid)
        {
            return GetTicker(new Guid(playerGuid), 500);
        }
        #endregion
    }
}
