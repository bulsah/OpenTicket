<%@ Page Title="" Language="C#" MasterPageFile="~/admin/YONETİCİ.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="OpenTicket.admin.WebForm1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="row">



    
    <div class="col-lg-4 col-sm-12">
        <div class="card bg-yesil">
        <div class="card-content">
          <div class="card-body">
            <div class="media d-flex">
              <div class="align-self-center">
                <i class="fa fa-user fa-2x float-left"></i>
              </div>
              <div class="media-body white text-right">
                <h3 runat="server" id="usercount">278</h3>
                <span>Toplam Kullanıcı</span>
              </div>
            </div>
          </div>
        </div>
      </div>


    </div>
    
  

     <div class="col-lg-4 col-sm-12">
        <div class="card bg-pembe">
        <div class="card-content">
          <div class="card-body">
            <div class="media d-flex">
              <div class="align-self-center">
                <i class="fa fa-database fa-2x float-left"></i>
              </div>
              <div class="media-body white text-right">
                <h3 runat="server" id="te">278</h3>
                <span>Toplam Etkinlik</span>
              </div>
            </div>
          </div>
        </div>
      </div>


    </div>

     <div class="col-lg-4 col-sm-12">
        <div class="card bg-sari">
        <div class="card-content">
          <div class="card-body">
            <div class="media d-flex">
              <div class="align-self-center">
                <i class="fa fa-check fa-2x float-left"></i>
              </div>
              <div class="media-body white text-right">
                <h3 runat="server" id="ae">278</h3>
                <span>Aktif Etkinlik</span>
              </div>
            </div>
          </div>
        </div>
      </div>


    </div>
    
    

    
</div>  
    

</asp:Content>
