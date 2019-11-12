<?php
class researcher_model extends CI_Model {
 
    public function __construct() {
        $this->load->database();
    }
    
    public function get_users($username = FALSE) {
        if ($username === FALSE) {
            $query = $this->db->get('researcher');
            return $query->result_array();
        }
 
        $query = $this->db->get_where('researcher', array('username' => $username));
        return $query->row_array();
    }

    public function insert_user($data){
    $con = mysqli_connect("localhost","root","d8ae3c166cab39bd","registration");
    
    if (mysqli_connect_errno()) {
      echo "Failed to connect to MySQL: " . mysqli_connect_error();
    }
    
    if (isset($_POST['reg_user'])) {
        $username = mysqli_real_escape_string($con,$this->input->post('username'));
        $email = mysqli_real_escape_string($con, $this->input->post('email'));
        $password_1 = mysqli_real_escape_string($con, $this->input->post('password_1'));
        $password_2 = mysqli_real_escape_string($con, $this->input->post('password_2'));
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

      $user_check_query = "SELECT * FROM researcher WHERE username=? OR email=? LIMIT 1";
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
        $sql = "INSERT INTO researcher (name, username, email, password, cuisine, address) VALUES (?, ?, ?, ?, ?, ?)";
        $dbResult = $this->db->query($sql, array($this->input->post('name'), $this->input->post('username'), $this->input->post('email'), $password, $this->input->post('cuisine'), $this->input->post('address')));
        $_SESSION['username'] = $username;
        $_SESSION['type'] = "partner";
        $_SESSION['success'] = "You are now logged in";
        return true;
      } 
    }
  }
    
    public function check_user($username, $password) {
        $con = mysqli_connect("localhost","root","d8ae3c166cab39bd","registration");
        
        if (mysqli_connect_errno()) {
          echo "Failed to connect to MySQL: " . mysqli_connect_error();
          }
        
        if (isset($_POST['login_user'])) {
            $username = mysqli_real_escape_string($con, $_POST['username']);
            $password = mysqli_real_escape_string($con, $_POST['password']);
            $errors = array(); 

          if (empty($username)) {
            array_push($errors, "Username is required");
          }
          if (empty($password)) {
            array_push($errors, "Password is required");
          }

          if (count($errors) == 0) {
              $sql = "SELECT * FROM researcher WHERE username = ? LIMIT 1";
              $result = $this->db->query($sql, array($this->input->post('username')));
              $num = $result->num_rows();
              if ($num == 1) {
                $row=$result->row_array();
                if(password_verify($password, $row['password'])){
                    $_SESSION['username'] = $username;
                    $_SESSION['type'] = "partner";
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