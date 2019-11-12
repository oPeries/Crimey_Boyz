<?php

require_once('../private/initialise.php');

if(!is_post_request()) {
    //Ensure only post requests can access this page
    //echo "plzwhy";
    error_404();

} else {

    //echo "hello";

  //Game validation would go here once implemented (Maybe a pre-shared key in the url parameters?)

  //Get the POST data
  if(!isset($_POST['username']) || !isset($_POST['name']) || !isset($_POST['email']) || !isset($_POST['password1']) || !isset($_POST['password2'])) {
      error_404(); //No data? 404 mate
  }
  //echo "hello2";

  $username = $_POST['username'] ?? '';
  $name = $_POST['name'] ?? '';
  $email = $_POST['email'] ?? '';
  $password1 = $_POST['password1'] ?? '';
  $password2 = $_POST['password2'] ?? '';

  //Check if the user credentials are valid
  $db = db_connect();
  $result = insert_user($username, $name, $email, $password1, $password2);
  db_disconnect($db);

  echo $result;
}

?>
