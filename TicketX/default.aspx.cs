using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            myVideo.Visible = false;
            slider.Visible = false;
            eventDetails.Visible = false;
            
            if (Request.QueryString.Count > 0)
            {
                if (Request.QueryString["filter"] != null)
                {
                    // Validate filter parameter
                    if (!int.TryParse(Request.QueryString["filter"], out int filterValue))
                    {
                        Page.Response.Redirect("default.aspx");
                        return;
                    }

                    slider.Visible = true;
                    slider.InnerHtml = GetEvents(filterValue);
                }

                if (Request.QueryString["detail"] != null)
                {
                    // Validate detail parameter
                    if (!int.TryParse(Request.QueryString["detail"], out int detailId))
                    {
                        Page.Response.Redirect("error.aspx");
                        return;
                    }

                    myVideo.Visible = true;
                    eventDetails.Visible = true;
                    eventDetails.InnerHtml = GetEventDetail(detailId);
                }
            }
            else
            {
                slider.Visible = true;
                slider.InnerHtml = GetEvents();
                myVideo.Visible = false;
                eventDetails.Visible = false;
            }
        }
  
        public static string GetEvents(int? filterValue = null)
        {
            var html = "";

            try
            {
                vt v = new vt();
                SqlCommand cmd;
                
                if (filterValue.HasValue)
                {
                    cmd = new SqlCommand("SELECT * FROM events WHERE active=1 AND eventarea=@filter");
                    cmd.Parameters.AddWithValue("@filter", filterValue.Value);
                }
                else
                {
                    cmd = new SqlCommand("SELECT * FROM events WHERE active=1");
                }

                var reader = v.Select(cmd);
                reader.Read();
                
                if (reader.HasRows)
                {
                    html += BuildEventCard(reader);
                    
                    while (reader.Read())
                    {
                        html += BuildEventCard(reader);
                    }
                }
                
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEvents error: {ex.Message}");
                html = "<div class='alert alert-danger'>Error loading events.</div>";
            }

            return html;
        }

        private static string BuildEventCard(System.Data.SqlClient.SqlDataReader reader)
        {
            var html = "<div class='col-lg-4 col-sm-12'><div class='card'>";
            html += $"<img class='card-img-top' src='posters/{HttpUtility.HtmlEncode(reader["eventposter"].ToString())}' alt='Event poster'>";
            html += "<div class='card-body'>";
            html += $"<p class='card-text text-center'>{HttpUtility.HtmlEncode(reader["eventname"].ToString())}<br>";
            html += $"{HttpUtility.HtmlEncode(reader["date"].ToString().Replace("00:00:00", ""))}<br>";
            html += $"{HttpUtility.HtmlEncode(reader["time"].ToString())}</p>";
            html += "<div class='text-center'>";
            html += $"<a href='default.aspx?detail={HttpUtility.HtmlEncode(reader["id"].ToString())}' class='btn btn-danger'>View Details</a>";
            html += "</div></div></div></div>";
            
            return html;
        }

        public static string GetEventDetail(int eventId)
        {
            var html = "";

            try
            {
                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM eventlist WHERE active=1 AND id=@eventid");
                cmd.Parameters.AddWithValue("@eventid", eventId);
                
                var reader = v.Select(cmd);
                reader.Read();
                
                if (reader.HasRows)
                {
                    html += "<div id='eventDetails' runat='server' class='container'>";
                    html += "<div class='row'>";
                    html += "<div class='col-lg-8 col-sm-12 col-md-8 mt-2'>";
                    html += "<div class='card-m text-center'>";
                    html += $"<img class='afis' src='posters/{HttpUtility.HtmlEncode(reader["eventposter"].ToString())}' />";
                    html += "</div></div>";
                    
                    html += "<div class='col-lg-4 col-sm-12 col-md-4 mt-2'>";
                    html += "<div class='card-m'><div class='card-body-m'>";
                    html += "<div class='konserbaslik'>";
                    html += HttpUtility.HtmlEncode(reader["eventname"].ToString());
                    html += "</div>";
                    
                    html += "<div class='konserdetay'>";
                    html += $"<p class='text-center text-capitalize'>{HttpUtility.HtmlEncode(reader["eventdescription"].ToString())}</p>";
                    html += $"<p class='text-left'><i class='fa fa-location mr-2'></i>{HttpUtility.HtmlEncode(reader["eventareaname"].ToString())}</p>";
                    html += $"<p class='text-left'><i class='fa fa-calendar mr-2'></i>{HttpUtility.HtmlEncode(reader["date"].ToString().Replace("00:00:00", ""))} {HttpUtility.HtmlEncode(reader["time"].ToString())}</p>";
                    html += $"<p class='text-left'><i class='fa fa-users mr-2'></i>{GetTotalSales(reader["id"].ToString())}/{HttpUtility.HtmlEncode(reader["eventcapacity"].ToString())}</p>";
                    html += $"<p class='text-left'><i class='fa fa-money-bill mr-2'></i>{HttpUtility.HtmlEncode(reader["Expr1"].ToString())} TL</p>";
                    html += "<div class='text-center'>";
                    html += $"<a href='buyticket.aspx?eventid={HttpUtility.HtmlEncode(reader["id"].ToString())}' class='btn btn-danger'><i class='fa fa-cart-plus'></i> Buy Ticket</a>";
                    html += "</div></div></div></div></div></div></div>";
                }
                
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetEventDetail error: {ex.Message}");
                html = "<div class='alert alert-danger'>Error loading event details.</div>";
            }

            return html;
        }

        public static string GetTotalSales(string eventId)
        {
            var count = "0";
            
            try
            {
                if (!int.TryParse(eventId, out int validEventId))
                    return "0";

                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM ticketreport WHERE eventid=@eventid");
                cmd.Parameters.AddWithValue("@eventid", validEventId);
                
                var reader = v.Select(cmd);
                reader.Read();
                
                if (reader.HasRows)
                {
                    count = reader["peoplecount"].ToString();
                }
                
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetTotalSales error: {ex.Message}");
            }

            return count;
        }
    }
}
