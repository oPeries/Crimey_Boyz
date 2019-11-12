<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
            </div>
        </div>
    </section>    

    <section class="login-bg text-center">
        <div class="container">
            <div class="form">
    			<div class="tab-content">
                    <div id="login">
                        <h1>Login</h1>	
                        <?php echo form_open('users/login'); ?>
                        
                        <div class="field-wrap">
                            <label>User Name<span class="req">*</span>
                            </label>
                            <input type="text" required name="username" value="<?php if (isset($_COOKIE["username"])): echo $_COOKIE["username"]; endif ?>"/>
                        </div>

                        <div class="field-wrap">
                            <label>Password<span class="req">*</span>
                            </label>
                            <input type="password" required name="password"/>
                        </div>
                        
                        <div class ="inline-field">
                                <input type="checkbox" id="remember-me" name="remember-me" <?php if (isset($_COOKIE["email"])): echo "checked"; endif ?>>
                                <label for="remember-me">Keep me logged in</label>
                        </div>

                        <button class="button button-block" type="submit" name="login_user">Log In</button>

                        <?php echo form_close(); ?>
                    </div>
                </div>
            </div>
        </div>
    </section>
</main>
