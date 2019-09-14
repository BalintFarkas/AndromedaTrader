using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AndromedaServer.WebSites
{
    public partial class AccountLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetRedirectQueryString()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                return string.Format("?ReturnUrl={0}", Request.QueryString["ReturnUrl"]);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}