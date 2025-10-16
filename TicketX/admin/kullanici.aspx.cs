using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TicketX.yonetici
{
    public partial class kullanici : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {



            vt v = new vt();

            if (Request.QueryString["kullanici"]!=null)
            {
                if (Request.QueryString["kullanici"]!=null)
                {
                    var k = Request.QueryString["kullanici"];
                if (Request.QueryString["sifre"]!=null)
                {
                    if (Request.QueryString["sifre"]=="1")
                    {
                          var okun=  v.Select("select * from kullanicilar where id=" + k);
                            okun.Read();
                            if (okun.HasRows)
                            {

                                var ys = v.randompass(8);
                                v.SendMail(okun["kullanicimail"].ToString(), "Yeni Şifre", "Yeni Şifreniz="+ys);
                                v.InsertUpdateDelete("update kullanicilar set kullanicisifre='"+v.Crypt(ys)+"' where id="+k);
                            }

                        }

                }

                    if (Request.QueryString["statu"]!=null)
                    {
                        v.InsertUpdateDelete("update kullanicilar set aktif=" + Request.QueryString["statu"] + " where id=" + k);
                    }
                    if (Request.QueryString["yonetici"]!=null)
                    {
                        v.InsertUpdateDelete("update kullanicilar set yonetici=" + Request.QueryString["yonetici"] + " where id=" + k);
                    }

                }

               
            }


            var t = "<table class='table'>";


            //////////////// id, kullanicimail, kullaniciadi, kullanicisifre, kayittarihi, aktif, yonetici
            t += "<thead><tr><td>Id</td><td>Mail</td><td>Adı</td><td>Kayıt Tarihi</td><td>Aktif</td><td>Yonetici?</td><td>ŞifreGönder</td></tr></thead>";
         var oku=   v.Select("select * from kullanicilar");
            oku.Read();
            if (oku.HasRows)
            {
                t += "<tbody><tr>";
                t += "<td>"+oku[0].ToString()+"</td>";
                t += "<td>"+oku[1].ToString()+"</td>";
                t += "<td>"+oku[2].ToString()+"</td>";
             
                t += "<td>"+oku[4].ToString()+"</td>";
                t += "<td>" + aktifmi(oku[5].ToString(), oku[0].ToString()) + "</td>";
                t += "<td>" + yoneticimi(oku[6].ToString(), oku[0].ToString()) + "</td>";
                t += "<td><a class='btn btn-sm btn-info' href='?kullanici=" + oku[0].ToString() + "&sifre=1'>Gonder</a></td>";


                t += "</tr>";

                while (oku.Read())
                {
                    t += "<tr>";
                    t += "<td>" + oku[0].ToString() + "</td>";
                    t += "<td>" + oku[1].ToString() + "</td>";
                    t += "<td>" + oku[2].ToString() + "</td>";

                    t += "<td>" + oku[4].ToString() + "</td>";
                    t += "<td>" + aktifmi(oku[5].ToString(), oku[0].ToString()) + "</td>";
                    t += "<td>" + yoneticimi(oku[6].ToString(),oku[0].ToString()) + "</td>";
                    t += "<td><a class='btn btn-sm btn-info' href='?kullanici=" + oku[0].ToString()+"&sifre=1'>Gonder</td>";
                 

                    t += "</tr>";

                }
                t += "</tbody></table>";

            }
            else
            {
                t += "<tbody><tr><td colspan='4'></td></tr></tbody></table>";
            }
            table.InnerHtml = t;
        
        }


        public static string aktifmi(string a , string user)
        {
            var c = "asdadsasdads";
            if (a.ToLower()== "true")
            {
                c="<a class='btn btn-sm btn-info' href='?kullanici="+user+"&statu=0'>Aktif</a>";
            }
            else
            {
                c = "<a class='btn btn-sm btn-danger' href='?kullanici=" + user + "&statu=1'>Pasif</a>";
            }


            return c;
        }

        public static string yoneticimi(string a, string user)
        {
            var c = "asdadsasdads";
            if (a.ToLower() == "true")
            {
                c = "<a class='btn btn-sm btn-info' href='?kullanici=" + user + "&yonetici=0'>Yonetici</a>";
            }
            else
            {
                c = "<a class='btn btn-sm btn-danger' href='?kullanici=" + user + "&yonetici=1'>Kullanıcı</a>";
            }


            return c;
        }

        protected void Unnamed1_Click(object sender, EventArgs e)
        {
            vt v = new vt();

            var s = new SqlCommand(
            "insert into kullanicilar (kullaniciadi,kullanicimail,kullanicisifre,kayittarihi,aktif,yonetici) values (@a,@b,@c,GetDate(),1,0) select SCOPE_IDENTITY() ");
            s.Parameters.Add("@a", kullaniciadi.Text);
            s.Parameters.Add("@b", kullanicimail.Text);
            s.Parameters.Add("@c", v.Crypt(kullanicisifre.Text));
            var oku = v.Select2(s);
            oku.Read();
            Page.Response.Redirect("kullanici.aspx");
        }
    }
}