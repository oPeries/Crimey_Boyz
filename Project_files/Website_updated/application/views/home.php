<main>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script>
        $(document).ready(function() {
            // Add smooth scrolling to all links
            $("a").on('click', function(event) {

                // Make sure this.hash has a value before overriding default behavior
                if (this.hash !== "") {
                    // Prevent default anchor click behavior
                    event.preventDefault();

                    // Store hash
                    var hash = this.hash;

                    // Using jQuery's animate() method to add smooth page scroll
                    // The optional number (800) specifies the number of milliseconds it takes to scroll to the specified area
                    $('html, body').animate({
                        scrollTop: $(hash).offset().top
                    }, 400, function() {

                        // Add hash (#) to URL when done scrolling (default click behavior)
                        window.location.hash = hash;
                    });
                } // End if
            });
        });

    </script>
    <section class="banner-area landing-page text-center">
        <div class="container">
            <div class="row">
                <div class="intro">
                    <div class="banner-image">
                        <img class="big_logo" src="<?php echo base_url('images/crimey_boyz_logo.png');?>" alt="CRIMEYBOYS">
                    </div>
                </div>
            </div>
        </div>
        <div class="more-content" id="first">
            <a href="<?php echo base_url('/index.php/#first');?>">
                <div class="template-btn template-btn2 mt-4">Learn More</div>
            </a>
        </div>
    </section>

    <section class="welcome-area section-padding2 first-section" id="learn-more">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">

                        <h3>This is Crimey Boyz</h3>

                        <p class="pt-3 text-paragraph">
                            In a world where monolithic corporations are left unchecked, one bank is all that stands between five hapless criminals being dirt poor and filthy rich. Crimey Boyz is an epic platformer party game where up to five players can work together to pull off the most ridiculous heist in the galaxy. Work together or alone to pay off your space debt and profit as much as possible. <br>
                            <i>Crimey Boyz is free and you can download the game using the link below.<br></i>
                            A PC connected to a TV is recommended so all players can comfortably see the screen<br>
                            Android is recommended on a tablet but a smartphone should also work<br>
                        </p>

                        <div class="col-lg-12">
                            <a href="<?php echo base_url('home/download');?>" class="template2-btn mt-4">Download Crimey Boyz</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="extra-section section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">
                        <h3 style="color: white">The Goal</h3><br>
                        <p class="pt-3 text-paragraph" style="color:white">
                            Crimey Boyz is an asymmetric multiplayer game which aims to provide data of how users act in an asymmetrically balanced game. <br>
                            The game uses the asymmetry of the Gamepad and Mission Control roles as an imbalance of power to encourage both selfish and selfless play. <br>
                            The players are given a wide variety of means and incentives to act selfishly or altruistically with each other through the game’s systems.</p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="welcome-area section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">
                        <h3>The Gameplay</h3>
                        <p class="pt-3 text-paragraph">
                            Crimey Boyz runs on windows and requires the use of an Android device to act as the controller for Mission Control.
                            Mission control is the brains behind the operation, guiding the players through the space bank.
                            <br>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="extra-section section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">
                        <h3 style="color: white">How to Play Crimey Boyz</h3><br>
                        <p class="pt-3 text-paragraph" style="color: white">
                            Crimey Boys is a multiplayer party game in which up to 5 players conduct a getaway heist. Most of the players will play with a traditional gamepad controller. One of these players is <i>Mission Control</i>, who plays the game with a tablet, and can spawn platforms that can help or hinder other players. At the end of each game, the player is given a score equal to the amount of money they earnt in the game. <br>
                            To find out exactly how to play, head to the page below.
                            <div class="col-lg-12" style="text-align: center">
                                <a href="<?php echo base_url('GetStarted');?>" class="template2-btn mt-4">Get Started</a>
                            </div>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="welcome-area section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">
                        <iframe width="560" height="315" src="https://www.youtube.com/embed/6UAVVH0qtss" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
                    </div>
                </div>
            </div>
        </div>
    </section>
</main>
