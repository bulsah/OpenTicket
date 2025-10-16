using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX.yonetici
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            vt v = new vt();
          var oku=  v.Select("select count(*) from kullanicilar where aktif=1");
            oku.Read();
            usercount.InnerHtml = oku[0].ToString();
            oku = v.Select("select count(*) from etkinlikler ");
            oku.Read();
            te.InnerHtml = oku[0].ToString(); 
            oku = v.Select("select count(*) from etkinlikler where aktif=1");
            oku.Read();
            ae.InnerHtml = oku[0].ToString();
        }
    }
}