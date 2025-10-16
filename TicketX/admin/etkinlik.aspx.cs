using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket.admin
{
    public partial class etkinlik : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDropdowns();
                LoadEventsTable();
            }
        }

        private void LoadDropdowns()
        {
            try
            {
                vt v = new vt();
                
                // Load event areas dropdown
                var cmd1 = new SqlCommand("SELECT id, areaname FROM eventareas ORDER BY areaname");
                v.FillDropdown(etkinlikalani, cmd1, "areaname", "id");
                
                // Load price list dropdown
                var cmd2 = new SqlCommand("SELECT id, price FROM pricelist WHERE active=1 ORDER BY price");
                v.FillDropdown(fiyatlis, cmd2, "price", "id");
            }
            catch (Exception ex)
            {
                hata.InnerText = $"Error loading dropdowns: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"LoadDropdowns error: {ex.Message}");
            }
        }

        protected void kaydet_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(etkinlikadi.Text))
                {
                    hata.InnerText = "Event name is required.";
                    return;
                }

                if (!afisekle.HasFile)
                {
                    hata.InnerText = "Please select a poster image.";
                    return;
                }

                // Validate file type
                if (afisekle.PostedFile.ContentType != "image/jpeg" && 
                    afisekle.PostedFile.ContentType != "image/jpg")
                {
                    hata.InnerText = "Please select a JPEG image file.";
                    return;
                }

                // Validate file size (10 MB max)
                if (afisekle.PostedFile.ContentLength > 10485760)
                {
                    hata.InnerText = "Maximum file size exceeded (10 MB max).";
                    return;
                }

                vt v = new vt();
                
                // Generate secure filename
                string secureFileName = PasswordHasher.HashPassword(DateTime.Now.ToString("O") + etkinlikadi.Text);
                string shortFileName = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(secureFileName)
                ).Substring(0, 40).Replace("/", "_").Replace("+", "-") + ".jpeg";

                // Save file
                string savePath = Server.MapPath("../posters/") + shortFileName;
                afisekle.SaveAs(savePath);

                // Insert event - parameterized query
                var cmd = new SqlCommand(@"
                    INSERT INTO events (eventname, eventdescription, eventarea, date, time, 
                                       registrationdate, eventcapacity, eventposter, price, active) 
                    VALUES (@name, @description, @area, @date, @time, GETDATE(), @capacity, @poster, @price, 1);
                    SELECT SCOPE_IDENTITY()");
                
                cmd.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(etkinlikadi.Text));
                cmd.Parameters.AddWithValue("@description", HttpUtility.HtmlEncode(detay.Text));
                cmd.Parameters.AddWithValue("@area", etkinlikalani.SelectedValue);
                cmd.Parameters.AddWithValue("@price", fiyatlis.SelectedValue);
                cmd.Parameters.AddWithValue("@date", tarih.Text);
                cmd.Parameters.AddWithValue("@time", saat.Text);
                cmd.Parameters.AddWithValue("@capacity", kapasite.Text);
                cmd.Parameters.AddWithValue("@poster", shortFileName);

                var newId = v.ExecuteScalar(cmd);

                hata.InnerText = $"Event created successfully! ID: {newId}";
                
                // Reload table
                LoadEventsTable();
                
                // Clear form
                ClearForm();
            }
            catch (Exception ex)
            {
                hata.InnerText = $"Error: {HttpUtility.HtmlEncode(ex.Message)}";
                System.Diagnostics.Debug.WriteLine($"Save event error: {ex.Message}");
            }
        }

        private void LoadEventsTable()
        {
            try
            {
                var html = "<table class='table table-striped'>";
                html += @"<thead><tr>
                    <th>ID</th><th>Event Name</th><th>Description</th><th>Area</th>
                    <th>Date</th><th>Time</th><th>Registration</th><th>Capacity</th>
                    <th>Poster</th><th>Price</th><th>Status</th>
                </tr></thead><tbody>";

                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM eventlist ORDER BY id DESC");
                var reader = v.Select(cmd);

                while (reader.Read())
                {
                    string eventId = reader["id"].ToString();
                    html += "<tr>";
                    html += $"<td>{HttpUtility.HtmlEncode(eventId)}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["eventname"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["eventdescription"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["eventareaname"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["date"].ToString().Replace("00:00:00", ""))}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["time"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["registrationdate"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["eventcapacity"].ToString())}\\{GetSalesCount(eventId)}</td>";
                    html += $"<td><img src='../posters/{HttpUtility.HtmlEncode(reader["eventposter"].ToString())}' class='minifoto'/></td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["Expr1"].ToString())}</td>";
                    html += $"<td>{GetStatusButton(reader["active"].ToString(), eventId)}</td>";
                    html += "</tr>";
                }

                reader.Close();
                html += "</tbody></table>";
                tablo.InnerHtml = html;
            }
            catch (Exception ex)
            {
                tablo.InnerHtml = $"<div class='alert alert-danger'>Error loading events: {HttpUtility.HtmlEncode(ex.Message)}</div>";
                System.Diagnostics.Debug.WriteLine($"LoadEventsTable error: {ex.Message}");
            }
        }

        private string GetSalesCount(string eventId)
        {
            try
            {
                if (!int.TryParse(eventId, out int validId))
                    return "0";

                vt v = new vt();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM ticketholders WHERE active=1 AND eventid=@eventid");
                cmd.Parameters.AddWithValue("@eventid", validId);
                
                var count = v.ExecuteScalar(cmd);
                return count?.ToString() ?? "0";
            }
            catch
            {
                return "0";
            }
        }

        private string GetStatusButton(string isActive, string eventId)
        {
            bool active = isActive.ToLower() == "true";
            if (active)
                return $"<a class='btn btn-sm btn-success' href='?event={eventId}&status=0'>Active</a>";
            else
                return $"<a class='btn btn-sm btn-danger' href='?event={eventId}&status=1'>Inactive</a>";
        }

        private void ClearForm()
        {
            etkinlikadi.Text = "";
            detay.Text = "";
            tarih.Text = "";
            saat.Text = "";
            kapasite.Text = "";
        }
    }
}
