<?php
class researcher extends CI_Controller {
 
    public function __construct() {
        parent::__construct();
        $this->data['status'] = "";
        $this->load->model('researcher_model');
        $this->load->helper('form', 'url');
        $this->load->library('form_validation', 'session', 'cookie');
    }
 
    public function index() {
        $username = $_SESSION['username'];
        $data['users_data'] = $this->researcher_model->get_users($username);
        $data['error'] = '';
        $data['page'] = 'basic';

        if (empty($data['users_data'])) {
            show_404();
        }

        $this->load->view('header', $data);
        $this->load->view('adminprofile', $data);
        $this->load->view('footer');
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
                
        if ($this->form_validation->run() === FALSE) {
            $data['page'] = 'basic';            
            $this->load->view('header', $data);
            $this->load->view('researcher-register');
            $this->load->view('footer');
        } else {  
            $data = array(
                'name' => $this->input->post('name'),
                'username' => $this->input->post('username'),
                'email' => $this->input->post('email'),
                'favfood' => $this->input->post('institution'),
                'address' => $this->input->post('field'),
                'password_1' => $this->input->post('password_1'),
                'password_2' => $this->input->post('password_2')
            );
            if($this->researcher_model->insert_user($data) === TRUE) {
                $this->session->set_userdata('logged_in', true);
                redirect(base_url() . "researcherProfile/");
            }
        }
    }

    public function login() {
        $this->load->helper('form');
        $this->load->library('form_validation', 'cookie');
        $this->form_validation->set_rules('username', 'Username', 'required');
        $this->form_validation->set_rules('password', 'Password', 'required');
        
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
        $check_login = $this->researcher_model->check_user($username, $password);
 
        //if the result is query result is 1 then valid user
        if ($check_login) {
            //if yes then set the session 'loggin_in' as trueFset
            $this->session->set_userdata('logged_in', true);
            redirect(base_url() . "researcherProfile/");
        } else {
            //if no then set the session 'logged_in' as false
            $this->session->set_userdata('logged_in', false);
            $data = array(
                'error_message' => 'Invalid Username or Password'
            );

            $data['page'] = 'basic';
            
            $this->load->view('header', $data);
            $this->load->view('researcher-login', $data);
            $this->load->view('footer');            
        } 
    }
    
    public function logout() {    
        if ($this->session->userdata('logged_in')) {
            $this->session->unset_userdata('logged_in');
            session_destroy();
        }
        redirect(base_url() . "home/");
    }    
}