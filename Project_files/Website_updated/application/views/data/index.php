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
                <h3><span class="style-change">Data</span></h3>
                <p class="leaderboard_text">Below is data collected from the game downloadable in a CSV file.</p>
                <!--<?php foreach ($data as $data_item): ?>
                        <ul class="item">
                            <li class="item-name">ID: <?php echo $data_item['id']; ?></li>
                            <li class="item-des">User ID: <?php echo $data_item['user_id']; ?></li>
                            <li class="item-des">Number of Collisions: <?php echo $data_item['collisions']; ?></li>
                            <li class="item-des">Score: <?php echo $data_item['score']; ?></li>
                        </ul>
                <?php endforeach; ?> -->
                <div class="download_data">
                    <a href="<?php echo base_url('/data/exportCSV');?>" class="template-btn template-btn2 mt-4">Download</a>
                </div>
            </div>
         </div>
    </div>
</main>