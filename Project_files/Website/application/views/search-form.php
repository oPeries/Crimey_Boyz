<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                    <div class="search">
                        <?php
                            echo form_open('data/execute_search');

                            echo form_input(array('name'=>'search', 'class'=>'searchTerm', 'placeholder'=>'What are you looking for?'));
                        ?>
                        <button type="submit" class="searchButton">
                            <i class="fa fa-search"></i>
                        </button>
                    </div>
            </div>
        </div>
    </section>
</main>