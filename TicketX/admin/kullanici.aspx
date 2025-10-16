<%@ Page Title="" Language="C#" MasterPageFile="~/admin/YONETİCİ.Master" AutoEventWireup="true" CodeBehind="kullanici.aspx.cs" Inherits="OpenTicket.admin.kullanici" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="card">
        <div class="card-body">
            <div class="">
                <label>Kullanıcı adı</label>
                <asp:TextBox runat="server" ID="kullaniciadi" CssClass="form-control"></asp:TextBox>
                <label>Kullanıcı Şifre</label>
                <asp:TextBox runat="server" ID="kullanicisifre" CssClass="form-control"></asp:TextBox>
                <label>Kullanıcı mail</label>
                <asp:TextBox runat="server" ID="kullanicimail" CssClass="form-control"></asp:TextBox>
                <asp:Button Text="Kaydet"  runat="server" CssClass="btn btn-info mt-1 float-right" OnClick="Unnamed1_Click"/>


            </div>


        </div>


    </div>


    <div class="card">
        <div class="card-body">
            <div id="table" runat="server">


            </div>


        </div>


    </div>

</asp:Content>
