<?php
class data extends CI_Controller {
 
    public function __construct() {
        parent::__construct();
        $this->load->model('data_model');
        $this->load->helper(array('url_helper','form'));
        $this->load->library('form_validation');
    }
 
    public function index() {
        $data['data'] = $this->data_model->get_data(); 
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('data/index', $data);
        $this->load->view('footer');
    }
 
    public function view($slug = NULL) {
        $data['data_item'] = $this->data_model->get_data($slug);
        
        if (empty($data['data_item'])) {
            show_404();
        }
 
        $data['name'] = $data['data_item']['name'];
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('data/view', $data);
        $this->load->view('footer');
    }
    
    public function create() {
        if (!$this->session->userdata('logged_in')) {
            redirect(site_url('users/login'));
        } else {
            $data['user_id'] = $this->session->userdata('user_id');
        }
        $this->load->helper('form');
        $this->load->library('form_validation');
 
        $data['name'] = 'Create a menu item';
 
        $this->form_validation->set_rules('name', 'Name', 'required');
        $this->form_validation->set_rules('description', 'Description', 'required');
 
        if ($this->form_validation->run() === FALSE) {
            $data['page'] = 'basic';

            $this->load->view('header', $data);
            $this->load->view('data/create', $data);
            $this->load->view('footer');
        } else {
            $this->data_model->set_data();
            $data['page'] = 'basic';

            $this->load->view('header', $data);
            $this->load->view('data/success', $data);
            $this->load->view('footer');
        }
    }
    
    public function edit() {
        if (!$this->session->userdata('logged_in')) {
            redirect(site_url('researcher/login'));
        } else {
            $data['user_id'] = $this->session->userdata('user_id');
        }
        $id = $this->uri->segment(3);
        
        if (empty($id)) {
            show_404();
        }
        
        $this->load->helper('form');
        $this->load->library('form_validation');
        
        $data['name'] = 'Edit a menu item';        
        $data['data_item'] = $this->data_model->get_data_by_id($id);

        $this->form_validation->set_rules('name', 'Name', 'required');
        $this->form_validation->set_rules('description', 'Description', 'required');
 
        if ($this->form_validation->run() === FALSE) {
            $data['page'] = 'basic';

            $this->load->view('header', $data);
            $this->load->view('data/edit', $data);
            $this->load->view('footer');
        } else {
            $this->data_model->set_data($id);
            $this->load->view('data/success');
            redirect( base_url() . 'data');
        }
    }
    
    public function search() {
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('search-form');
        $this->load->view('footer');
    }
    
    public function execute_search($slug = NULL) {
        // Retrieve the posted search term.
        $search_term = $this->input->post('search');

        // Use a model to retrieve the results.
        $data['results'] = $this->data_model->get_results($search_term);
        $data['val'] = $this->data_model->get_data($slug);
        $data['page'] = 'basic';
        // Pass the results to the view.

        $this->load->view('header', $data);
        $this->load->view('search-results', $data);
        $this->load->view('footer');
         
    }
    
    public function exportCSV(){ 
        // file name 
        $filename = 'data_'.date('Ymd').'.csv'; 
        header("Content-Description: File Transfer"); 
        header("Content-Disposition: attachment; filename=$filename"); 
        header("Content-Type: application/csv; ");

        // get data 
        $gameData = $this->data_model->get_data();

        // file creation 
        $file = fopen('php://output', 'w');
        
        $header = array("ID Number","User ID","Number of Collisions","Score"); 
        fputcsv($file, $header);
        foreach ($gameData as $key=>$line){ 
         fputcsv($file,$line); 
        }
        fclose($file); 
        exit; 
  }
    
    public function delete() {
        if (!$this->session->userdata('logged_in')) {
            redirect(site_url('users/login'));
        }
        $id = $this->uri->segment(3);
        
        if (empty($id)) {
            show_404();
        }
                
        $data_item = $this->data_model->get_data_by_id($id);
        
        if ($data_item['user_id'] != $this->session->userdata('user_id')) {
            $currentClass = $this->router->fetch_class(); // class = controller
            redirect(site_url($currentClass));
        }
        
        $this->data_model->delete_data($id);        
        redirect( base_url() . '/data');        
    }
}