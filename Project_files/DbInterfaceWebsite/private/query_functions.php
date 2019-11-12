<?php

function get_user_id($username) {
  global $db;

  $sql = "SELECT * FROM users ";
  $sql .= "WHERE username='" . $username . "' LIMIT 1";
  $result = mysqli_query($db, $sql);
  confirm_result_set($result);
  $user = mysqli_fetch_assoc($result);
  mysqli_free_result($result);
  return $user['id'];
}

function check_user_credentials($username, $password) {
  global $db;

  $sql = "SELECT * FROM users ";
  $sql .= "WHERE username='" . $username . "' LIMIT 1";
  $result = mysqli_query($db, $sql);
  confirm_result_set($result);
  $user = mysqli_fetch_assoc($result);
  mysqli_free_result($result);
  return password_verify($password, $user['password']);
}

function insert_score($username, $collisions, $score) {
  global $db;

  $id = get_user_id($username);
  if (!$id) {
    return false;
  }

  $sql = "INSERT INTO data ";
  $sql .= "(user_id, collisions, score) ";
  $sql .= "VALUES (";
  $sql .= "'" . $id . "',";
  $sql .= "'" . $collisions . "',";
  $sql .= "'" . $score . "'";
  $sql .= ")";
  $result = mysqli_query($db, $sql);
  // For INSERT statements, $result is true/false
  //return $result;
  if($result) {
    return true;
  } else {
    // INSERT failed
    //echo mysqli_error($db);
    error_404();
    db_disconnect($db);
    exit;
  }
}

//Expects $sessionInfo to be an array with the following
//"startTime" - mysql formatted timestamp
//"player1" - username of player 1
//"player2" - username of player 2
//"player3" - username of player 3
//"player4" - username of player 4 (optional)
//"player4" - username of player 5 (optional)
function insert_session($sessionInfo) {
    global $db;

    if(!isset($sessionInfo) || !isset($sessionInfo["startTime"])) {
        return false;
    }

    $sqlCols = "INSERT INTO sessions ";
    $sqlCols .= "(sessionStartTime";

    $sqlVals = "VALUES (";
    $sqlVals .= "'" . $sessionInfo["startTime"] . "'";

    if(isset($sessionInfo["player1"])) {
        if($sessionInfo["player1"] == "") {
            $sessionInfo["player1"] = "Guest";
        }
        $sqlCols.= ",player1";
        $sqlVals .= ",'" . get_user_id($sessionInfo["player1"]) . "'";
    }

    if(isset($sessionInfo["player2"])) {
        if($sessionInfo["player2"] == "") {
            $sessionInfo["player2"] = "Guest";
        }
        $sqlCols.= ",player2";
        $sqlVals .= ",'" . get_user_id($sessionInfo["player2"]) . "'";
    }

    if(isset($sessionInfo["player3"])) {
        if($sessionInfo["player3"] == "") {
            $sessionInfo["player3"] = "Guest";
        }
        $sqlCols.= ",player3";
        $sqlVals .= ",'" . get_user_id($sessionInfo["player3"]) . "'";
    }

    if(isset($sessionInfo["player4"])) {
        if($sessionInfo["player4"] == "") {
            $sessionInfo["player4"] = "Guest";
        }
        $sqlCols.= ",player4";
        $sqlVals .= ",'" . get_user_id($sessionInfo["player4"]) . "'";
    }

    if(isset($sessionInfo["player5"])) {
        if($sessionInfo["player5"] == "") {
            $sessionInfo["player5"] = "Guest";
        }
        $sqlCols.= ",player5";
        $sqlVals .= ",'" . get_user_id($sessionInfo["player5"]) . "'";
    }

    $sqlCols .= ")";
    $sqlVals .= ")";
    //echo $sqlCols . $sqlVals . "\n";
    $result = mysqli_query($db, $sqlCols . $sqlVals);

    if($result = false) {
        return false;
    }

    $sql = "SELECT LAST_INSERT_ID() AS ID";
    $result = mysqli_query($db, $sql);
    $id = mysqli_fetch_assoc($result);

    return $id["ID"];
}

