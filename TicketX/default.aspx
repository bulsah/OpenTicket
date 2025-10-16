<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="OpenTicket._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>OpenTicket - Event Ticketing System</title>
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
        <video autoplay muted loop id="myVideo" runat="server">
            <source src="fe/css/blsh.mp4" type="video/mp4">
        </video>
        
        <div id="filters" class="container mt-4" runat="server">
            <div class="row">
                <div class="col-3">
                    <a href="default.aspx">
                        <div class="card">
                            <div class="card-body text-center">
                                <h3><i class="fa fa-acorn"></i> OpenTicket</h3>
                            </div>
                        </div>
                    </a>
                </div>
                
                <div class="col-3">
                    <a href="?filter=1">
                        <div class="card">
                            <div class="card-body text-center">
                                <h3><i class="fa fa-microphone-stand"></i> Concert</h3>
                            </div>
                        </div>
                    </a>
                </div>
                
                <div class="col-3">
                    <a href="?filter=2">
                        <div class="card">
                            <div class="card-body text-center">
                                <h3><i class="fa fa-theater-masks"></i> Theater</h3>
                            </div>
                        </div>
                    </a>
                </div>
                
                <div class="col-3">
                    <a href="?filter=3">
                        <div class="card">
                            <div class="card-body text-center">
                                <h3><i class="fa fa-laptop-code"></i> Seminar</h3>
                            </div>
                        </div>
                    </a>
                </div>
            </div>
        </div>
        
        <div class="container mt-2">
            <div id="slider" runat="server" class="row">
                <div class="card col-lg-4 col-sm-12">
                    <img class="card-img-top" src="posters/i.jpeg" alt="Event poster">
                    <div class="card-body">
                        <p class="card-text">Sample Event 22.07.2021 16:30</p>
                        <div class="text-center">
                            <a href="default.aspx?detail=1" class="btn btn-danger">View Details</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="eventDetails" runat="server" class="container">
            <div class="row">
                <div class="col-lg-8 col-sm-12 col-md-8 mt-2">
                    <div class="card-m text-center">
                        <img class="afis" src="posters/i.webp" />
                    </div>
                </div>
                
                <div class="col-lg-4 col-sm-12 col-md-4 mt-2">
                    <div class="card-m">
                        <div class="card-body-m">
                            <div class="konserbaslik">
                                Sample Event
                            </div>  
                            <div class="konserdetay">
                                <p class="text-center text-capitalize">Event description here</p>
                                <p class="text-left">
                                    <i class="fa fa-location mr-2"></i> Main Hall
                                </p>
                                <p class="text-left">
                                    <i class="fa fa-calendar mr-2"></i> 22.07.2021 14:30
                                </p>
                                <p class="text-left">
                                    <i class="fa fa-users mr-2"></i> 12/120
                                </p>
                                <p class="text-left">
                                    <i class="fa fa-money-bill mr-2"></i> 220 TL
                                </p>
                                <div class="text-center">
                                    <a href="buyticket.aspx" class="btn btn-danger">
                                        <i class="fa fa-cart-plus"></i> Buy Ticket
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
    
    <script src="fe/js/jquery.js"></script>
    <script src="fe/js/bootstrap.js"></script>
    <script type="text/javascript">
        $('.carousel').carousel();
    </script>
</body>
</html>
