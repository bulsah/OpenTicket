using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX.yonetici
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void giris_Click(object sender, EventArgs e)
        {
            vt v = new vt();
           
            var s = new SqlCommand(
              "select * from kullanicilar where kullaniciadi=@kadi and kullanicisifre=@ksif and yonetici=1");
            s.Parameters.Add("@kadi", kullaniciadi.Text);
            s.Parameters.Add("@ksif",v.Crypt( sifre.Text));
            var oku = v.Select2(s);
            oku.Read();
            if (oku.HasRows)
            {
                Page.Response.Redirect("default.aspx");
            }
            else
            {
                uyari.InnerHtml = "<div class='alert alert-danger mt-2'>Kullanici adı yada şifre hatalı code="+v.Crypt(sifre.Text)+"</div>";
            }
        }
    }
}