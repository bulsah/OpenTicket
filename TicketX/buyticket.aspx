<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="buyticket.aspx.cs" Inherits="OpenTicket.buyticket" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Buy Ticket - OpenTicket</title>
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link href="https://fonts.googleapis.com/css2?family=Comfortaa:wght@700&display=swap" rel="stylesheet"> 
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link href="https://fonts.googleapis.com/css2?family=Caveat:wght@600&family=Comfortaa:wght@700&display=swap" rel="stylesheet">
    <link href="fe/fa/css/all.css" rel="stylesheet" />
    <link href="fe/css/st.css" rel="stylesheet" />
    <link href="fe/css/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div id="notification" runat="server"></div>

        <div class="container">
            <div class="row">
                <div class="col-12">
                    <asp:Image ID="Image1" runat="server" />
                </div>
            </div>
     
            <div class="row" id="purchasePanel" runat="server">
                <div class="col-6">
                    <div class="card">
                        <div class="card-body">
                            <label class="mt-1">Full Name</label>
                            <asp:TextBox ID="fullname" runat="server" CssClass="form-control" required></asp:TextBox>
                            
                            <label class="mt-1">Email</label>
                            <asp:TextBox ID="email" runat="server" CssClass="form-control" TextMode="Email" required></asp:TextBox>
                   
                            <label class="mt-1">Event Name</label>
                            <asp:TextBox ID="eventName" runat="server" CssClass="form-control" disabled></asp:TextBox>
                  
                            <label class="mt-1">Price</label>
                            <asp:TextBox ID="price" ClientIDMode="Static" runat="server" CssClass="form-control" disabled></asp:TextBox>
                            
                            <label class="mt-1">Quantity</label>
                            <asp:TextBox ID="quantity" ClientIDMode="Static" runat="server" CssClass="form-control" TextMode="Number" required></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="col-6">
                    <div class="card">
                        <div class="card-body">
                            <label class="mt-1">Cardholder Name</label>
                            <input type="text" class="form-control" required/>
                  
                            <label class="mt-1">Card Number</label>
                            <input type="text" class="form-control" maxlength="16" required/>
                    
                            <label class="mt-1">Expiration Date</label>
                            <input type="month" class="form-control" required/>
                            
                            <label class="mt-1">Security Code (CVV)</label>
                            <input type="text" class="form-control" maxlength="3" required/>
                            
                            <label class="mt-1">Total Amount</label>
                            <asp:TextBox ID="totalPrice" ClientIDMode="Static" runat="server" CssClass="form-control" disabled></asp:TextBox>
                     
                            <asp:Button ID="PurchaseButton" CssClass="btn btn-info mt-2 col-12" runat="server" Text="Purchase" OnClick="PurchaseButton_Click" />
                        </div>
                    </div>
                </div>

                <asp:HiddenField ID="eventId_hidden" runat="server" />
            </div>
        </div>
    </form>
    
    <script src="fe/js/jquery.js"></script>
    <script src="fe/js/bootstrap.js"></script>
    <script type="text/javascript">
        $("#quantity").on("change", function () {
            var price = $("#price").val();
            var quantity = $("#quantity").val();
            var total = parseFloat(quantity) * parseFloat(price);
            $("#totalPrice").val(total);
        });
    </script>
</body>
</html>
