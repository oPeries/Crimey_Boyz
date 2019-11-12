<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                </div>
            </div>
        </div>
    </section>
    <div class="login-bg text-center">
        <div class="container">
            <div class="form">         
    			<div class="tab-content">
    				<div id="signup">
    					<h1>Gain Researcher Access</h1>
    					<?php echo validation_errors(); ?>
                        <?php echo form_open('researcher/register'); ?>
                            <div class="field-wrap">
                                <label>Name<span class="req">*</span>
                                </label>
                                <input type="text" name="name" required autocomplete="off"/>
                            </div>

                            <div class="field-wrap">
                                <label>Email Address<span class="req">*</span>
                                </label>
                                <input type="email" name="email" required autocomplete="off"  pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"/>
                            </div>

                            <div class="field-wrap">
                                <label>Research Institution<span class="req">*</span>
                                </label>
                                <input type="text" name="institution" required autocomplete="off"/>
                            </div>   

                            <div class="field-wrap">
                                <label>Field<span class="req">*</span>
                                </label>
                                <input type="text" name="field" required autocomplete="off"/>
                            </div>
                            
                            <div class="field-wrap">
                                <label>User Name<span class="req">*</span>
                                </label>
                                <input type="text" name="username" required autocomplete="off"/>
                            </div>

                            <div class="field-wrap">
                                <label>Set A Password<span class="req">*</span>
                                </label>
                                <input type="password"required autocomplete="off" name="password_1" pattern="(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}" title="Must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters"/>
                            </div>
                            
                            <div class="field-wrap">
                                <label>Confirm Password<span class="req">*</span>
                                </label>
                                <input type="password"required autocomplete="off" name="password_2" pattern="(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,}" title="Must contain at least one number and one uppercase and lowercase letter, and at least 8 or more characters"/>
                            </div>   
                                                    
                            <button type="submit" class="button button-block" name="reg_user">Get Started</button>
    				
                        <?php echo form_close(); ?>
                    </div>
                </div>
            </div>
        </div>
    </div>
</main>