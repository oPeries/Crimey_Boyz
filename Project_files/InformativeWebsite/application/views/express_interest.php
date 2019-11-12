<main>
	<section class="banner-area banner-area2 text-center">
		<div class="container">
			<div class="row">
				<div class="further-info" style="margin: 0 auto">
					<h1><i>Want to get Crimey Boys?</i></h1>
                    <h4><i>Let us know you're interested!</i></h4>
				</div>
			</div>
		</div>
	</section>

	<section class="login-bg text-center">
		<div class="container">
			<div class="form">
				<div class="tab-content">
					<div id="signup">
						<h1>Expression of Interest</h1>

						<h1><?php echo validation_errors(); ?></h1>
						<?php echo form_open('expressinterest'); ?>
						<div class="field-wrap">
							<label>Name<span class="req">*</span>
							</label>
							<input type="text" name="name" required autocomplete="on" value="<?php echo set_value('name');?>"/>
						</div>

						<div class="field-wrap">
							<label>Email Address<span class="req">*</span>
							</label>
							<input type="email" name="email" required autocomplete="on" pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$" value="<?php echo set_value('email');?>"/>
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
							<input type="checkbox" id="early-acccess" name="early_access">
							<label for="early-access">Sign up for early access</label>
						</div>

						<div class="field-wrap">
							<label>Leave a comment</label>
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
