using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket
{
    public partial class buyticket : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Request.QueryString["action"] != null)
            {
                if (Page.Request.QueryString["action"] == "success")
                {
                    notification.InnerHtml = "<div class='alert alert-info'>Purchase completed successfully.</div>";
                    purchasePanel.Visible = false;
                }
            }

            if (Page.Request.QueryString["code"] != null)
            {
                purchasePanel.Visible = false;
                
                // SQL Injection protection - parameterized query
                var code = Page.Request.QueryString["code"];
                if (string.IsNullOrWhiteSpace(code))
                {
                    Page.Response.Redirect("default.aspx");
                    return;
                }

                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM ticketholders WHERE qrcodehash=@code");
                cmd.Parameters.AddWithValue("@code", code);
                
                var reader = v.Select(cmd);
                reader.Read();

                if (reader.HasRows)
                {
                    v.GenerateQRCode(Image1, code + "openticket");
                }
                
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            if (Page.Request.QueryString["eventid"] != null)
            {
                var eventId = Page.Request.QueryString["eventid"];
                
                // Validate event ID
                if (!int.TryParse(eventId, out int validEventId))
                {
                    Page.Response.Redirect("default.aspx");
                    return;
                }

                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM eventlist WHERE id=@eventid");
                cmd.Parameters.AddWithValue("@eventid", validEventId);
                
                var reader = v.Select(cmd);
                reader.Read();
                
                if (reader.HasRows)
                {
                    eventName.Text = reader["eventname"].ToString();
                    price.Text = reader["Expr1"].ToString();
                    eventId_hidden.Value = reader["id"].ToString();
                }
                
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }
        }

        protected void PurchaseButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Input validation
                if (!int.TryParse(quantity.Text, out int ticketCount) || ticketCount <= 0)
                {
                    notification.InnerHtml = "<div class='alert alert-danger'>Please enter a valid quantity.</div>";
                    return;
                }

                if (!InputValidationHelper.IsValidEmail(email.Text))
                {
                    notification.InnerHtml = "<div class='alert alert-danger'>Please enter a valid email address.</div>";
                    return;
                }

                if (string.IsNullOrWhiteSpace(fullname.Text) || fullname.Text.Length < 3)
                {
                    notification.InnerHtml = "<div class='alert alert-danger'>Please enter a valid name.</div>";
                    return;
                }

                vt v = new vt();
                
                // Generate secure hash using PBKDF2 instead of SHA1
                string qrHash = PasswordHasher.HashPassword(DateTime.Now.ToString() + fullname.Text + email.Text);
                
                // Use short hash for QR code (first 32 characters)
                string shortHash = Convert.ToBase64String(
                    System.Text.Encoding.UTF8.GetBytes(qrHash)
                ).Substring(0, 32).Replace("/", "_").Replace("+", "-");

                for (int i = 0; i < ticketCount; i++)
                {
                    // Generate unique hash for each ticket
                    string ticketHash = PasswordHasher.HashPassword(shortHash + i.ToString());
                    string ticketShortHash = Convert.ToBase64String(
                        System.Text.Encoding.UTF8.GetBytes(ticketHash)
                    ).Substring(0, 32).Replace("/", "_").Replace("+", "-");

                    // Parameterized query to prevent SQL injection
                    var cmd = new SqlCommand(
                        @"INSERT INTO ticketholders (username, eventid, transactiondate, priceid, qrcodehash, email) 
                          VALUES (@username, @eventid, GETDATE(), @priceid, @qrcode, @email)");
                    
                    cmd.Parameters.AddWithValue("@username", HttpUtility.HtmlEncode(fullname.Text));
                    cmd.Parameters.AddWithValue("@eventid", int.Parse(eventId_hidden.Value));
                    cmd.Parameters.AddWithValue("@priceid", decimal.Parse(price.Text));
                    cmd.Parameters.AddWithValue("@qrcode", ticketShortHash);
                    cmd.Parameters.AddWithValue("@email", email.Text);

                    v.InsertUpdateDelete(cmd);

                    // Send email notification
                    string emailBody = $@"
                        <h2>Dear {HttpUtility.HtmlEncode(fullname.Text)},</h2>
                        <p>Your ticket purchase has been completed successfully.</p>
                        <p><a href='https://localhost:44334/buyticket.aspx?code={ticketShortHash}'>
                           Click here to view your ticket and QR code
                        </a></p>
                        <p>Thank you for choosing OpenTicket!</p>";

                    v.SendEmail(email.Text, "Ticket Purchase Confirmation", emailBody);
                }

                // Log the operation
                v.LogOperation($"Ticket purchase: {ticketCount} tickets for event ID {eventId_hidden.Value}");

                Page.Response.Redirect("buyticket.aspx?action=success");
            }
            catch (Exception ex)
            {
                notification.InnerHtml = $"<div class='alert alert-danger'>An error occurred: {HttpUtility.HtmlEncode(ex.Message)}</div>";
                
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Purchase error: {ex.Message}");
            }
        }
    }
}
