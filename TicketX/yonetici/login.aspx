<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="TicketX.yonetici.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <link href="../fe/css/bootstrap.css" rel="stylesheet" />
    <link href="../fe/css/st.css" rel="stylesheet" />
    <script src="../fe/js/bootstrap.js"></script>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">



           
            <div class="col d-flex justify-content-center">

 <div class="card ">
                <div class="card-body">

<label for="kullaniciadi">Kullanıcı Adınız</label>
            <asp:TextBox ID="kullaniciadi" runat="server" CssClass="form-control"></asp:TextBox>
            
            <label for="sifre">Kullanıcı Şifresi</label>
            <asp:TextBox ID="sifre" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
            <asp:Button CssClass="btn btn-info mt-1" id="giris" runat="server" Text="Giriş Yap" OnClick="giris_Click"/>
                    <div id="uyari" runat="server"></div>
                </div>


            </div>
            
 </div>
            </div>
           

        </div>
    </form>
</body>
</html>
