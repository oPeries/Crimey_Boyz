<?php
defined('BASEPATH') OR exit('No direct script access allowed');

//Controller for the informative website home page
class Custom404 extends CI_Controller {

    public function __construct() {
        parent::__construct();
    }

    public function index() {
        $data['page'] = 'basic';

        $this->output->set_status_header('404');
        $this->load->view('header', $data);
        $this->load->view('custom404');
        $this->load->view('footer');
    }
}
