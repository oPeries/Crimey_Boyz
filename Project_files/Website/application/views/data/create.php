<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h1><i>Menu</i></h1>
                </div>
            </div>
        </div>
    </section>
    <div class="login-bg">
        <div class="container">
            <div class="form">
                <h2><?php echo $name; ?></h2>

                <?php echo validation_errors(); ?>

                <?php echo form_open('data/create'); ?>    
                    <table class="edit">
                        <tr>
                            <td><label class="create" for="name"></label></td>
                            <td><input type="input" name="name" size="50" placeholder="Name" /></td>
                        </tr>
                        <tr>
                            <td><label class="create" for="description"></label></td>
                            <td><textarea name="description" rows="10" cols="40" placeholder="Description"></textarea></td>
                        </tr>
                        <tr>
                            <td></td>
                            <td><input type="submit" name="submit" value="Create menu item" /></td>
                        </tr>
                    </table>    
                </form>
            </div>
        </div>
    </div>
</main>