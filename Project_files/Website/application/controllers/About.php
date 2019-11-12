
<?php
defined('BASEPATH') OR exit('No direct script access allowed');

class About extends CI_Controller {
    
    public function __construct() {
        parent::__construct();
        $this->load->helper('url');
        $this->load->database();
        $this->load->library('session');
    }
    
    public function index() {
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('about');
        $this->load->view('footer');
    }
}
