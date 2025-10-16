using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket.admin
{
    public partial class kullanici : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Handle query string actions
            if (Request.QueryString["user"] != null)
            {
                if (!int.TryParse(Request.QueryString["user"], out int userId))
                {
                    Page.Response.Redirect("users.aspx");
                    return;
                }

                HandleUserActions(userId);
            }

            LoadUsersTable();
        }

        private void HandleUserActions(int userId)
        {
            try
            {
                vt v = new vt();

                // Reset password action
                if (Request.QueryString["resetpw"] == "1")
                {
                    var getUserCmd = new SqlCommand("SELECT * FROM users WHERE id=@userid");
                    getUserCmd.Parameters.AddWithValue("@userid", userId);
                    
                    var reader = v.Select(getUserCmd);
                    reader.Read();
                    
                    if (reader.HasRows)
                    {
                        string userEmail = reader["email"].ToString();
                        reader.Close();

                        // Generate secure random password
                        string newPassword = v.GenerateRandomPassword(12);
                        string hashedPassword = PasswordHasher.HashPassword(newPassword);

                        // Update password - parameterized
                        var updateCmd = new SqlCommand(
                            "UPDATE users SET passwordhash=@hash WHERE id=@userid");
                        updateCmd.Parameters.AddWithValue("@hash", hashedPassword);
                        updateCmd.Parameters.AddWithValue("@userid", userId);
                        v.InsertUpdateDelete(updateCmd);

                        // Send email
                        v.SendEmail(userEmail, "Password Reset", 
                            $"Your new password is: <strong>{newPassword}</strong><br>Please change it after login.");
                        
                        v.LogOperation($"Password reset for user ID: {userId}");
                    }
                }

                // Toggle active status
                if (Request.QueryString["status"] != null)
                {
                    int status = Request.QueryString["status"] == "1" ? 1 : 0;
                    var cmd = new SqlCommand("UPDATE users SET active=@status WHERE id=@userid");
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    v.InsertUpdateDelete(cmd);
                }

                // Toggle admin status
                if (Request.QueryString["admin"] != null)
                {
                    int adminStatus = Request.QueryString["admin"] == "1" ? 1 : 0;
                    var cmd = new SqlCommand("UPDATE users SET isadmin=@admin WHERE id=@userid");
                    cmd.Parameters.AddWithValue("@admin", adminStatus);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    v.InsertUpdateDelete(cmd);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"HandleUserActions error: {ex.Message}");
            }
        }

        private void LoadUsersTable()
        {
            try
            {
                var html = "<table class='table table-striped'>";
                html += @"<thead><tr>
                    <th>ID</th><th>Email</th><th>Name</th><th>Registration Date</th>
                    <th>Active</th><th>Admin</th><th>Actions</th>
                </tr></thead><tbody>";

                vt v = new vt();
                var cmd = new SqlCommand("SELECT * FROM users ORDER BY id DESC");
                var reader = v.Select(cmd);

                while (reader.Read())
                {
                    string userId = reader["id"].ToString();
                    html += "<tr>";
                    html += $"<td>{HttpUtility.HtmlEncode(userId)}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["email"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["username"].ToString())}</td>";
                    html += $"<td>{HttpUtility.HtmlEncode(reader["registrationdate"].ToString())}</td>";
                    html += $"<td>{GetActiveButton(reader["active"].ToString(), userId)}</td>";
                    html += $"<td>{GetAdminButton(reader["isadmin"].ToString(), userId)}</td>";
                    html += $"<td><a class='btn btn-sm btn-warning' href='?user={userId}&resetpw=1'>Reset Password</a></td>";
                    html += "</tr>";
                }

                reader.Close();
                html += "</tbody></table>";
                table.InnerHtml = html;
            }
            catch (Exception ex)
            {
                table.InnerHtml = $"<div class='alert alert-danger'>Error loading users: {HttpUtility.HtmlEncode(ex.Message)}</div>";
                System.Diagnostics.Debug.WriteLine($"LoadUsersTable error: {ex.Message}");
            }
        }

        private string GetActiveButton(string isActive, string userId)
        {
            bool active = isActive.ToLower() == "true";
            if (active)
                return $"<a class='btn btn-sm btn-success' href='?user={userId}&status=0'>Active</a>";
            else
                return $"<a class='btn btn-sm btn-danger' href='?user={userId}&status=1'>Inactive</a>";
        }

        private string GetAdminButton(string isAdmin, string userId)
        {
            bool admin = isAdmin.ToLower() == "true";
            if (admin)
                return $"<a class='btn btn-sm btn-info' href='?user={userId}&admin=0'>Admin</a>";
            else
                return $"<a class='btn btn-sm btn-secondary' href='?user={userId}&admin=1'>User</a>";
        }

        protected void Unnamed1_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(kullaniciadi.Text) || 
                    string.IsNullOrWhiteSpace(kullanicimail.Text) || 
                    string.IsNullOrWhiteSpace(kullanicisifre.Text))
                {
                    hata.InnerText = "All fields are required.";
                    return;
                }

                // Validate email
                if (!InputValidationHelper.IsValidEmail(kullanicimail.Text))
                {
                    hata.InnerText = "Please enter a valid email address.";
                    return;
                }

                // Validate password strength
                if (!InputValidationHelper.IsStrongPassword(kullanicisifre.Text, out string message))
                {
                    hata.InnerText = message;
                    return;
                }

                vt v = new vt();

                // Hash password with PBKDF2
                string hashedPassword = PasswordHasher.HashPassword(kullanicisifre.Text);

                // Insert user - parameterized query
                var cmd = new SqlCommand(@"
                    INSERT INTO users (username, email, passwordhash, registrationdate, active, isadmin) 
                    VALUES (@name, @email, @hash, GETDATE(), 1, 0);
                    SELECT SCOPE_IDENTITY()");
                
                cmd.Parameters.AddWithValue("@name", HttpUtility.HtmlEncode(kullaniciadi.Text));
                cmd.Parameters.AddWithValue("@email", kullanicimail.Text);
                cmd.Parameters.AddWithValue("@hash", hashedPassword);

                var newId = v.ExecuteScalar(cmd);

                // Log operation
                v.LogOperation($"New user created: {kullaniciadi.Text} (ID: {newId})");

                // Redirect to refresh
                Page.Response.Redirect("users.aspx");
            }
            catch (Exception ex)
            {
                hata.InnerText = $"Error creating user: {HttpUtility.HtmlEncode(ex.Message)}";
                System.Diagnostics.Debug.WriteLine($"Create user error: {ex.Message}");
            }
        }
    }
}
