<?php
defined('BASEPATH') OR exit('No direct script access allowed');

//Controller for the informative website home page
class Home extends CI_Controller {
    public function __construct() {
        parent::__construct();
        $this->data['status'] = "";
        $this->load->model(['visits_model', 'express_interest_model']);
        $this->load->helper(['url','date', 'security','form']);
        $this->load->library(['session','form_validation']);
    }

    public function index() {
        $data['page'] = 'basic';

      //  $this->load->view('header', $data);
        //$this->load->view('home');
        //$this->load->view('footer');

        $this->visits_model->count_page_visit();

        $this->form_validation->set_rules('name', 'name', 'required|xss_clean');
        $this->form_validation->set_rules('email', 'email', 'required|xss_clean');
        $this->form_validation->set_rules('age', 'age', 'required|xss_clean');
        $this->form_validation->set_rules('country', 'country', 'required|xss_clean');
        $this->form_validation->set_rules('comment', 'comment', 'xss_clean');

        $showForm = TRUE;
        if (isset($_POST['count_click'])) {
            $this->express_interest_model->count_interest_clicks(); //if clicked the "express interest button on the home page"

        } else if(isset($_POST['express_interest'])) {
            $showForm = ($this->form_validation->run() === FALSE);
            echo 'post';
        }

        if ($showForm) {
            $data['ages'] = $this->express_interest_model->get_age_ranges();
            $data['countries'] = $this->express_interest_model->get_countries();

            $this->load->view('header', $data);
            $this->load->view('home', $data);
            $this->load->view('footer');

            $this->visits_model->count_page_visit();

        } else {
            $form = array(
                'name' => $this->input->post('name'),
                'email' => $this->input->post('email'),
                'age' => $this->input->post('age'),
                'country' => $this->input->post('country'),
                'early_access' => $this->input->post('early_access') === null ? '0' : '1',
                'comment' => $this->input->post('comment')
            );
            $this->express_interest_model->insert_expression_of_interest($form);
            redirect(base_url('/expressinterest/thankyou'));
        }
    }
}
