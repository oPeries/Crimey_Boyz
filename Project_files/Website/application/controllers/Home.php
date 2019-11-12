<?php
defined('BASEPATH') OR exit('No direct script access allowed');

class Home extends CI_Controller {
    public function __construct() {
        parent::__construct();
        $this->load->helper(['url', 'form']);
        $this->load->database();
        $this->load->library(['session', 'zip']);
    }

    public function index() {
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('home');
        $this->load->view('footer');
    }

    public function createzip() {
        $this->load->helper('download');

        // Read files from directory
        if ($this->input->post('download') != null) {
            // File name
            $file = FCPATH.'/uploads/CrimeyBoyzGame.zip';

            // Download
            force_download($file, null);
        }

        // Load view
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('home');
        $this->load->view('footer');
    }
}
