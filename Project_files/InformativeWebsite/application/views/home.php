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
                    }, 800, function() {

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
                        <img class="big_logo" src="<?php echo base_url('images/CRIMEYBOYS.png');?>" alt="CRIMEYBOYS">
                        <h3 style="color:#aaa">Coming Soon</h3>
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

                        <p class="pt-3 text-paragraph">In a world where monolithic corporations are left unchecked, one bank is all that stands between five hapless criminals being dirt poor and filthy rich. Crimey Boyz is an epic platformer party game where up to five players can work together to pull off the most ridiculous heist in the galaxy. Work together or alone to pay off your space debt and profit as much as possible.
                        </p><br>

                        <div class="further-info">
                            <form method='post' action='<?= base_url('expressinterest') ?>'>
                                <h5>If you are interested in Crimey Boyz...</h5>
                                <input type="submit" name="count_click" value='Let us know' class="template2-btn mt-4" formaction="#signup">
                            </form>
                            <div>
                                <br><br>
                                <h4 style="color:#111">Share on Social Media!</h4>
                                <a href="https://www.facebook.com/sharer/sharer.php?u=<?= urlencode(base_url()) ?>" class="template2-btn mt-4">Facebook</a>
                                <a href="http://twitter.com/share?text=Crimey Boyz looks awesome!&url=<?= urlencode(base_url()) ?>" class="template2-btn mt-4">Twitter</a>
                                <!--<a href="http://www.linkedin.com/shareArticle?mini=true&url=<?= urlencode(base_url()) ?>" class="template2-btn mt-4">LinkedIn</a> -->
                                <a href="http://www.reddit.com/submit?url=<?= urlencode(base_url()) ?>" target="_blank" title="Submit to Reddit" class="template2-btn mt-4">Reddit</a>
                            </div>
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
                        <p style="color:white">
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
                            Please see the video below for more information.<br>
                        </p>
                        <iframe width="100%" height="500vh" src="https://www.youtube.com/embed/wDFIXjX_PM4" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
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
                        <h3 style="color: white">Play Crimey Boyz</h3><br>
                        <p class="pt-3 text-paragraph" style="color:white">
                            <i>Crimey Boyz is free and you can download the game using the link below.<br>
                            </i>
                            A PC connected to a TV is recommended so all players can comfortably see the screen<br>
                            Android is recommended on a tablet but a smartphone should also work<br>
                            A download link for the game will be made available soon<br>

                        </p>
                    </div>
                </div>
            </div>
        </div>
    </section>

    <section class="login-bg text-center" id="signup">
        <div class="container">
            <div class="form">
                <div class="tab-content">
                    <div>
                        <h2 class="eoi">Expression of Interest</h2>

                        <h2><?php echo validation_errors(); ?></h2>
                        <?php echo form_open('home'); ?>
                        <div class="field-wrap">
                            <label>Name<span class="req">*</span>
                            </label>
                            <input type="text" name="name" required autocomplete="on" value="<?php echo set_value('name');?>" />
                        </div>

                        <div class="field-wrap">
                            <label>Email Address<span class="req">*</span>
                            </label>
                            <input type="email" name="email" required autocomplete="on" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$" value="<?php echo set_value('email');?>" />
                        </div>

                        <div class="field-wrap">
                            <label>Age<span class="req">*</span>
                            </label>
                            <select name="age" required autocomplete="off" selected="<?php echo set_value('age');?>">
                                <option value="">Please Select</option>
                                <?php foreach ($ages as $age): ?>
                                <option value=<?php echo $age['id']?>><?php echo $age['age_range']?></option>
                                <?php endforeach; ?>
                            </select>
                        </div>

                        <div class="field-wrap">
                            <label>Country<span class="req">*</span>
                            </label>
                            <select name="country" required autocomplete="off" selected="<?php echo set_value('country');?>">
                                <option value="">Please Select</option>
                                <?php foreach ($countries as $country): ?>
                                <option value=<?php echo $country['id']?>><?php echo $country['country_name']?></option>
                                <?php endforeach; ?>
                            </select>
                        </div>

                        <div class="inline-field">
                            <label for="early-access">Sign up for early access</label>
                            <input type="checkbox" id="early-acccess" name="early_access">
                        </div>

                        <div class="field-wrap">
                            <label>Leave a comment:</label>
                            <textarea rows="4" cols="50" name="comment"><?php echo set_value('comment');?></textarea>
                        </div>

                        <button type="submit" class="button button-block" name="express_interest">Submit</button>

                        <?php echo form_close(); ?>
                    </div>
                </div>
            </div>
        </div>
    </section>
</main>
