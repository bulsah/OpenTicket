<%@ Page Title="" Language="C#" MasterPageFile="~/yonetici/YONETİCİ.Master" AutoEventWireup="true" CodeBehind="etkinlik.aspx.cs" Inherits="TicketX.yonetici.etkinlik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col-3">
            <div class="card">
                <div class="card-body">
                    <label>Etkinlik Adı</label>
                    <asp:TextBox runat="server" id="etkinlikadi" class="form-control"></asp:TextBox>
                    <label>Etkinlik Detay</label>
                      <asp:TextBox runat="server" id="detay" class="form-control" textmode="multiline"></asp:TextBox>
            <label>Etkinlik Alanı</label>
                    
                    <asp:DropDownList runat="server" class="form-control" id="etkinlikalani"></asp:DropDownList>
                    
                    
                    
                    
                    <label>Tarih</label>
                      <asp:TextBox runat="server" id="tarih" class="form-control" textmode="date"></asp:TextBox>
          <label>Saat</label>
                      <asp:TextBox runat="server" id="saat" class="form-control" textmode="time"></asp:TextBox>
               <label>Kapasite</label>
                      <asp:TextBox runat="server" id="kapasite" class="form-control" textmode="number"></asp:TextBox>
                  <label>Fiyat</label>
                     <asp:DropDownList runat="server" class="form-control" id="fiyatlis"></asp:DropDownList>
                
               
                <label>Afis</label>
                <asp:FileUpload runat="server" class="form-control" id="afisekle"></asp:FileUpload>
 </div>

                <asp:Button runat="server" Text="Kaydet" class="btn btn-info" id="kaydet" OnClick="kaydet_Click" />
                <div id="hata" runat="server">



                </div>
            </div>


        </div>

        <div class="col-9">
            <div class="card">
                <div class="card-body">

                    <div id="tablo" runat="server">


                    </div>

                </div>


            </div>


        </div>
    </div>


</asp:Content>
