using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket.admin
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                vt v = new vt();
                
                // Count active users - parameterized query
                var cmd1 = new SqlCommand("SELECT COUNT(*) FROM users WHERE active=1");
                var count1 = v.ExecuteScalar(cmd1);
                usercount.InnerHtml = count1?.ToString() ?? "0";
                
                // Count total events
                var cmd2 = new SqlCommand("SELECT COUNT(*) FROM events");
                var count2 = v.ExecuteScalar(cmd2);
                te.InnerHtml = count2?.ToString() ?? "0";
                
                // Count active events
                var cmd3 = new SqlCommand("SELECT COUNT(*) FROM events WHERE active=1");
                var count3 = v.ExecuteScalar(cmd3);
                ae.InnerHtml = count3?.ToString() ?? "0";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dashboard error: {ex.Message}");
            }
        }
    }
}
