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
                <div class="item-bar">
                    <!-- <a href="<?php echo base_url('data/create'); ?>">Add</a> | -->
                    <!-- <a href="<?php echo base_url('data/search'); ?>">Search</a> -->
                </div>
                
                <?php foreach ($data as $data_item): ?>
                        <ul class="item">
                            <li class="item-name">ID: <?php echo $data_item['id']; ?></li>
                            <li class="item-des">User ID: <?php echo $data_item['user_id']; ?></li>
                            <li class="item-des">Number of Collisions: <?php echo $data_item['collisions']; ?></li>
                            <li class="item-des">Score: <?php echo $data_item['score']; ?></li>
                            <!-- <li class="item-action">
                                <a href="<?php echo site_url('data/view/'.$data_item['slug']); ?>">View</a> | 
                                <a href="<?php echo site_url('data/edit/'.$data_item['id']); ?>">Edit</a> | 
                                <a href="<?php echo site_url('data/delete/'.$data_item['id']); ?>" onClick="return confirm('Are you sure you want to delete?')">Delete</a>
                            </li> -->
                        </ul>
                <?php endforeach; ?>
                <div class="download_data">
                    <a href="<?php echo base_url('/data/exportCSV');?>" class="template-btn template-btn2 mt-4">Download</a>
                </div>
            </div>
         </div>
    </div>
</main>