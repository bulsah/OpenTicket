using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace OpenTicket.admin
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Clear any existing session on login page
            if (!IsPostBack)
            {
                Session.Clear();
            }
        }

        protected void giris_Click(object sender, EventArgs e)
        {
            try
            {
                // Input validation
                if (string.IsNullOrWhiteSpace(kullaniciadi.Text) || string.IsNullOrWhiteSpace(sifre.Text))
                {
                    uyari.InnerHtml = "<div class='alert alert-danger mt-2'>Please enter username and password.</div>";
                    return;
                }

                vt v = new vt();
                
                // First, get user with username (parameterized)
                var getUserCmd = new SqlCommand(
                    "SELECT * FROM users WHERE username=@username AND isadmin=1");
                getUserCmd.Parameters.AddWithValue("@username", kullaniciadi.Text);
                
                var reader = v.Select(getUserCmd);
                reader.Read();
                
                if (reader.HasRows)
                {
                    string storedPasswordHash = reader["passwordhash"].ToString();
                    int userId = Convert.ToInt32(reader["id"]);
                    string userName = reader["username"].ToString();
                    
                    reader.Close();
                    v.CloseConnection();
                    
                    // Check if it's old SHA1 hash or new PBKDF2
                    bool isValidPassword = false;
                    
                    if (storedPasswordHash.Contains("."))
                    {
                        // New PBKDF2 format
                        isValidPassword = PasswordHasher.VerifyPassword(sifre.Text, storedPasswordHash);
                    }
                    else
                    {
                        // Legacy SHA1 - verify and migrate
                        if (PasswordHasher.VerifyLegacySHA1(sifre.Text, storedPasswordHash))
                        {
                            isValidPassword = true;
                            
                            // Migrate to new hash
                            string newHash = PasswordHasher.HashPassword(sifre.Text);
                            var updateCmd = new SqlCommand(
                                "UPDATE users SET passwordhash=@newhash WHERE id=@userid");
                            updateCmd.Parameters.AddWithValue("@newhash", newHash);
                            updateCmd.Parameters.AddWithValue("@userid", userId);
                            
                            vt v2 = new vt();
                            v2.InsertUpdateDelete(updateCmd);
                        }
                    }
                    
                    if (isValidPassword)
                    {
                        // Set session variables
                        Session["userid"] = userId;
                        Session["username"] = userName;
                        Session["isadmin"] = true;
                        Session["logintime"] = DateTime.Now;
                        
                        // Log successful login
                        vt v3 = new vt();
                        v3.LogLoginAttempt(userName, true);
                        
                        // Redirect to admin panel
                        Page.Response.Redirect("default.aspx");
                    }
                    else
                    {
                        // Log failed login attempt
                        v.LogLoginAttempt(kullaniciadi.Text, false);
                        
                        uyari.InnerHtml = "<div class='alert alert-danger mt-2'>Invalid username or password.</div>";
                    }
                }
                else
                {
                    reader.Close();
                    v.CloseConnection();
                    
                    // Log failed login attempt
                    v.LogLoginAttempt(kullaniciadi.Text, false);
                    
                    uyari.InnerHtml = "<div class='alert alert-danger mt-2'>Invalid username or password.</div>";
                }
            }
            catch (Exception ex)
            {
                uyari.InnerHtml = $"<div class='alert alert-danger mt-2'>An error occurred. Please try again.</div>";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            }
        }
    }
}
