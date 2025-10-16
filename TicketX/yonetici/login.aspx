<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="OpenTicket.admin.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Admin Login - OpenTicket</title>
    <link href="../fe/css/bootstrap.css" rel="stylesheet" />
    <link href="../fe/css/st.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col d-flex justify-content-center">
                    <div class="card">
                        <div class="card-body">
                            <h3 class="text-center mb-3">Admin Login</h3>
                            
                            <label for="username">Username</label>
                            <asp:TextBox ID="username" runat="server" CssClass="form-control" required></asp:TextBox>
            
                            <label for="password" class="mt-2">Password</label>
                            <asp:TextBox ID="password" runat="server" CssClass="form-control" TextMode="Password" required></asp:TextBox>
                            
                            <asp:Button CssClass="btn btn-info mt-3 w-100" ID="LoginButton" runat="server" Text="Login" OnClick="LoginButton_Click"/>
                            
                            <div id="notification" runat="server" class="mt-2"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    <script src="../fe/js/jquery.js"></script>
    <script src="../fe/js/bootstrap.js"></script>
</body>
</html>
