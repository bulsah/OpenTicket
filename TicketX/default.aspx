<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="TicketX._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <link rel="preconnect" href="https://fonts.gstatic.com">
<link href="https://fonts.googleapis.com/css2?family=Comfortaa:wght@700&display=swap" rel="stylesheet"> 
    <link rel="preconnect" href="https://fonts.gstatic.com">
<link href="https://fonts.googleapis.com/css2?family=Caveat:wght@600&family=Comfortaa:wght@700&display=swap" rel="stylesheet"> <link href="fe/fa/css/all.css" rel="stylesheet" />
    <link href="fe/css/st.css" rel="stylesheet" />
    <link href="fe/css/bootstrap.css" rel="stylesheet" />
    <form id="form1" runat="server">
        <video autoplay muted loop id="myVideo" runat="server">
  <source src="fe/css/blsh.mp4" type="video/mp4">
</video>
          <div id="filtreler" class="container mt-4" runat="server">
            <div class="row">
                       <div class="col-3">
                 <a href="default.aspx">

 <div class="card">
                        <div class="card-body text-center">


                            <h3><i class="fa fa-acorn"></i> Ticketx</h3>
                        </div>


                    </div>
                 </a>
                   
                   

                </div>
                <div class="col-3">
                 <a href="?filtre=1">

 <div class="card">
                        <div class="card-body text-center">


                            <h3><i class="fa fa-microphone-stand "></i> Konser</h3>
                        </div>


                    </div>
                 </a>
                   
                   

                </div>
                <div class="col-3">

                     <a href="?filtre=2">
                <div class="card">
                        <div class="card-body text-center">

   <h3><i class="fa fa-theater-masks "></i> Tiyatro</h3>

                        </div>


                    </div>

</a>
                </div>
                <div class="col-3">
                     <a href="?filtre=3">
                  <div class="card">
                        <div class="card-body text-center">
   <h3><i class="fa fa-laptop-code"></i>Seminer</h3>

                        </div>


                    </div>
</a>

                </div>


            </div>


        </div>
        <div class="container mt-2">


      
        <div id="slider" runat="server" class="row">
           
            <div class="card col-lg-4 col-sm-12">
      
  <img class="card-img-top" src="afisler/i.jpeg" alt="Card image cap">
  <div class="card-body">
    <p class="card-text">Manga Konseri 22.07.2021 16:30</p>
     <div class="text-center">
   <a href="default.aspx?detay=1" class="btn btn-danger">Detayla</a>

     </div>
   
  </div>
</div>
       
  </div>






        </div>

     




        
        <div id="detayla" runat="server" class="container">

            <div class="row">
                <div class="col-lg-8 col-sm-12 col-md-8 mt-2">
                    <div class="card-m text-center">
                 
                        <img class="afis" src="afisler/i.webp" />

                    </div>

                </div>
                <div class="col-lg-4 col-sm-12 col-md-4 mt-2">
                   <div class="card-m">
                       <div class="card-body-m">
                      <div class="konserbaslik">

                                Manga Konseri

                      </div>  
           <div class="konserdetay">
               <p class="text-center text-capitalize" > kasdfjsajıfdjsadfj ıasdfndjfasdfsasdf aıosdjfasıodf poawerıgeropıqwervmjk owaqerıogfwwef</p>
               <p class="text-left">
              <i class="fa fa-location mr-2"></i>     Büyük Salon


               </p>
               <p class="text-left">
                 <i class="fa fa-calendar mr-2" ></i> 22.07.2021 14:30


               </p>
                  <p class="text-left">
                 <i class="fa fa-users mr-2" ></i>12/120


               </p>

               <p class="text-left">

                 <i class="fa fa-money-bill mr-2" ></i>220TL

               </p>
               <div class="text-center">
                   <btn class="btn btn-danger"><i class="fa fa-cart-plus"></i> Satın Al</btn>


               </div>


           </div>

                       </div>

                   </div>



                </div>

            </div>

        </div>

     <%--   <div class="mainfooter">
<h4>Sonraki Etkinlik İçin Kalan Süre</h4>
           
            <h1>16 Gün 18 Saat 17 Dk</h1>

        </div>--%>
    </form>
    <script src="fe/js/jquery.js"></script>
    <script src="fe/js/bootstrap.js"></script>


    <script type="text/javascript">
$('.carousel').carousel()


    </script>
</body>
</html>
