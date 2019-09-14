using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace AndromedaServer.WebPages
{
    public partial class AccountRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CreateUserWizard1_CreatedUser(object sender, EventArgs e)
        {
            //Log in the freshly created user - no need for another round of typing
            FormsAuthentication.SetAuthCookie(CreateUserWizard1.UserName, false);

            //Throw the user back to wherever he came from - also saves him some clicking
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                Response.Redirect(Request.QueryString["ReturnUrl"]);
            }
            else
            {
                Response.Redirect("Default.aspx");
            }
        }
    }
}