//Expects $sessionID to be the integer ID of the session this round was played in (using DB id)
//Expects $roundInfo to be an array with the following
//"name" - the name of the scene / round played
//"startTime" - the seconds between the session start time and the round starting (float)
//"player1Score" - score of player 1 at the start of this round
//"player2Score" - score of player 2 at the start of this round
//"player3Score" - score of player 3 at the start of this round
//"player4Score" - score of player 4 at the start of this round (optional)
//"player5Score" - score of player 5 at the start of this round (optional)
//"tabletPlayer" - playernum of the tablet player this round
function insert_round($sessionID, $roundInfo) {
    global $db;

    if(!isset($sessionID) || !isset($roundInfo) || !isset($roundInfo["startTime"]) || !isset($roundInfo["name"])) {
        return false;
    }

    $sqlCols = "INSERT INTO rounds ";
    $sqlCols .= "(sessionID,sceneName,roundStartTime";

    $sqlVals = "VALUES (";
    $sqlVals .= "'" . $sessionID . "',";
    $sqlVals .= "'" . $roundInfo["name"] . "',";
    $sqlVals .= "'" . $roundInfo["startTime"] . "'";

    if(isset($roundInfo["player1Score"])) {
        $sqlCols.= ",player1StartingScore";
        $sqlVals .= ",'" . strval($roundInfo["player1Score"]) . "'";
    }

    if(isset($roundInfo["player2Score"])) {
        $sqlCols.= ",player2StartingScore";
        $sqlVals .= ",'" . strval($roundInfo["player2Score"]) . "'";
    }

    if(isset($roundInfo["player3Score"])) {
        $sqlCols.= ",player3StartingScore";
        $sqlVals .= ",'" . strval($roundInfo["player3Score"]) . "'";
    }

    if(isset($roundInfo["player4Score"])) {
        $sqlCols.= ",player4StartingScore";
        $sqlVals .= ",'" . strval($roundInfo["player4Score"]) . "'";
    }

    if(isset($roundInfo["player5Score"])) {
        $sqlCols.= ",player5StartingScore";
        $sqlVals .= ",'" . strval($roundInfo["player5Score"]) . "'";
    }

    $sqlCols .= ",tabletPlayer)";
    $sqlVals .= "," . $roundInfo["tabletPlayer"] . ")";

    //echo $sqlCols . $sqlVals . "\n";
    $result = mysqli_query($db, $sqlCols . $sqlVals);

    if($result = false) {
        return false;
    }

    $sql = "SELECT LAST_INSERT_ID() AS ID";
    $result = mysqli_query($db, $sql);
    $id = mysqli_fetch_assoc($result);

    return $id["ID"];
}

//Expects $roundID to be the integer ID of the round this interaction was performed in (using DB id)
//Expects $interactionInfo to be an array with the following
//"name" - the metric name of the action being performed
//"player" - the player number of the player performing the action (int)
//"time" - the seconds between the session start time and the action being performed (float)
//"x" - the x coordinate the action was performed from (float) (optional)
//"y" - the y coordinate the action was performed from (float) (optional)
//"data" - addational data for the specific action (optional)
function insert_interaction($roundID, $interactionInfo) {
    global $db;

    if(!isset($roundID) || !isset($interactionInfo) || !isset($interactionInfo["name"]) || !isset($interactionInfo["player"]) || !isset($interactionInfo["time"])) {
        return false;
    }

    $sqlCols = "INSERT INTO interactions ";
    $sqlCols .= "(metricName,roundID,initiatingPlayerNum,actionTime";

    $sqlVals = "VALUES (";
    $sqlVals .= "'" . $interactionInfo["name"] . "',";
    $sqlVals .= "'" . $roundID . "',";
    $sqlVals .= "'" . $interactionInfo["player"] . "',";
    $sqlVals .= "'" . $interactionInfo["time"] . "'";

    if(isset($interactionInfo["x"])) {
        $sqlCols.= ",actionXPos";
        $sqlVals .= ",'" . strval($interactionInfo["x"]) . "'";
    }

    if(isset($interactionInfo["y"])) {
        $sqlCols.= ",actionYPos";
        $sqlVals .= ",'" . strval($interactionInfo["y"]) . "'";
    }

    if(isset($interactionInfo["data"])) {
        $sqlCols.= ",actionSpecificData";
        $sqlVals .= ",'" . $interactionInfo["data"] . "'";
    }

    $sqlCols .= ")";
    $sqlVals .= ")";

    //echo $sqlCols . $sqlVals . "\n";
    $result = mysqli_query($db, $sqlCols . $sqlVals);

    return $result;
}

//Try add a new user to the DB
function insert_user($username, $name, $email, $password1, $password2) {
  global $db;

  if(!isset($username) || !isset($name) || !isset($email) || !isset($password1) || !isset($password2)) {
      return "not all data set";
  }

  if($password1 != $password2) {
      return "pass";
  }

  $sql = "SELECT * FROM users ";
  $sql .= "WHERE username='" . $username . "' ";
  $sql .= "OR email='" . $email . "' LIMIT 1";
  $result = mysqli_query($db, $sql);
  confirm_result_set($result);
  $user = mysqli_fetch_assoc($result);
  mysqli_free_result($result);

  if($user) {
    if ($user['username'] === $username) {
      return "username";
    }

    if ($user['email'] === $email) {
      return "email";
    }
  }

  $password = password_hash($password1, PASSWORD_BCRYPT);

  $sql = "INSERT INTO users ";
  $sql .= "(name, username, email, password) ";
  $sql .= "VALUES (";
  $sql .= "'" . $name . "',";
  $sql .= "'" . $username . "',";
  $sql .= "'" . $email . "',";
  $sql .= "'" . $password . "'";
  $sql .= ")";
  $result = mysqli_query($db, $sql);
  // For INSERT statements, $result is true/false
  //return $result;
  if($result) {
    return "ok";
  } else {
    // INSERT failed
    //echo mysqli_error($db);
    error_404();
    db_disconnect($db);
    exit;
  }
}


 ?>
