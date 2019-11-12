<?php
defined('BASEPATH') OR exit('No direct script access allowed');


class Users extends CI_Controller {
    
    public function __construct() {
        parent::__construct();
        $this->data['status'] = "";
        $this->load->model('users_model');
        $this->load->helper('form');
        $this->load->library('form_validation', 'session', 'cookie');
    }

    public function index() {
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('userprofile');
        $this->load->view('footer');
    }

    public function login() {
        $this->load->helper('form');
        $this->load->library('form_validation', 'cookie');
        $this->form_validation->set_rules('username', 'Username', 'required');
        $this->form_validation->set_rules('password', 'Password', 'required');
        
        //get the input fields from login form
        $username = $this->input->post('username');
        $password = $this->input->post('password'); 
        $rememberme = $this->input->post('remember-me');
        
        if ($rememberme) {
            setcookie("username", $this->input->post('username'), time() + 60*60*24, "/");  
            setcookie("password", $this->input->post('password'), time() + 60*60*24, "/");
        } else {
            delete_cookie('username');
            delete_cookie('password');
        }
                    
        //send the email pass to query if the user is present or not
        $check_login = $this->users_model->check_user($username, $password);
 
        //if the result is query result is 1 then valid user
        if ($check_login) {
            //if yes then set the session 'loggin_in' as true
            $this->session->set_userdata('logged_in', true);
            redirect(base_url() . "UserProfile/"); 
        } else {
            //if no then set the session 'logged_in' as false
            $this->session->set_userdata('logged_in', false);
            $data = array(
                'error_message' => 'Invalid Username or Password'
            );
            
             $data['page'] = 'basic';

            $this->load->view('header', $data);
            $this->load->view('login', $data);
            $this->load->view('footer');            
        }
    }
        
    public function register() {
        $this->load->helper('form');
        $this->load->library('form_validation');
        $this->form_validation->set_rules(
            'username', 'Username',
            'required|min_length[5]|max_length[12]|is_unique[users.username]',
            array(
                'required'      => 'You have not provided %s.',
                'is_unique'     => 'This %s already exists.'
            )
        );
        $this->form_validation->set_rules('password_1', 'Password', 'required');
        $this->form_validation->set_rules('password_2', 'Password Confirmation', 'required|matches[password_1]');
        $this->form_validation->set_rules('email', 'Email', 'required|valid_email|is_unique[users.email]');
        if ($this->form_validation->run() == FALSE) {
             $data['page'] = 'basic';

            $this->load->view('header', $data);
            $this->load->view('register');
            $this->load->view('footer');
        } else {
            $data = array(
                'name' => $this->input->post('name'),
                'username' => $this->input->post('username'),
                'email' => $this->input->post('email'),
                'address' => $this->input->post('address'),
                'password_1' => $this->input->post('password_1'),
                'password_2' => $this->input->post('password_2')
            );
            if($this->users_model->insert_user($data) === TRUE) {
                $this->session->set_userdata('logged_in', true);
                redirect(base_url() . "UserProfile/");
            }
        }
    }

    public function logout() {
        session_destroy();
        redirect(base_url() . "home/");
    }
}
