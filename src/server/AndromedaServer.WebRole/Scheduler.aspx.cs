using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Andromeda.WebPages
{
    public partial class Scheduler : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //This page just serves as an event generator. It is called by a console application ran by Windows Task Scheduler.
            //The class below will evaluate whether enough time has passed since the last run.
            Andromeda.Scheduler.ScheduledTasks.IncreaseCommodityQuantities();
        }
    }
}