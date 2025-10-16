using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX.yonetici
{
    public partial class etkinlik : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            vt v = new vt();
            v.dropdownveri(etkinlikalani, "select id, etkinlikalanadi from etkinlikalani");
            v.dropdownveri(fiyatlis, "select id,fiyat from fiyatlar where aktif=1");
            tablogetir();
        }

        protected void kaydet_Click(object sender, EventArgs e)
        {
            if (afisekle.HasFile)
                try
                {
                    if (afisekle.PostedFile.ContentType == "image/jpeg")
                    {
                        if (afisekle.PostedFile.ContentLength < 99999999999999)
                        {
   vt v = new vt();
                            var dosyaadi = v.Crypt(DateTime.Now.ToString("O") + "bulsah");
                            afisekle.SaveAs(Server.MapPath("../afisler/") + dosyaadi+".jpeg");
                            hata.InnerText = "<div class='alert alert-info'>Kayıt Yapıldı</div>";

                         
                            var s = new SqlCommand(
                            "insert into etkinlikler (etkinlikadi, etkinlikaciklama, etkinlikalani, tarih," +
                            " saat, kayittarihi, etkinlikkapasitesi, etkinlikafis"+
                            ",fiyat) values (@a,@b,@c,@d,@f, GetDate(),@g,@h,@j) select SCOPE_IDENTITY() ");
                            s.Parameters.Add("@a", etkinlikadi.Text);
                            s.Parameters.Add("@b", detay.Text);
                            s.Parameters.Add("@c",etkinlikalani.SelectedValue );
                            s.Parameters.Add("@j",fiyatlis.SelectedValue );
                            s.Parameters.Add("@d",tarih.Text );
                            s.Parameters.Add("@f",saat.Text );
                            s.Parameters.Add("@g",kapasite.Text );
                            s.Parameters.Add("@h",dosyaadi+".jpeg" );
                            var oku = v.Select2(s);
                            oku.Read();



                        }
                        else
                        {
                            hata.InnerText = "Maksimum boyutu aştınız";
                        }
                    }
                    else
                    {
                        hata.InnerText = "Resim dosyası seçin.";
                    }


                   


                }
                catch (Exception ex)
                {
                    hata.InnerText = "Hata Oluştu: " + ex.Message.ToString();
                }
            else
            {
                hata.InnerText = "Dosya Seçin ve GÖNDER Butonuna Tıklayın.";
            }
        }

        public void tablogetir()

        {

            var t = "<table class='table'>";
            vt v = new vt();

            //// etkinlikadi, etkinlikaciklama, etkinlikalani, tarih, saat, kayittarihi, etkinlikkapasitesi, etkinlikafis

            t += "<thead><tr><td>Id</td>" +
                "<td>Etkinlik Adı</td>" +
                "<td>Açıklama</td>" +
                "<td>Alan</td>" +
                "<td>Tarih</td>" +
                "<td>Saat</td>" +
                "<td>Kayıt Tarihi</td>" +
                "<td>Kapasite</td>" +
                "<td>Afiş</td>" +
                "<td>Fiyat</td>" +
                "<td>Aktif</td>" +
                "</tr></thead>";
            var oku = v.Select("select * from etkinliklistesi");
            oku.Read();
            if (oku.HasRows)
            {
                t += "<tbody><tr>";
                t += "<td>" + oku[0].ToString() + "</td>";
                t += "<td>" + oku[1].ToString() + "</td>";
                t += "<td>" + oku[2].ToString() + "</td>";

                t += "<td>" + oku[3].ToString() + "</td>";
                t += "<td>" + oku[4].ToString().Replace("00:00:00","") + "</td>";
                t += "<td>" + oku[5].ToString() + "</td>";
                t += "<td>" + oku[6].ToString() + "</td>";
                t += "<td>" + oku[7].ToString() +"\\"+oran(oku[0].ToString())+ "</td>";
                t += "<td><img src='../afisler/" + oku[8].ToString() + "' class='minifoto'></img></td>";
                t += "<td>" + oku[9].ToString() + "</td>";
               
                t += "<td>" + aktifmi(oku[10].ToString(), oku[0].ToString()) + "</td>";



                t += "</tr>";

                while (oku.Read())
                {
                    t += "<tr>";
                    t += "<td>" + oku[0].ToString() + "</td>";
                    t += "<td>" + oku[1].ToString() + "</td>";
                    t += "<td>" + oku[2].ToString() + "</td>";

                    t += "<td>" + oku[3].ToString() + "</td>";
                    t += "<td>" + oku[4].ToString().Replace("00:00:00", "") + "</td>";
                    t += "<td>" + oku[5].ToString() + "</td>";
                    t += "<td>" + oku[6].ToString() + "</td>";
                    t += "<td>" + oku[7].ToString() + "\\" + oran(oku[0].ToString()) + "</td>";
                    t += "<td><img src='../afisler/" + oku[8].ToString() + "' class='minifoto'></img></td>";
                    t += "<td>" + oku[9].ToString() + "</td>";
                    t += "<td>" +aktifmi(oku[10].ToString(), oku[0].ToString()) + "</td>";
                    t += "</tr>";

                }
                t += "</tbody></table>";

            }
            else
            {
                t += "<tbody><tr><td colspan='4'></td></tr></tbody></table>";
            }
            tablo.InnerHtml = t;



        }


        public static string oran(string etkinlikid)
        {
            var t = "";
            vt v = new vt();
         var oku=   v.Select("select count(*) from biletalanlar where aktif=1 and etkinlikid=" + etkinlikid);
            oku.Read();
            if (oku.HasRows)
            {
                t = oku[0].ToString();
            }

            return t;
        }

        public static string aktifmi(string a, string etk)
        {
            var c = "asdadsasdads";
            if (a.ToLower() == "true")
            {
                c = "<a class='btn btn-sm btn-info' href='?etkinlik=" + etk + "&statu=0'>Aktif</a>";
            }
            else
            {
                c = "<a class='btn btn-sm btn-danger' href='?etkinlik=" + etk + "&statu=1'>Pasif</a>";
            }


            return c;
        }
    }
}