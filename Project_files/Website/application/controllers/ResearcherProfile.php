<?php
defined('BASEPATH') OR exit('No direct script access allowed');

class researcherProfile extends CI_Controller {
    
    public function __construct() {
        parent::__construct();
        $this->logged_in();
        $this->load->model('researcher_model');
        $this->load->helper(array('url','form'));
        $this->load->library('form_validation', 'session', 'cookie', 'image_lib');
    }

    public function index() {
        $username = $_SESSION['username'];
        $data['users_data'] = $this->researcher_model->get_users($username);
        $data['error'] = '';
        $data['page'] = 'profile';

        if (empty($data['users_data'])) {
            show_404();
        }

        $this->load->view('header', $data);
        $this->load->view('adminprofile', $data);
        $this->load->view('footer');
    }
    
    public function do_upload() {
        $this->load->library('upload');
        $this->load->library('image_lib');

        $new_name = $_SESSION['username'];

        $config['file_name'] = $new_name;
        $config['upload_path'] = './uploads/';
        $config['allowed_types'] = 'png';
        $config['max_size']    = '0';
        $config['max_width']  = '0';
        $config['max_height']  = '0';
        $config['overwrite']= true;
        $config['remove_spaces'] = TRUE;
        $this->upload->initialize($config);

        if(!$this->upload->do_upload('userfile')){
            $error = array('error'=>$this->upload->display_errors());
            $data['page'] = 'profile';
            $username = $_SESSION['username'];
            $data['users_data'] = $this->researcher_model->get_users($username);

            $this->load->view('header', $data);
            $this->load->view('adminprofile', $error);
            $this->load->view('footer');
        } else {
            $data = $this->upload->data();
            $info = $this->upload->data();
            $config['image_library'] = 'gd2';
            $config['source_image'] = './uploads/'.$data["raw_name"].$data['file_ext'];
            $config['new_image'] = './uploads/'.$data["raw_name"].$data['file_ext'];
            $config['create_thumb'] = FALSE;
            $config['maintain_ratio'] = FALSE;
            $config['width']         = 200;
            $config['height']       = 200;

            $this->image_lib->initialize($config);

            $this->image_lib->resize();
            
            $data['img'] = base_url().'/uploads/'.$data["raw_name"].$data['file_ext'];
            $data['page'] = 'profile';

            $this->load->view('header', $data);
            $this->load->view('researcher_upload_success');
            $this->load->view('footer');
        }
    }
        
    function logged_in(){
        $logged_in = $this->session->userdata('logged_in');
        if(!isset($logged_in) || $logged_in != true){
            echo 'You need to login to access this page. <a href="../users/login">Login</a>';
            die();
        }
    }
}

