using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Andromeda.Data;
using Andromeda.ServerEntities;

namespace Andromeda.WebPages
{
    public partial class RegisterComplete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/login.aspx?ReturnUrl=" + Request.Url.PathAndQuery);

            AndromedaDataContext db = new AndromedaDataContext();
            var player = db.Players.First(i => i.PlayerName == User.Identity.Name);
            PlayerGuidLiteral.Text = player.FirstShipGuid.Value.ToString();
        }
    }
}