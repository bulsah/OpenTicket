using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            myVideo.Visible = false;
            slider.Visible = false;
            detayla.Visible = false;
            if (Request.QueryString.Count>0)
            {
                if (Request.QueryString["filtre"]!=null)
                {
                    slider.Visible = true;
                    slider.InnerHtml = getir(" and etkinlikalani="+Request.QueryString["filtre"]);

                }



                if (Request.QueryString["detay"]!=null)
                { myVideo.Visible = true;
                    detayla.Visible = true;
                    detayla.InnerHtml = detaygetir(Request.QueryString["detay"]);
                    var a = Request.QueryString["detay"];
                    int k = 0;
                    try
                    {
                       k= Convert.ToInt32(a);
                    }
                    catch (Exception)
                    {
                        Page.Response.Redirect("hata.aspx");
                        throw;
                    }


                }
            }
            else
            {
                slider.Visible = true;
                slider.InnerHtml = getir();

               
                myVideo.Visible = false;
                detayla.Visible = false;
            }
        }
  

    public static string getir(string filtre="")
    {
            var k = "";

            vt v = new vt();
          var oku=  v.Select("select * from etkinlikler where aktif=1 "+filtre);
            oku.Read();
            if (oku.HasRows)
            {
 k += "<div class=' col-lg-4 col-sm-12'><div class='card'>";
   
        k += "<img class='card-img-top' src='afisler/"+oku["etkinlikafis"].ToString()+"' alt='Card image cap'>";
        k += "<div class='card-body'>";
        k += "<p class='card-text text-center'>"+oku["etkinlikadi"].ToString()+ " <br>" + oku["tarih"].ToString().Replace("00:00:00", "") + "<br> " + oku["saat"].ToString()+"</p>";
        k += "<div class='text-center'>";
        k += "<a href='default.aspx?detay="+oku["id"].ToString()+"' class='btn btn-danger'>Detayla</a>";
     
        k += "</div>";
      
        k += "</div>";
        k += "</div></div>";
                while (oku.Read())
                {
                    k += "<div class=' col-lg-4 col-sm-12'><div class='card'>";
                    k += "<img class='card-img-top' src='afisler/" + oku["etkinlikafis"].ToString() + "' alt='Card image cap'>";
                    k += "<div class='card-body'>";
                    k += "<p class='card-text text-center'>" + oku["etkinlikadi"].ToString() + "<br> " + oku["tarih"].ToString().Replace("00:00:00","") + "<br> " + oku["saat"].ToString() + "</p>";
                    k += "<div class='text-center'>";
                    k += "<a href='default.aspx?detay=" + oku["id"].ToString() + "' class='btn btn-danger'>Detayla</a>";

                    k += "</div>";

                    k += "</div>";
                    k += "</div></div>";
                }
            }

           


        return k;
    }

        public static string detaygetir(string filtre)
        {

            var k = "";

            vt v = new vt();
            var oku = v.Select("select * from etkinliklistesi where aktif=1 and id=" + filtre);
            oku.Read();
            if (oku.HasRows)
            {
                k += "<div id='detayla' runat='server' class='container'>";
               
                k += "<div class='row'>";
                k += "<div class='col-lg-8 col-sm-12 col-md-8 mt-2'>";
                k += "<div class='card-m text-center'>";
           
                k += "<img class='afis' src='afisler/"+oku["etkinlikafis"].ToString()+"' />";
             
                k += "</div>";
               
                k += "</div>";
                k += "<div class='col-lg-4 col-sm-12 col-md-4 mt-2'>";
                k += "<div class='card-m'>";
                k += "<div class='card-body-m'>";
                k += "<div class='konserbaslik'>";
              
                k += oku["etkinlikadi"].ToString();
              
                k += "</div>  ";
                k += "<div class='konserdetay'>";
                k += "<p class='text-center text-capitalize' > "+ oku["etkinlikaciklama"].ToString() + "</p>";
                k += "<p class='text-left'>";
                k += "<i class='fa fa-location mr-2'></i>    "+ oku["etkinlikalanadi"].ToString() ;
            
                k += "</p>";
                k += "<p class='text-left'>";
                k += "<i class='fa fa-calendar mr-2' ></i> "+ oku["tarih"].ToString().Replace("00:00:00", "") + " "+ oku["saat"].ToString();
               
                k += "</p>";
                k += "<p class='text-left'>";
                k += "<i class='fa fa-users mr-2' ></i>"+toplamsatis(oku["id"].ToString()) +"/"+ oku["etkinlikkapasitesi"].ToString() ;
            
                k += "</p>";
               
                k += "<p class='text-left'>";
              
                k += "<i class='fa fa-money-bill mr-2' ></i>"+ oku["Expr1"].ToString() + "TL";
                
                k += "</p>";
                k += "<div class='text-center'>";
                k += "<a href='biletal.aspx?etkinlikid="+oku["id"].ToString()+"' class='btn btn-danger'><i class='fa fa-cart-plus'></i> Satın Al</a>";
         
                k += "</div>";
          
                k += "</div>";
            
                k += "</div>";
           
                k += "</div>";
          
                k += "</div>";
              
                k += "</div>";
           
                k += "</div>";

             
            }



            return k;
        }
        public static string toplamsatis(string etkinlikid)
        {
            var k = "0";
            vt v = new vt();
         var oku=   v.Select("select * from biletraporu where etkinlikid="+etkinlikid);
            oku.Read();
            if (oku.HasRows)
            {
                k = oku["kisisayisi"].ToString();
            }
            return k;
        }


}  }