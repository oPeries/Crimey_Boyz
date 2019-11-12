<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
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
              
                <section id="bio">
                    <p>Welcome back to Crimey Boyz!</p>
                    
                    <p>Here you can edit your profile and settings. Add a photo of yourself or update your email address.</p>
                </section>
              
                <section id="settings" class="hidden">
                    <p><strong>Edit your user settings:</strong></p>

                    <p class="setting"><span>Name </span><?php echo $users_data['name']; ?></p>

                    <p class="setting"><span>User Name </span><?php echo $users_data['username']; ?></p>
                    
                    <p class="setting"><span>E-mail Address </span><?php echo $users_data['email']; ?></p>
                    
                    <p class="setting"><span>Insitution </span><?php echo $users_data['institution']; ?></p>
                    
                    <p class="setting"><span>Field </span><?php echo $users_data['field']; ?></p>

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