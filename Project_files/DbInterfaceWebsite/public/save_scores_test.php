<?php

require_once('../private/initialise.php');

if(!is_post_request()) {
    //Ensure only post requests can access this page
    error_404();

} else {

  //Game validation would go here once implemented (Maybe a pre-shared key in the url parameters?)

  //Get the POST data to add a new score
  $username = $_POST['username'] ?? '';
  $password = $_POST['password'] ?? '';
  if(!$_POST['collisions'] || !$_POST['score']) {
      error_404(); //No data? 404 mate
  }
  $collisions = $_POST['collisions'] ?? '';
  $score = $_POST['score'] ?? '';

  //Check if the user credentials are valid
  $db = db_connect();
  if(!check_user_credentials($username, $password)) {
    db_disconnect($db);
    error_404(); //Not valid? 404 mate
  }

  //Here if user credentials are valid
  $result = insert_score($username, $collisions, $score);
  db_disconnect($db);

  if(!$result) {
    error_404();
  }
}

?>
