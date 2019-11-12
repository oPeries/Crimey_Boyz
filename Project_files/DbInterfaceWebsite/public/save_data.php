<?php

require_once('../private/initialise.php');

if(!is_post_request()) {
    //Ensure only post requests can access this page
    error_404();

} else {

  //Game validation would go here once implemented (Maybe a pre-shared key in the url parameters?)

  $contents = file_get_contents('php://input');
  //echo $contents . "\n";

  $data = json_decode($contents, true);

  $db = db_connect();

  //Session info and rounds info MUST be set to save to DB
  if(isset($data["session"]) && isset($data["rounds"])) {

      //Try save the session data
      $sessionID = insert_session($data["session"]);
      if($sessionID == $false) {
          echo "failed to save session info\n";
          db_disconnect($db);
          error_404();
      } else {

          //Try to save the round data
          foreach($data["rounds"] as $round) {
              $roundID = insert_round($sessionID, $round);

              if($roundID == $false) {
                  echo "failed to save round info\n";
                  db_disconnect($db);
                  error_404();

              } else {

                  foreach($round["interactions"] as $interaction) {
                      if(!insert_interaction($roundID, $interaction)) {
                          echo "interaction insert failed\n";
                      }
                  }
              }
          }
      }

  } else {
      echo "session or rounds not set\n";
      db_disconnect($db);
      error_404();
  }

  //Here if did not error
  db_disconnect($db);
}

?>
