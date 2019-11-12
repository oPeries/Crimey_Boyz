<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="search">
                    <div class="banner-image">
                        <img class="big_logo" src="<?php echo base_url('images/CRIMEYBOYS.png');?>" alt="CRIMEYBOYS">
                    </div>
                </div>
            </div>
        </div>
        <div class="more-content">
            <a href="<?php echo base_url('#section2');?>" class="template-btn template-btn2 mt-4">Learn More</a>
        </div>
    </section>

    <section class="welcome-area section-padding2" id="section2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">

                        <h3><span class="style-change">Welcome</span> <br>to Crimey Boyz</h3>

                        <p class="pt-3 text-paragraph">Slimy Boyz is an asymmetric platformer party game.

                            <i>“Different opportunities, different interfaces, different goals”</i>

                            Players are able to make alliances and betray each other through the pickups and game mechanics.

                            Most players play with a standard gamepad.

                            One player always plays with a mobile tablet, which interfaces with the same instance of the game as the others.

                            The game is intended to be quite accessible, and is focused around cultivating the social dynamics that our client wants to research.
                        </p>

                        <!--                        <a href="<?php echo base_url('login');?>" class="template2-btn mt-3">Login</a>-->

                        <div class="col-lg-12">
                            <form method='post' action='<?= base_url() ?>home/createzip/'>
       <input type="submit" name="download" value='Download Crimey Boyz'>
    </form>
                            <a href="<?php echo base_url('download');?>" class="template2-btn mt-4">Download</a>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="leaderboard section-padding2" id="section3">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">

                        <h3><span class="style-change">Leaderboard</span></h3>

                        <p class="leaderboard_text">This is a leaderboard for public to view game data.</p>

                    </div>
                    <div><img class="leaderboard_image" src="<?php echo base_url('images/leaderboard_placeholder.png');?>" alt="leaderboard_placeholder"></div>
                </div>
            </div>
        </div>
    </section>
</main>
