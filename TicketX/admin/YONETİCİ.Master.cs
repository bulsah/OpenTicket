using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket.admin
{
    public partial class YONETİCİ : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in as admin
            if (Session["isadmin"] == null || !(bool)Session["isadmin"])
            {
                Response.Redirect("login.aspx");
            }
        }
    }
}
