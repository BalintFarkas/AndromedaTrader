using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Andromeda;
using System.ServiceModel.Activation;
using Andromeda.Data;
using Andromeda.ClientEntities;
using Andromeda.ServerEntities;
using Andromeda.Common;
using Andromeda.EventLog;
using Andromeda.EventLogEntries;
using System.Web;
using System.ServiceModel.Channels;
using AndromedaServer.WebRole;

namespace Andromeda.WebServices
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Logging is interwoven into the code of methods that need to be logged. The log entry is filled
    /// with information as soon as it is available (to make sure that no matter where an exception happens, the most possible 
    /// information gets out) and saved after the method execution is complete (when we know whether an exception occurred or not).
    /// </remarks>
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(
        IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.PerCall)]
    public class AndromedaService : IAndromedaService
    {
        private AndromedaDataContext db = DataContextFactory.GetAndromedaDataContext();

        public Andromeda.ClientEntities.Spaceship GetSpaceshipStatus(Guid playerGuid)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Create and return a view model based on current ship status
            var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
            spaceship.UpdateTravelStatus(); //Make sure we return the latest info
            return new Andromeda.ClientEntities.Spaceship()
            {
                Cargo = spaceship.CommodityInHolds.Select(i => new Andromeda.ClientEntities.CommodityInHold()
                    {
                        Name = i.Commodity.Name,
                        Stock = i.Count.Value
                    }).ToList(),
                FreeCapacity = spaceship.FreeCapacity,
                IsInTransit = spaceship.IsInTransit,
                Money = spaceship.Player.PlayerMoney.Value,
                TotalCapacity = spaceship.TotalCapacity,
                SpeedInLightYearsPerMinute = spaceship.SpeedInLightYearsPerMinute,
                SensorRangeInLightYears = spaceship.SensorRangeInLightYears,
                DriveCount = spaceship.DriveCount.Value,
                SensorCount = spaceship.SensorCount.Value,
                ModificationsRemaining = Constants.ModificationLimit - spaceship.ModificationCount.Value,
                CannonCount = spaceship.CannonCount.Value,
                ShieldCount = spaceship.ShieldCount.Value,
                LastRaided = spaceship.LastRaided.Value,
                TotalShipsOwnedByPlayer = spaceship.Player.Spaceships.Count
            };
        }

        public Andromeda.ClientEntities.Star GetCurrentStar(Guid playerGuid)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Fetch values from DB
            var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
            spaceship.UpdateTravelStatus();
            if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

            //Return star
            var currentStar = db.Stars.Single(i => i.Id == spaceship.CurrentStarId);
            var coordinates = spaceship.Coordinates;
            return ConvertServerStarToClientStar(currentStar, coordinates.Item1, coordinates.Item2);
        }

        public List<Andromeda.ClientEntities.Star> GetVisibleStars(Guid playerGuid)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Create memory space for result
            List<Andromeda.ClientEntities.Star> resultStars = new List<Andromeda.ClientEntities.Star>();

            //Fetch values from DB
            var stars = db.Stars.ToList(); //TODO Excellent candidate for caching
            var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);

            //Fetch current ship coordinates
            var coordinates = spaceship.Coordinates;

            //Check for each star whether it is in range
            //If it is, create a ViewModel from it and add it to the Results collection
            foreach (var star in stars)
            {
                double distanceToSpaceship = DistanceCalculator.GetDistance(coordinates.Item1, coordinates.Item2, star.X.Value, star.Y.Value);
                if (distanceToSpaceship <= spaceship.SensorRangeInLightYears)
                {
                    resultStars.Add(ConvertServerStarToClientStar(star, coordinates.Item1, coordinates.Item2));
                }
            }

            //Done
            return resultStars;
        }

        public void RegisterRunLocation(Guid playerGuid, bool isEmulator)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Perform registration
            var player = db.Players.Single(i => i.FirstShipGuid == playerGuid);
            player.IsRunLocationEmulator = isEmulator;
            player.RunLocationIp = HttpContext.Current.Request.UserHostAddress;
            player.RunLocationTimestamp = TimeGetter.GetLocalTime();

            //If the user runs from Azure, log the touch
            if (player.IsRunLocationEmulator == false)
            {
                AzureTouch touch;
                if (db.AzureTouches.Any(i => i.PlayerGuid == playerGuid))
                {
                    touch = db.AzureTouches.First(i => i.PlayerGuid == playerGuid);
                }
                else
                {
                    touch = new AzureTouch()
                    {
                        PlayerGuid = playerGuid,
                        UserName = player.PlayerName
                    };
                    db.AzureTouches.InsertOnSubmit(touch);
                }
                touch.LastTouchDate = TimeGetter.GetLocalTime();

                //Detect client IP using WCF
                var props = OperationContext.Current.IncomingMessageProperties;
                var endpointProperty = props[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpointProperty != null)
                {
                    touch.LastTouchIp = endpointProperty.Address;
                }
            }

            db.SubmitChanges();
        }

        public void LaunchSpaceship(Guid playerGuid, Guid starGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            Launch logEntry = new Launch()
            {
                Guid = playerGuid,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();

                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                var log_currentStar = db.Stars.Single(i => i.Id == spaceship.CurrentStarId);
                logEntry.From = log_currentStar.Name;
                logEntry.FromId = log_currentStar.Id;
                #endregion

                //If ship is travelling it may not launch again
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Get target star
                var star = db.Stars.Single(i => i.Guid == starGuid);

                #region Log update
                logEntry.To = star.Name;
                logEntry.ToId = star.Id;
                #endregion

                //If the target star is the same as the current star, the ship may not launch
                if (star.Id == spaceship.CurrentStarId) throw new Exception(Localization.Error_AlreadyInTargetSystem);

                //If the star is out of range, the ship may not launch
                var coordinates = spaceship.Coordinates;
                if (DistanceCalculator.GetDistance(coordinates.Item1, coordinates.Item2, star.X.Value, star.Y.Value) > spaceship.SensorRangeInLightYears)
                {
                    throw new Exception(Localization.Error_StarIsTooFar);
                }

                //Everything is OK - launch!
                spaceship.TargetStarId = star.Id;
                spaceship.LaunchDate = TimeGetter.GetLocalTime();
                spaceship.ModificationCount = 0; //Clear mod count - starship will be modifiable again on the next star
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void Buy(Guid playerGuid, string commodityName, int quantity)
        {
            #region Log update
            bool alreadyLogged = false;
            Trade logEntry = new Trade()
            {
                Commodity = commodityName,
                IsPurchase = true,
                Guid = playerGuid,
                Quantity = quantity,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //Filter negatives
                if (quantity < 0) throw new Exception(Localization.Error_NoNegativeQuantity);

                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                var star = db.Stars.Single(i => i.Id == spaceship.CurrentStarId);

                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                logEntry.Star = star.Name;
                logEntry.StarId = star.Id;
                #endregion

                //If ship is travelling it may not trade
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Check if the commodity is found on the star and there is enough of it
                if (!star.CommodityAtStars.Any(i => i.Commodity.Name == commodityName)) throw new Exception(Localization.Error_NotTradedHere);
                var commodityAtStar = star.CommodityAtStars.Single(i => i.Commodity.Name == commodityName);
                #region Log update
                logEntry.UnitPrice = commodityAtStar.BuyPrice.Value;
                #endregion
                if (commodityAtStar.Stock < quantity) throw new Exception(Localization.Error_NotEnoughForSale);

                //Check if the player has enough money and space
                if (commodityAtStar.BuyPrice * quantity > spaceship.Player.PlayerMoney) throw new Exception(Localization.Error_NotEnoughMoneyForTrade);
                if (spaceship.FreeCapacity < quantity) throw new Exception(Localization.Error_NotEnoughCargoSpace);

                //Perform the transaction
                if (quantity == 0) return; //Buying 0 units is null action, but we have to exit here, or a record containing 0 units would be created
                spaceship.Player.PlayerMoney -= commodityAtStar.BuyPrice * quantity;
                #region Log update
                logEntry.NewPlayerMoney = spaceship.Player.PlayerMoney.Value;
                #endregion
                commodityAtStar.Stock -= quantity;
                Andromeda.ServerEntities.CommodityInHold commodityInHold;
                if (spaceship.CommodityInHolds.Any(i => i.Commodity.Name == commodityName))
                {
                    commodityInHold = spaceship.CommodityInHolds.Single(i => i.Commodity.Name == commodityName);
                }
                else
                {
                    commodityInHold = new Andromeda.ServerEntities.CommodityInHold()
                    {
                        Spaceship = spaceship,
                        Commodity = commodityAtStar.Commodity,
                        Count = 0,
                        NetWorth = 0
                    };
                    db.CommodityInHolds.InsertOnSubmit(commodityInHold);
                }
                commodityInHold.Count += quantity;
                commodityInHold.NetWorth += commodityAtStar.BuyPrice * quantity; //For inclusion in Net Worth calculation

                //Commit
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void BuyMaximum(Guid playerGuid, string commodityName)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Get ship and star
            var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
            spaceship.UpdateTravelStatus();
            var star = db.Stars.Single(i => i.Id == spaceship.CurrentStarId);

            //Check if the ship is landed and if the commodity is found on the star
            //Detailed constraints will be checked in the actual Buy method - here we only check the constrainst essential to the working of this function
            if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);
            if (!star.CommodityAtStars.Any(i => i.Commodity.Name == commodityName)) throw new Exception(Localization.Error_NotTradedHere);
            var commodityAtStar = star.CommodityAtStars.Single(i => i.Commodity.Name == commodityName);

            //Calculate constraints on maximum amount that can be purchased
            int maximumByPrice = int.MaxValue;
            if (commodityAtStar.BuyPrice.Value != 0) //Protect against DBZ
                maximumByPrice = (int)(spaceship.Player.PlayerMoney.Value / commodityAtStar.BuyPrice.Value); //Because of integer truncation the value is rounded to the lower bound if fractional - this is just fine
            int maximumByCargoHold = spaceship.FreeCapacity;
            int maximumByStock = commodityAtStar.Stock.Value;

            //Get actual maximum value - the smallest of the constraints
            int maximum = Math.Min(Math.Min(maximumByPrice, maximumByCargoHold), maximumByStock);

            //Execute actual purchase
            Buy(playerGuid, commodityName, maximum);
        }

        public void Sell(Guid playerGuid, string commodityName, int quantity)
        {
            #region Log update
            bool alreadyLogged = false;
            Trade logEntry = new Trade()
            {
                Commodity = commodityName,
                IsPurchase = false,
                Guid = playerGuid,
                Quantity = quantity,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //Filter negatives
                if (quantity < 0) throw new Exception(Localization.Error_NoNegativeQuantity);

                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                var star = db.Stars.Single(i => i.Id == spaceship.CurrentStarId);

                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                logEntry.Star = star.Name;
                logEntry.StarId = star.Id;
                #endregion

                //If ship is travelling it may not trade
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Check if the commodity is found on the star and can be sold
                if (!star.CommodityAtStars.Any(i => i.Commodity.Name == commodityName)) throw new Exception(Localization.Error_NotTradedHere);
                var commodityAtStar = star.CommodityAtStars.Single(i => i.Commodity.Name == commodityName);
                #region Log update
                logEntry.UnitPrice = commodityAtStar.BuyPrice.Value;
                #endregion
                if (commodityAtStar.IsSellable != true) throw new Exception(Localization.Error_NotBoughtHere);

                //Check if the player has this commodity, and has enough of it
                if (!spaceship.CommodityInHolds.Any(i => i.Commodity.Name == commodityName)) throw new Exception(Localization.Error_NotInHold);
                var commodityInHold = spaceship.CommodityInHolds.Single(i => i.Commodity.Name == commodityName);
                if (commodityInHold.Count < quantity) throw new Exception(Localization.Error_NotEnoughInHold);

                //Perform the transaction
                spaceship.Player.PlayerMoney += commodityAtStar.SellPrice * quantity;
                #region Log update
                logEntry.NewPlayerMoney = spaceship.Player.PlayerMoney.Value;
                #endregion
                commodityInHold.Count -= quantity;
                commodityInHold.NetWorth -= (long)
                    ((double)quantity / (double)commodityInHold.Count) * commodityInHold.NetWorth; //Decrease the Net Worth of the stock by the calculated percentage
                if (commodityInHold.Count == 0) db.CommodityInHolds.DeleteOnSubmit(commodityInHold);

                //Commit
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void AddSensor(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 1,
                Guid = playerGuid,
                IsIncrease = true,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Enough hold space?
                if (spaceship.FreeCapacity < Constants.SensorCostInCapacity) throw new Exception(Localization.Error_NotEnoughSpaceForComponent);

                //Perform modification
                spaceship.SensorCount++;
                #region Log update
                logEntry.NewQuantity = spaceship.SensorCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void RemoveSensor(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 1,
                Guid = playerGuid,
                IsIncrease = false,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Any sensors to remove?
                if (spaceship.SensorCount <= 0) throw new Exception(Localization.Error_ComponentNotPresent);

                //Perform modification
                spaceship.SensorCount--;
                #region Log update
                logEntry.NewQuantity = spaceship.SensorCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void AddDrive(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 0,
                Guid = playerGuid,
                IsIncrease = true,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Enough hold space?
                if (spaceship.FreeCapacity < Constants.DriveCostInCapacity) throw new Exception(Localization.Error_NotEnoughSpaceForComponent);

                //Perform modification
                spaceship.DriveCount++;
                #region Log update
                logEntry.NewQuantity = spaceship.DriveCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void RemoveDrive(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 0,
                Guid = playerGuid,
                IsIncrease = false,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Any drives to remove?
                if (spaceship.DriveCount <= 0) throw new Exception(Localization.Error_ComponentNotPresent);

                //Perform modification
                spaceship.DriveCount--;
                #region Log update
                logEntry.NewQuantity = spaceship.DriveCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void UpgradeShip(Guid playerGuid, int modelType)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipUpgrade logEntry = new ShipUpgrade()
            {
                Guid = playerGuid,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                logEntry.OldModel = spaceship.ShipModel.Value;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Validate model type
                if (modelType != 1 && modelType != 2) throw new Exception(Localization.Error_InvalidShipModel);
                #region Log update
                logEntry.NewModel = modelType;
                #endregion

                //Check possibility of upgrade
                if (modelType <= spaceship.ShipModel) throw new Exception(Localization.Error_ShipModelAlreadyBetter);

                //Check affordability of upgrade
                if (spaceship.Player.PlayerMoney < Constants.ModelTypes[modelType].Cost)
                    throw new Exception(string.Format(Localization.Error_NotEnoughMoneyForShipModel,
                        Constants.ModelTypes[modelType].Cost));

                //Perform upgrade
                spaceship.Player.PlayerMoney -= Constants.ModelTypes[modelType].Cost;
                spaceship.ShipModel = modelType;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void AddCannon(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 2,
                Guid = playerGuid,
                IsIncrease = true,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Enough hold space?
                if (spaceship.FreeCapacity < Constants.CannonCostInCapacity) throw new Exception(Localization.Error_NotEnoughSpaceForComponent);

                //Perform modification
                spaceship.CannonCount++;
                #region Log update
                logEntry.NewQuantity = spaceship.CannonCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void RemoveCannon(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 2,
                Guid = playerGuid,
                IsIncrease = false,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Any cannons to remove?
                if (spaceship.CannonCount <= 0) throw new Exception(Localization.Error_ComponentNotPresent);

                //Perform modification
                spaceship.CannonCount--;
                #region Log update
                logEntry.NewQuantity = spaceship.CannonCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void AddShield(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 3,
                Guid = playerGuid,
                IsIncrease = true,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Enough hold space?
                if (spaceship.FreeCapacity < Constants.ShieldCostInCapacity) throw new Exception(Localization.Error_NotEnoughSpaceForComponent);

                //Perform modification
                spaceship.ShieldCount++;
                #region Log update
                logEntry.NewQuantity = spaceship.ShieldCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void RemoveShield(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipModification logEntry = new ShipModification()
            {
                ComponentId = 3,
                Guid = playerGuid,
                IsIncrease = false,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship and star
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                spaceship.UpdateTravelStatus();
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                #endregion
                if (spaceship.IsInTransit) throw new Exception(Localization.Error_ShipIsInSpace);

                //Can still be modified?
                if (spaceship.ModificationCount >= Constants.ModificationLimit) throw new Exception(Localization.Error_ModificationLimitReached);

                //Any shields to remove?
                if (spaceship.ShieldCount <= 0) throw new Exception(Localization.Error_ComponentNotPresent);

                //Perform modification
                spaceship.ShieldCount--;
                #region Log update
                logEntry.NewQuantity = spaceship.ShieldCount.Value;
                #endregion
                spaceship.ModificationCount++;
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public List<MerchantShip> GetRaidableShips(Guid playerGuid)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Get ship
            var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
            var player = spaceship.Player;

            //Get ships that have not been raided in the last 10 minutes and have existed for at least 1 day
            //(And of course not the player)
            DateTime raidGracePeriodLimit = TimeGetter.GetLocalTime().Subtract(Constants.RaidGracePeriod);
            DateTime creationGracePeriodLimit = TimeGetter.GetLocalTime().Subtract(Constants.CreationGracePeriod);
            var notInGracePeriod = db.Spaceships.Where(i => i.Deleted == false && i.Player.FirstShipGuid != player.FirstShipGuid && i.LastRaided < raidGracePeriodLimit && i.Created < creationGracePeriodLimit).ToList();

            //Check distance between the respective ships and return those that are in range
            List<MerchantShip> merchantShips = new List<MerchantShip>();
            var coords = spaceship.Coordinates;
            foreach (var targetCandidate in notInGracePeriod)
            {
                var targetCoords = targetCandidate.Coordinates;
                var targetDistance = DistanceCalculator.GetDistance(coords.Item1, coords.Item2, targetCoords.Item1, targetCoords.Item2);
                if (targetDistance <= spaceship.SensorRangeInLightYears)
                {
                    merchantShips.Add(new MerchantShip() { TransponderCode = targetCandidate.TransponderCode.Value, Distance = targetDistance });
                }
            }
            return merchantShips;
        }

        public void Raid(Guid playerGuid, Guid transponderCode)
        {
            #region Log update
            bool alreadyLogged = false;
            PirateRaid logEntry = new PirateRaid()
            {
                Guid = playerGuid,
                Timestamp = TimeGetter.GetLocalTime(),
                TargetTransponder = transponderCode
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get ship
                var spaceship = db.Spaceships.Single(i => i.PlayerGuid == playerGuid);
                #region Log update
                logEntry.Player = spaceship.Player.PlayerName;
                logEntry.AttackerCannons = spaceship.CannonCount.Value;
                #endregion
                var player = spaceship.Player;

                //Get target
                if (!db.Spaceships.Any(i => i.TransponderCode == transponderCode)) throw new Exception(Localization.Error_InvalidTransponderCode);
                var target = db.Spaceships.Single(i => i.TransponderCode == transponderCode);
                #region Log update
                logEntry.TargetPlayer = target.Player.PlayerName;
                logEntry.DefenderShields = target.ShieldCount.Value;
                #endregion

                //If the target is the player himself, quit
                if (spaceship.Player.FirstShipGuid == target.Player.FirstShipGuid)
                {
                    throw new Exception(Localization.Error_CannotAttackYourself);
                }

                //Check if target is in grace period
                if (target.LastRaided.Value.Add(Constants.RaidGracePeriod) > TimeGetter.GetLocalTime())
                {
                    throw new Exception(Localization.Error_ShipRaidedRecently);
                }
                if (target.Created.Value.Add(Constants.CreationGracePeriod) > TimeGetter.GetLocalTime())
                {
                    throw new Exception(Localization.Error_ShipCreatedRecently);
                }

                //Check if target is in distance
                var coords = spaceship.Coordinates;
                var targetCoords = target.Coordinates;
                var distance = DistanceCalculator.GetDistance(coords.Item1, coords.Item2, targetCoords.Item1, targetCoords.Item2);
                #region Log update
                logEntry.Distance = distance;
                #endregion
                if (distance > spaceship.SensorRangeInLightYears)
                {
                    throw new Exception(Localization.Error_ShipOutOfRange);
                }

                //Perform raid
                int distanceModifier = (int)Math.Floor(distance / 10);
                int attackerStrength = spaceship.CannonCount.Value - distanceModifier;
                int defenderStrength = target.ShieldCount.Value;

                //Player won the engagement
                if (attackerStrength > defenderStrength)
                {
                    #region Log update
                    logEntry.IsSuccessful = true;
                    #endregion
                    //Grab his stuff!
                    //Grabbing one by one, until either storage space or stuff available runs out
                    //Carefully also transferring net worth and deleting empty CommodityInHolds
                    int stolenAmount = 0;
                    while (spaceship.FreeCapacity > 0 && target.CommodityInHolds.Count > 0)
                    {
                        var commodityStolen = target.CommodityInHolds.First();
                        if (commodityStolen.Count > 0)
                        {
                            Andromeda.ServerEntities.CommodityInHold commodityAddedTo;
                            if (spaceship.CommodityInHolds.Any(i => i.Commodity == commodityStolen.Commodity))
                            {
                                commodityAddedTo = spaceship.CommodityInHolds.First(i => i.Commodity == commodityStolen.Commodity);
                            }
                            else
                            {
                                commodityAddedTo = new ServerEntities.CommodityInHold()
                                {
                                    Commodity = commodityStolen.Commodity,
                                    Count = 0,
                                    NetWorth = 0,
                                    Spaceship = spaceship,
                                };
                                db.CommodityInHolds.InsertOnSubmit(commodityAddedTo);
                            }

                            long netWorthTransfer = (long)((double)1 / (double)commodityStolen.Count.Value * (double)commodityStolen.NetWorth.Value);

                            commodityAddedTo.Count++;
                            commodityAddedTo.NetWorth += netWorthTransfer;

                            commodityStolen.Count--;
                            commodityStolen.NetWorth -= netWorthTransfer;

                            stolenAmount++;
                        }
                        else
                        {
                            target.CommodityInHolds.Remove(commodityStolen);
                            db.CommodityInHolds.DeleteOnSubmit(commodityStolen);
                        }
                    }
                    #region Log update
                    logEntry.AmountStolen = stolenAmount;
                    #endregion

                    target.LastRaided = TimeGetter.GetLocalTime();
                    db.SubmitChanges();
                }
                //Player lost the engagement
                else
                {
                    #region Log update
                    logEntry.IsSuccessful = false;
                    logEntry.AmountStolen = 0;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public void BuyNewShip(Guid playerGuid)
        {
            #region Log update
            bool alreadyLogged = false;
            ShipPurchase logEntry = new ShipPurchase()
            {
                Guid = playerGuid,
                Timestamp = TimeGetter.GetLocalTime()
            };
            #endregion

            try
            {
                //No such player
                if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

                //Get player
                var player = db.Players.Single(i => i.FirstShipGuid == playerGuid);
                #region Log update
                logEntry.Player = player.PlayerName;
                logEntry.NewCount = player.Spaceships.Count + 1;
                #endregion

                //Check count
                if (player.Spaceships.Count >= Constants.MaxShipCount) throw new Exception(Localization.Error_ShipLimitReached);

                //Check money
                if (Constants.NewShipCosts[player.Spaceships.Count - 1] > player.PlayerMoney) throw new Exception(string.Format(Localization.Error_NotEnoughMoneyForShipPurchase, Constants.NewShipCosts[player.Spaceships.Count - 1]));

                //Perform purchase
                player.PlayerMoney -= Constants.NewShipCosts[player.Spaceships.Count - 1];
                Random rnd = new Random((int)DateTime.Now.Ticks);
                Andromeda.ServerEntities.Spaceship newShip = new ServerEntities.Spaceship()
                {
                    CannonCount = 0,
                    Created = TimeGetter.GetLocalTime(),
                    CurrentStarId = rnd.Next(0, AndromedaCache.Stars.Count),
                    Deleted = false,
                    DriveCount = 0,
                    LastRaided = TimeGetter.GetLocalTime(),
                    ModificationCount = 0,
                    Player = player,
                    PlayerGuid = Guid.NewGuid(),
                    SensorCount = 0,
                    ShieldCount = 0,
                    ShipModel = 0,
                    TransponderCode = Guid.NewGuid()
                };
                db.Spaceships.InsertOnSubmit(newShip);
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.Message;
                Logger.Log(logEntry);
                alreadyLogged = true;
                throw ex;
                #endregion
            }
            finally
            {
                #region Log update
                if (!alreadyLogged)
                {
                    logEntry.Error = "";
                    Logger.Log(logEntry);
                }
                #endregion
            }
        }

        public List<Guid> GetOwnedShips(Guid playerGuid)
        {
            //No such player
            if (!db.Spaceships.Any(i => i.PlayerGuid == playerGuid)) throw new Exception(Localization.Error_InvalidPlayerId);

            //Get player
            var player = db.Players.Single(i => i.FirstShipGuid == playerGuid);

            //Return player's owned ships
            return player.Spaceships.Select(i => i.PlayerGuid.Value).ToList();
        }

        /// <summary>
        /// Converts a server Star to a client Star. Used in multiple method calls, so factored out.
        /// </summary>
        /// <param name="star"></param>
        /// <param name="spaceshipX"></param>
        /// <param name="spaceshipY"></param>
        /// <returns></returns>
        private Andromeda.ClientEntities.Star ConvertServerStarToClientStar(Andromeda.ServerEntities.Star star, double spaceshipX, double spaceshipY)
        {
            return new Andromeda.ClientEntities.Star()
                {
                    Name = star.Name,
                    DistanceInLightYears = DistanceCalculator.GetDistance(spaceshipX, spaceshipY, star.X.Value, star.Y.Value),
                    StarGuid = star.Guid.Value,
                    Commodities = star.CommodityAtStars.Select(i => new Andromeda.ClientEntities.CommodityAtStar()
                    {
                        Name = i.Commodity.Name,
                        Price = i.BuyPrice.Value, //TODO Change if buyprice != sellprice
                        Stock = i.Stock.Value
                    }).ToList()
                };
        }
    }
}