<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                </div>
            </div>
        </div>
    </section>    
    <div class="login-bg">
        <div class="container">
            <div class="form">
                <h3>Your file was successfully uploaded!</h3>
                <?php if (file_exists('uploads/'.$_SESSION['username'].'.png')) { ?>
                    <img src="<?php echo base_url('uploads/'.$_SESSION['username'].'.png');?>" alt="default avatar">;
                <?php } else { ?>
                    <img src="<?php echo base_url('images/avatar.png');?>" alt="default avatar">
                <?php } ?>

                <p><a href="<?php echo base_url('UserProfile/'); ?>">Back to Profile</a></p>
            </div>
        </div>
    </div>
</main>