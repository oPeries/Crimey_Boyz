<?php

require_once('../private/initialise.php');

if(!is_post_request()) {
    //Ensure only post requests can access this page
    error_404();

} else {

  //Game validation would go here once implemented (Maybe a pre-shared key in the url parameters?)

  //Get the POST data to check user credentials
  $username = $_POST['username'] ?? '';
  $password = $_POST['password'] ?? '';

  //Check if the user credentials are valid
  $db = db_connect();
  $result = check_user_credentials($username, $password);
  db_disconnect($db);

  if(!$result) {
    //If credentials are not a match, throw 404
    error_404();
  }
}

?>
