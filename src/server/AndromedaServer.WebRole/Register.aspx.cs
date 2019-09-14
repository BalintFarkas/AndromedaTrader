using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Andromeda.Data;
using Andromeda.ServerEntities;
using Andromeda.Common;

namespace Andromeda.WebPages
{
    //This page is intended to provide a registration form and a rule review form in one. If the user already has a spaceship, the reg button is not displayed. 
    //If he logs in after the reg button is clicked, then no new ship is created, but he is thrown back to the Map page.
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                AndromedaDataContext db = new AndromedaDataContext();

                if (db.Players.Any(i => i.PlayerName == User.Identity.Name))
                {
                    RegisterPanel.Visible = false;
                }
                else
                {
                    RegisterPanel.Visible = true;
                }
            }
            else
            {
                RegisterPanel.Visible = true;
            }
        }

        protected void CreatePilotButton_Click(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/AccountLogin.aspx?ReturnUrl=" + Request.Url.PathAndQuery);

            AndromedaDataContext db = new AndromedaDataContext();

            if (db.Players.Any(i => i.PlayerName == User.Identity.Name)) Response.Redirect("Map.aspx"); //Avoid duplicates

            Random rnd = new Random((int)DateTime.Now.Ticks);

            //Create the player
            var player = new Player()
            {
                FirstShipGuid = Guid.NewGuid(),
                PlayerMoney = 5000,
                PlayerName = User.Identity.Name
            };
            db.Players.InsertOnSubmit(player);

            //Create the user's spaceship.
            //The page will now reload and the UI will automatically update to display the Guid and
            //make the template available for download.
            db.Spaceships.InsertOnSubmit(
                new Spaceship()
                {
                    CannonCount = 0,
                    Created = TimeGetter.GetLocalTime(),
                    CurrentStarId = rnd.Next(0, db.Stars.Count()),
                    Deleted = false,
                    DriveCount = 0,
                    LastRaided = TimeGetter.GetLocalTime(),
                    ModificationCount = 0,
                    Player = player,
                    PlayerGuid = player.FirstShipGuid,
                    SensorCount = 0,
                    ShieldCount = 0,
                    ShipModel = 0,
                    TransponderCode = Guid.NewGuid()
                });
            db.SubmitChanges();

            Response.Redirect("RegisterComplete.aspx");
        }
    }
}