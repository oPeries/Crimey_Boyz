<!DOCTYPE html>
<html lang="en">

<head>

    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">

    <title>Crimey Boyz</title>

    <link rel="shortcut icon" href="<?php echo base_url('images/logo.png');?>" type="image/x-icon">
    <link rel="stylesheet" href="<?php echo base_url('css/style.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/jquery-ui.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/Chart.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/bootstrap-4.1.3.min.css');?>">

    <script src="<?php echo base_url('js/vendor/jquery-2.2.4.min.js');?>"></script>
    <script src="<?php echo base_url('js/vendor/jquery-ui.min.js');?>"></script>
    <script src="<?php echo base_url('js/vendor/Chart.min.js');?>"></script>
    <script src="<?php echo base_url('js/vendor/bootstrap-4.1.3.min.js');?>"></script>
    <!-- Hotjar Tracking Code for https://scruffle.uqcloud.net -->
    <script>
        (function(h, o, t, j, a, r) {
            h.hj = h.hj || function() {
                (h.hj.q = h.hj.q || []).push(arguments)
            };
            h._hjSettings = {
                hjid: 1527800,
                hjsv: 6
            };
            a = o.getElementsByTagName('head')[0];
            r = o.createElement('script');
            r.async = 1;
            r.src = t + h._hjSettings.hjid + j + h._hjSettings.hjsv;
            a.appendChild(r);
        })(window, document, 'https://static.hotjar.com/c/hotjar-', '.js?sv=');

    </script>
</head>

<body>

    <header class="header-area">
        <div class="container">
            <div class="row">
                <div class="col-lg-2">
                    <div class="logo-area">
                        <a href="<?php echo base_url('/index.php/home');?>"><img src="<?php echo base_url('images/CRIMEYBOYS.png');?>" alt="logo"></a>
                    </div>
                </div>
                <div class="col-lg-10">
                    <div class="main-menu">
                        <ul>
                            <li class="active"><a href="<?php echo base_url('/home');?>">Home</a></li>
                            <li><a href="<?php echo base_url('/data');?>">Researchers</a>
                                <ul class="sub-menu">
                                    <?php if( isset($_SESSION['username']) && !empty($_SESSION['username']) ) { ?>
                                        <?php if( $_SESSION['type'] == "researcher") { ?>
                                            <li><a href="<?php echo base_url('/ResearcherProfile');?>">My Profile</a></li>
                                            <li><a href="<?php echo base_url('/reseacher/logout');?>">Logout</a></li>
                                        <?php } if( $_SESSION['type'] == "user") { ?>
                                            <li><a href="<?php echo base_url('/researcher/register');?>">Sign Up</a></li>
                                            <li><a href="<?php echo base_url('researcher/login');?>">Login</a></li>
                                        <?php } ?>
                                    <?php } else{ ?>
                                        <li><a href="<?php echo base_url('/researcher/register');?>">Sign Up</a></li>
                                        <li><a href="<?php echo base_url('researcher/login');?>">Login</a></li>
                                    <?php } ?>
                                </ul>
                            </li>
                            <li>
                                <?php if( isset($_SESSION['username']) && !empty($_SESSION['username']) ) { ?>
                                    <?php if( $_SESSION['type'] == "user") { ?>
                                        <li><a href="<?php echo base_url('UserProfile');?>">My Account</a></li>
                                        <li><a href="<?php echo base_url('users/logout');?>">Logout</a></li>
                                    <?php } if( $_SESSION['type'] == "researcher") { ?>
                                        <li><a href="<?php echo base_url('ResearcherProfile');?>">My Account</a></li>
                                        <li><a href="<?php echo base_url('users/logout');?>">Logout</a></li>
                                    <?php } ?>
                                <?php } else { ?>
                                    <li><a href="<?php echo base_url('users/login');?>">Login</a></li>
                                    <li><a href="<?php echo base_url('users/register');?>">Sign Up</a></li>
                                <?php } ?>
                            </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    </header>