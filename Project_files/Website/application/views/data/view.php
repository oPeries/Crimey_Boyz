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
                    <p>
                        <?php
                        echo '<h2>'.$data_item['name'].'</h2>';
                        echo $data_item['description'];
                        ?>
                    </p>
                    <p><a href="<?php echo site_url('data/'); ?>">Back to Menu</a></p>
                </div>
        </div>
    </div>
</main>