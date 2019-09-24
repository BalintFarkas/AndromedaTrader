using Andromeda.Common;
using Andromeda.Data;
using Andromeda.EventLog;
using Andromeda.EventLogEntries;
using Andromeda.ServerEntities;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Andromeda.WebPages
{
    public partial class Map : System.Web.UI.Page
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AndromedaDataContext db = new AndromedaDataContext();

            //Determine which panels to show
            if (!User.Identity.IsAuthenticated)
            {
                NoPilotPanel.Visible = true;
                PilotPanel.Visible = false;
                FromEmulatorPanel.Visible = false;
                leaderboardWarningSpan.Visible = false;
            }
            else
            {
                if (!db.Players.Any(i => i.PlayerName == User.Identity.Name))
                {
                    NoPilotPanel.Visible = true;
                    PilotPanel.Visible = false;
                    FromEmulatorPanel.Visible = false;
                    leaderboardWarningSpan.Visible = false;
                }
                else
                {
                    NoPilotPanel.Visible = false;
                    PilotPanel.Visible = true;
                    leaderboardWarningSpan.Visible = true;

                    var player = db.Players.First(i => i.PlayerName == User.Identity.Name);
                    PilotGuidLiteral.Text = player.FirstShipGuid.Value.ToString();

                    //If the IsRunLocationEmulator is null, the user hasn't run the emulator yet. Don't scare him.
                    //If it is false, then he's running from the cloud. Good job.
                    //If it is true, he is explicitly running from emulator.
                    if (player.IsRunLocationEmulator == true)
                    {
                        FromEmulatorPanel.Visible = true;
                    }
                    else
                    {
                        FromEmulatorPanel.Visible = false;
                    }

                    //Detailed status info will be loaded via AJAX.
                }
            }
        }

        protected void TeleportPilotButton_Click(object sender, EventArgs e)
        {
            AndromedaDataContext db = new AndromedaDataContext();

            if (!User.Identity.IsAuthenticated) return; //Avoid unauthenticated users
            if (!db.Players.Any(i => i.PlayerName == User.Identity.Name)) return; //There must be a player

            #region Log update
            bool alreadyLogged = false;
            PilotWarp logEntry = new PilotWarp()
            {
                Player = User.Identity.Name,
                Timestamp = DateTime.Now
            };
            #endregion

            Random rnd = new Random((int)DateTime.Now.Ticks);

            try
            {
                var player = db.Players.Single(i => i.PlayerName == User.Identity.Name);
                #region Log update
                logEntry.Guid = player.FirstShipGuid.Value;
                #endregion

                foreach (var spaceship in player.Spaceships)
                {
                    if (spaceship.DebugTimestamp != null && spaceship.DebugTimestamp.Value.Add(TimeSpan.FromHours(48)) > TimeGetter.GetLocalTime())
                    {
                        throw new Exception("Az űrhajó még nem teleportálható!");
                    }
                    else
                    {
                        var stars = db.Stars.Where(i => i.Id != spaceship.CurrentStarId).ToList();

                        var newStar = stars[rnd.Next(0, stars.Count)];

                        spaceship.CurrentStarId = newStar.Id;
                        spaceship.LaunchDate = null;
                        spaceship.TargetStarId = null;
                        spaceship.DebugTimestamp = TimeGetter.GetLocalTime();

                        db.SubmitChanges();
                    }
                }

                Response.Redirect("Map.aspx"); //Make the browser throw away the submitted POST so that a F5 does not repeat the Reset Pilot command
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.ToString();
                Logger.Log(logEntry);
                alreadyLogged = true;
                //throw ex; WARNING THIS IS NOT USUALLY RESET
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

        protected void ResetPilotButton_Click(object sender, EventArgs e)
        {
            AndromedaDataContext db = new AndromedaDataContext();

            if (!User.Identity.IsAuthenticated) return; //Avoid unauthenticated users
            if (!db.Players.Any(i => i.PlayerName == User.Identity.Name)) return; //There must be an old ship

            #region Log update
            bool alreadyLogged = false;
            PilotReset logEntry = new PilotReset()
            {
                Player = User.Identity.Name,
                Timestamp = DateTime.Now
            };
            #endregion

            Random rnd = new Random((int)DateTime.Now.Ticks);

            try
            {
                //Get player
                var player = db.Players.Single(i => i.PlayerName == User.Identity.Name);
                #region Log update
                logEntry.Guid = player.FirstShipGuid.Value;
                #endregion

                //Set player
                player.PlayerMoney = 5000;

                //Take old ships out of commission by taking it away from the player and assigning it a new GUID
                //Since all other resources reference the ship by ID (not GUID), all other resources will be decommissioned as well
                foreach (var oldSpaceship in player.Spaceships)
                {
                    oldSpaceship.PlayerGuid = Guid.NewGuid(); //Guid changed to make the ship un-controllable
                    oldSpaceship.Deleted = true; //This will hide the ship from the map
                }
                player.Spaceships.Clear();

                //Create the user's new spaceship with the old GUID.
                //The page will now reload and the UI will automatically update to display the Guid and
                //make the template available for download.
                db.Spaceships.InsertOnSubmit(
                    new Spaceship()
                    {
                        PlayerGuid = player.FirstShipGuid,
                        DriveCount = 0,
                        SensorCount = 0,
                        CurrentStarId = rnd.Next(0, db.Stars.Count()),
                        Deleted = false,
                        ModificationCount = 0,
                        ShipModel = 0,
                        CannonCount = 0,
                        ShieldCount = 0,
                        LastRaided = TimeGetter.GetLocalTime(),
                        Created = TimeGetter.GetLocalTime(),
                        TransponderCode = Guid.NewGuid(),
                        Player = player,
                    });
                db.SubmitChanges();

                Response.Redirect("Map.aspx"); //Make the browser throw away the submitted POST so that a F5 does not repeat the Reset Pilot command
            }
            catch (Exception ex)
            {
                #region Log update
                logEntry.Error = ex.ToString();
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

        public string GetPlayerGuid()
        {
            if (User.Identity.IsAuthenticated)
            {
                AndromedaDataContext db = new AndromedaDataContext();

                if (db.Players.Any(i => i.PlayerName == User.Identity.Name))
                {
                    var player = db.Players.Single(i => i.PlayerName == User.Identity.Name);
                    return player.FirstShipGuid.Value.ToString();
                }
                else
                {
                    return "NONE";
                }
            }
            else
            {
                return "NONE";
            }
        }
    }
}