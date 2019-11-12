<?php
function error_404() {
  header($_SERVER["SERVER_PROTOCOL"] . " 404 Not Found");
  include(SHARED_PATH . '/my_404.php');
  exit();
}

function error_500() {
  header($_SERVER["SERVER_PROTOCOL"] . " 500 Internal Server Error");
  exit();
}

function is_post_request() {
  return $_SERVER['REQUEST_METHOD'] == 'POST';
}

function is_get_request() {
  return $_SERVER['REQUEST_METHOD'] == 'GET';
}
?>
