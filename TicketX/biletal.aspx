<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="biletal.aspx.cs" Inherits="TicketX.biletal" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
       <link rel="preconnect" href="https://fonts.gstatic.com">
<link href="https://fonts.googleapis.com/css2?family=Comfortaa:wght@700&display=swap" rel="stylesheet"> 
    <link rel="preconnect" href="https://fonts.gstatic.com">
<link href="https://fonts.googleapis.com/css2?family=Caveat:wght@600&family=Comfortaa:wght@700&display=swap" rel="stylesheet"> <link href="fe/fa/css/all.css" rel="stylesheet" />
    <link href="fe/css/st.css" rel="stylesheet" />
    <link href="fe/css/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="uyari" runat="server">


        </div>

        <div class="container">

            <div class="row">
                <div class="col-12">
                    <asp:Image ID="Image1" runat="server" />

                </div>


            </div>
     

            <div class="row" id="biletalpanel" runat="server">




                <div class=" col-6">



                

               <div class="card">
                <div class="card-body">
                    <label  class="mt-1">Ad Soyad</label>
                    <asp:TextBox ID="ad" runat="server" CssClass="form-control" required></asp:TextBox>
                            <label  class="mt-1">Mail</label>
                    <asp:TextBox ID="mail" runat="server" CssClass="form-control" TextMode="Email" required></asp:TextBox>
                   
                  
                    <label class="mt-1">Etkinlik Adı</label>
             <asp:TextBox ID="etadi" runat="server" CssClass="form-control"  disabled></asp:TextBox>
                  
                    
                    <label class="mt-1">Fiyat</label>
                  <asp:TextBox ID="fiyat" ClientIDMode="Static" runat="server" CssClass="form-control"  disabled></asp:TextBox>
                    <label class="mt-1">Bilet Sayısı</label>
                  
                    <asp:TextBox ID="sayi" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="Number"  required></asp:TextBox>

                </div>


            </div>
</div>

<div class=" col-6">



            <div class="card">
                <div class="card-body">
                    <label  class="mt-1">Kart Üzerindeki İsim</label>
                    <input type="text" class="form-control"/>
                  
                    <label class="mt-1">Kart Bilgileri</label>
                    <input type="text" class="form-control"/>
                    
                    <label class="mt-1">Son Kullanma Tarihi</label>
                    <input type="date" class="form-control"/>
                    <label class="mt-1">Güvenlik Kodu</label>
                  
                    <input type="text" class="form-control"/>
<label class="mt-1">Toplam Tutar</label>
                        <asp:TextBox ID="totalfiyat" ClientIDMode="Static" runat="server" CssClass="form-control"  disabled></asp:TextBox>
                     
                    <asp:Button ID="Button1" CssClass="btn btn-info mt-2 col-12" runat="server" Text="Satın Al" OnClick="Button1_Click" />
                </div>

            </div>
</div>

                <asp:HiddenField ID="etid" runat="server" />


 </div>
       


    </form>
        <script src="fe/js/jquery.js"></script>
    <script src="fe/js/bootstrap.js"></script>
    <script type="text/javascript">
        $("#sayi").on("change", function () {


       
          var f=  $("#fiyat").val()
            var s = $("#sayi").val()
            var t = parseFloat(s) * parseFloat(f);
            $("#totalfiyat").val(t)
            console.log($(this).val())
            console.log(t)
            console.log(s)
            console.log(f)

        } );


    </script>

</body>
</html>
