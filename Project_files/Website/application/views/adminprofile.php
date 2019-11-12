<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h1><i>Welcome Back <?php echo $_SESSION['username']; ?>!</i></h1>
                </div>
            </div>
        </div>
    </section>    
    <section class="login-bg text-center">
        <div class="form" id="w">
            <?php if (isset($_SESSION['success'])) : ?>
                <div class="error success" >
                    <h3>
                        <?php 
                            echo $_SESSION['success']; 
                            unset($_SESSION['success']);
                        ?>
                    </h3>
                </div>
            <?php endif ?>
            <div id="content" class="clearfix">

                <div class="userdata">
                <?php  if (isset($_SESSION['username'])) : ?>
                    <h1><strong><?php echo $_SESSION['username']; ?></strong></h1>
                    <p> <a href="<?php echo base_url('researcher/logout');?>" class="main-menu">Logout</a> </p>
                <?php endif ?>
                <div class="userdata">
                    <?php if (file_exists('uploads/'.$_SESSION['username'].'.png')) { ?>
                            <img src="<?php echo base_url('uploads/'.$_SESSION['username'].'.png');?>" alt="default avatar">;
                        <?php }else{ ?>
                            <img src="<?php echo base_url('images/avatar.png');?>" alt="default avatar">
                        <?php } ?>
                </div> 
                <?php echo $error;?>

                <nav id="profiletabs">
                    <ul class="clearfix">
                        <li><a href="#bio" class="sel">Profile</a></li>
                        <li><a href="#settings">Settings</a></li>
                    </ul>
                </nav>
              
                <section id="bio">
                    <p>Welcome back to FoodToYou!</p>
                    
                    <p>Here you can edit your profile and settings. Add a photo of yourself (or your favourite food) or update your email address or even change your favourite food.</p>
                    
                    <p>Feeling hungry? Whilst you're here, why not take a look into all the scrumptious meals FoodToYou can delivery straight to your hands. Whether you're peckish or starving, we have everything and anything you're craving. Now we're getting hungry...</p>
                </section>
              
                <section id="settings" class="hidden">
                    <p><strong>Edit your user settings:</strong></p>

                    <p class="setting"><span>Name </span><?php echo $users_data['name']; ?></p>

                    <p class="setting"><span>User Name </span><?php echo $users_data['username']; ?></p>
                    
                    <p class="setting"><span>E-mail Address </span><?php echo $users_data['email']; ?></p>
                    
                    <p class="setting"><span>Cuisine </span><?php echo $users_data['cuisine']; ?></p>
                    
                    <p class="setting"><span>Default Delivery Address </span><?php echo $users_data['address']; ?></p>

                    <p><strong>Upload your profile picture:</strong></p>

                    <?php echo form_open_multipart('researcherProfile/do_upload');?>

                        <input type="file" name="userfile" size="20" />

                        <br /><p>Upload square photos for best results. Images must be .png</p><br />

                        <input type="submit" value="Upload" />
                    </form>
                </section>
            </div>
        </div>
    </section>    
</main>