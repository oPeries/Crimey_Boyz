<?php
class Users_model extends CI_Model {

	public function __construct() {
		$this->load->database();
	}
    
  public function insert_user($data){
    if (isset($_POST['reg_user'])) {
        $username = $this->db->escape_str($this->input->post('username'));
        $email = $this->db->escape_str($this->input->post('email'));
        $password_1 = $this->db->escape_str($this->input->post('password_1'));
        $password_2 = $this->db->escape_str($this->input->post('password_2'));
        $errors = array();

      if (empty($username)) { 
        array_push($errors, "Username is required"); 
      }
      if (empty($email)) { 
        array_push($errors, "Email is required"); 
      }
      if (empty($password_1)) { 
        array_push($errors, "Password is required"); 
      }
      if ($password_1 != $password_2) {
        array_push($errors, "The two passwords do not match");
      }


      $user_check_query = "SELECT * FROM users WHERE username=? OR email=? LIMIT 1";
      $result = $this->db->query($user_check_query, array($this->input->post('username'), $this->input->post('email')));
      $user = $result->row_array();

      if ($user) { 
        if ($user['username'] === $username) {
          array_push($errors, "Username already exists");
        }

        if ($user['email'] === $email) {
          array_push($errors, "Email already exists");
        }
      }

      if (count($errors) == 0) {
        $password = password_hash($this->input->post('password_1'), PASSWORD_BCRYPT);//encrypt the password before saving in the database using https://www.php.net/manual/en/function.password-hash.php
        $sql = "INSERT INTO users (name, username, email, password) VALUES (?, ?, ?, ?)";
        $dbResult = $this->db->query($sql, array($this->input->post('name'), $this->input->post('username'), $this->input->post('email'), $password));
        $_SESSION['username'] = $username;
        $_SESSION['type'] = "user";
        $_SESSION['success'] = "You are now logged in";
        return true;
      } 
    }
  }
    
  public function check_user($username, $password) {
    if (isset($_POST['login_user'])) {
        $username = $this->db->escape_str($this->input->post('username'));
        $password = $this->db->escape_str($this->input->post('password'));
        $errors = array();

      if (empty($username)) {
        array_push($errors, "Username is required");
      }
      if (empty($password)) {
        array_push($errors, "Password is required");
      }

      if (count($errors) == 0) {
        $sql = "SELECT * FROM users WHERE username = ? LIMIT 1";
        $result = $this->db->query($sql, array($this->input->post('username')));
        $num = $result->num_rows();        
        if ($num == 1) {
            $row=$result->row_array();
            if(password_verify($password, $row['password'])){
                $_SESSION['username'] = $username;
                $_SESSION['type'] = "user";
                $_SESSION['success'] = "You are now logged in";
                return true;
            } else {
                array_push($errors, "Wrong username/password combination");
                return false;
            } 
        } else {
            return false;
        }       
      }
    }
  }
}   
