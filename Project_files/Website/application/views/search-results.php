<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="col-lg-12">
                    <h1><i>Data</i></h1>
                </div>
            </div>
        </div>
    </section>
    <div class="login-bg">
            <div class="container">
                <div class="form">
                    <?php foreach ($results as $val): ?>
                        <ul class="data">
                            <li class="data-name"><h5 class="results"><?php echo $val['name']; ?></h5></li>
                            <li class="data-des"><?php echo $val['description']; ?></li>
                            <li class="data-action">
                                <a href="<?php echo site_url('data/view/'.$val['slug']); ?>">View</a> 
                            </li>
                        </ul>
                <?php endforeach; ?>
                <p><a href="<?php echo site_url('data/'); ?>">Back to Data</a></p>
                </div>
        </div>
    </div>
</main>