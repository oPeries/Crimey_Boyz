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

                    <?php echo form_open('data/edit/'.$data_item['id']); ?>
                        <table class="edit">
                            <tr>
                                <td><label for="title"></label></td>
                                <td><input type="input" name="name" size="50" value="<?php echo $data_item['name'] ?>" /></td>
                            </tr>
                            <tr>
                                <td><label for="text"></label></td>
                                <td><textarea name="description" rows="10" cols="40"><?php echo $data_item['description'] ?></textarea></td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><input type="submit" name="submit" value="Edit item" /></td>
                            </tr>
                        </table>
                    </form>
                </div>
            </div>
    </div>
</main>