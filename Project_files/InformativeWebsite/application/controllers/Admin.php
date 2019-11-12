<?php
class admin extends CI_Controller {

    public function __construct() {
        parent::__construct();
        $this->load->model(['visits_model','admin_model']);
        $this->load->helper(array('url_helper','form','date'));
        $this->load->library('form_validation');
    }

    public function index() {
        $this->visits_model->count_page_visit();

        $data['page'] = 'basic';

        $data['total_visits'] = $this->admin_model->get_total_page_visits();
        $data['total_interest_clicks'] = $this->admin_model->get_total_interest_clicks();
        $data['total_interest_submissions'] = $this->admin_model->get_total_interests_submissions();

        $data['all_interest_submissions'] = $this->admin_model->get_all_submissions_of_interest();
        $data['submission_age_ranges'] = $this->admin_model->get_submission_age_ranges();
        $data['submission_countries'] = $this->admin_model->get_submission_countries();

        $data['all_visits'] =  $this->admin_model->get_all_page_visits();

        $this->load->view('header', $data);
        $this->load->view('admin/index', $data);
        $this->load->view('footer');
    }

    public function exportCSV() {
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
}
