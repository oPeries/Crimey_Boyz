<main>
    <section class="banner-area banner-area2 text-center">
        <div class="container">
            <div class="row">
                <div class="further-info" style="margin: 0 auto">
                    <h1 style="text-align:center"><i>Public Interest Statistics</i></h1>
                </div>
            </div>
        </div>
    </section>
    <div class="login-bg">
        <div class="container">
            <h2 style="text-align:center;color:#222">Totals & Counts</h2>
            <div class="form">
<!--
                <div class="item-bar">
                </div>
-->
                <h4 class="counts">Total # of Page Visits (Excluding Refreshes)</h4>
                <table class="data-table">
                    <tr>
                        <th>Page</th>
                        <th># of Visits</th>
                    </tr>
                    <?php foreach ($total_visits as $page_visits): ?>
                    <tr>
                        <td><?php echo $page_visits['page'] ?></td>
                        <td><?php echo $page_visits['visits'] ?></td>
                    </tr>
                    <?php endforeach; ?>
                </table>
                <br>
<!--
                <div class="item-bar">
                </div>
-->
                <h4 class="counts">Total Number of Clicks on the Express Interest Button</h4>
                <p class="data-table"><?php echo $total_interest_clicks; ?></p>
<!--
                <div class="item-bar">
                </div>
-->
                <h4 class="counts">Total Expressions of Interest Submitted</h4>
                <p class="data-table"><?php echo $total_interest_submissions; ?></p>
            </div>
        </div>
    </div>
    <section class="extra-section section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <h2 style="text-align:center;color:#fff">Page Visits Over Time</h2>
                    <canvas id="pageVisitsGraph" width="600" height="300"></canvas>

<!--
                    <div class="item-bar">
                    </div>
-->

                    <h4>Select grouping</h4>
                    <select id="visitsGraph_select">
                        <option value="year">By Year</option>
                        <option value="month">By Month</option>
                        <option value="day" selected>By Day</option>
                        <option value="hour">By Hour</option>
                    </select>

<!--
                    <div class="item-bar" style="padding: 3px;">
                    </div>
-->

                    <p>
                        <h4 id="visitsGraph_slider_text">Graph range:</h4>
                    </p>

<!--
                    <div class="item-bar">
                    </div>
-->

                    <div id="visitsGraph_slider"></div>
                </div>
            </div>
        </div>
    </section>
    <section class="welcome-area section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <h2 style="text-align:center;color:#222">Age Breakdown</h2>
                    <canvas id="ageRangesChart" width="600" height="300"></canvas>

                </div>
            </div>
        </div>
    </section>
    <section class="extra-section section-padding2">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <h2 style="text-align:center;color:#fff">Nationality Breakdown</h2>
                    <canvas id="countriesChart" width="600" height="300"></canvas>

                </div>
            </div>
        </div>
    </section>
    <section class="welcome-area section-padding3">
        <div class="container-fluid">
            <div class="row">
                <div class="col-md-6 align-self-center textbox">
                    <div class="welcome-text mt-5 mt-md-0">

                        <h2 style="text-align:center;color:#222">Expressions of interest</h2>

                        <p class="pt-3 text-paragraph">All expressions of interest submitted.</p>

                    </div>
                </div>
                <div style="overflow-y: scroll; height:700px; width:100%; margin:5%">
                    <table style="width:100%">
                        <tr>
                            <th>Submission Time (UTC)</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Age</th>
                            <th>Country</th>
                            <th>Early Access</th>
                            <th>Comment</th>
                        </tr>

                        <?php foreach ($all_interest_submissions as $submission): ?>
                        <tr>
                            <td><?php echo isset($submission['submission_time']) ? $submission['submission_time'] : '' ?></td>
                            <td><?php echo isset($submission['name']) ? $submission['name'] : '' ?></td>
                            <td><?php echo isset($submission['email']) ? $submission['email'] : '' ?></td>
                            <td><?php echo isset($submission['age_range']) ? $submission['age_range'] : '' ?></td>
                            <td><?php echo isset($submission['country_name']) ? $submission['country_name'] : '' ?></td>
                            <td><?php echo isset($submission['early_access']) ? ($submission['early_access'] === '1' ? 'Yes' : 'No') : '' ?></td>
                            <td><?php echo isset($submission['comment']) ? $submission['comment'] : '' ?></td>
                        </tr>
                        <?php endforeach; ?>
                    </table>
                </div>
            </div>
        </div>
    </section>
</main>

<!--Inject the DB data to JS for display on the graphs & call the script to process the graphs-->
<script type='text/javascript'>
    <?php
echo "var all_page_visits = ".json_encode($all_visits).";\n";
echo "var submission_age_ranges = ".json_encode($submission_age_ranges).";\n";
echo "var submission_countries = ".json_encode($submission_countries).";\n";
?>

</script>
<script src="<?php echo base_url('js/admin.js');?>"></script>
