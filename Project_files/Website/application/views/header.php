<!DOCTYPE html>
<html lang="en">
<head>

    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">

    <title>Crimey Boyz</title>

    <link rel="shortcut icon" href="<?php echo base_url('images/logo.png');?>" type="image/x-icon">

    <link rel="stylesheet" href="<?php echo base_url('css/animate-3.7.0.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/font-awesome-4.7.0.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/bootstrap-4.1.3.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/owl-carousel.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/jquery.datetimepicker.min.css');?>">
    <link rel="stylesheet" href="<?php echo base_url('css/style.css');?>">
    <?php if( $page == 'profile') { ?>
            <link rel="shortcut icon" href="http://designshack.net/favicon.ico">
            <link rel="icon" href="http://designshack.net/favicon.ico">
            <link rel="stylesheet" type="text/css" media="all" href="<?php echo base_url('css/user-profile-style.css');?>">
            <script type="text/javascript" src="<?php echo base_url('js/jquery-1.10.2.min.js');?>"></script>
    <?php } ?>
</head>
<body>

	<header class="header-area">
        <div class="container">
            <div class="row">
                <div class="col-lg-2">
                    <div class="logo-area">
                        <a href="<?php echo base_url('/index.php/home');?>"><img src="<?php echo base_url('images/logo.png');?>" alt="logo"></a>
                    </div>
                </div>
                <div class="col-lg-10">
                    <div class="custom-navbar">
                        <span></span>
                        <span></span>
                        <span></span>
                    </div>
                    <div class="main-menu">
                        <ul>
                            <li class="active"><a href="<?php echo base_url('/home');?>">Home</a></li>

                            <li><a href="<?php echo base_url('#section2');?>">Game</a></li>

                            <li><a href="<?php echo base_url('/data');?>">Data</a></li>

                            <li><a href="<?php echo base_url('/about');?>">Researchers</a>

                                <ul class="sub-menu">
                                    <?php if( isset($_SESSION['username']) && !empty($_SESSION['username']) ) { ?>
                                        <?php if( $_SESSION['type'] == "researcher") { ?>
                                            <li><a href="<?php echo base_url('/ResearcherProfile');?>">My Profile</a></li>
                                            <li><a href="<?php echo base_url('/researcher/logout');?>">Logout</a></li>
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