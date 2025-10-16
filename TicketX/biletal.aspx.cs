using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX
{
    public partial class biletal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Page.Request.QueryString["islem"] != null)
            {
                if (Page.Request.QueryString["islem"] == "ok")
                {
                    uyari.InnerHtml = "<div class='alert alert-info'>Satın alma işlemi gerçekleştirilmiştir.</div>";
                    biletalpanel.Visible = false;

                }
            }
                if (Page.Request.QueryString["code"]!=null)
            {
                biletalpanel.Visible = false;
                vt v = new vt();
           var oku=     v.Select("select * from biletalanlar where qrcodehash='"+ Page.Request.QueryString["code"] + "'");
                oku.Read();

                if (oku.HasRows)
                {
                    v.qrcodecreate(Image1, Page.Request.QueryString["code"] + "bulsah");
                }
                
            };

            if (Page.Request.QueryString["etkinlikid"]!=null)
            {
                var a = Page.Request.QueryString["etkinlikid"];
                vt v = new vt();
              var oku=  v.Select("select * from etkinliklistesi where id="+a);
                oku.Read();
                if (oku.HasRows)
                {
   etadi.Text = oku["etkinlikadi"].ToString();
   fiyat.Text = oku["Expr1"].ToString();
                    etid.Value = oku["id"].ToString();
                }
             
            
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            vt v= new vt();
            for (int i = 0; i < Convert.ToInt32(sayi.Text); i++)
            {
                var c = v.Crypt(DateTime.Now.ToString());
 v.InsertUpdateDelete("insert into biletalanlar (kullaniciadi,etkinlikid,islemtarihi,fiyatid,qrcodehash,mail) values " +
                "('"+ad.Text+"',"+etid.Value+",GetDate(),"+fiyat.Text+",'"+c+"','"+mail.Text+"')");
                v.SendMail(mail.Text, "Bilet Alımı Gerçekleşti", "Sayın:" + ad.Text + " <br> Bilet alımınız gerçekleştirilmiştir.<a href='https://localhost:44334/biletal.aspx?code=" +c+"'>Buradan Code Erişebilirsiniz</a>");
            }
            Page.Response.Redirect("biletal.aspx?islem=ok");
           
        }
    }
}