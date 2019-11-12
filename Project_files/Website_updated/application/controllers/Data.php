<?php
class data extends CI_Controller {

    public function __construct() {
        parent::__construct();
        $this->load->model('data_model');
        $this->load->helper(array('url_helper','form'));
        $this->load->library(array('form_validation', 'zip'));
    }

    public function index() {
        if (!$this->session->userdata('logged_in')) {
            redirect(site_url('researcher/login'));
        } else {
            $data['user_id'] = $this->session->userdata('user_id');
        }
        $data['data'] = $this->data_model->get_data();
        $data['page'] = 'basic';

        $this->load->view('header', $data);
        $this->load->view('data/index', $data);
        $this->load->view('footer');
    }

    public function exportCSV(){
        $tables = array('sessions', 'rounds', 'interactions', 'metrics');//, 'table2', 'table3');

        // create each CSV file and add to the ZIP folder
        foreach($tables as $table) {

            $fileName = '/tmp/'.$table.'.csv';

            $file = fopen($fileName, "w");

            $haders = $this->data_model->get_table_columns($table);
            fputcsv($file, $haders);

            $data = $this->data_model->get_whole_table($table); // data maybe from MySQL to add to your CSV file

            // add your data to the CSV file
            foreach($data as $d) {
                fputcsv($file, $d);
            }
            fclose($file);

            $this->zip->read_file($fileName);

            // now delete this CSV file
            if(is_file($fileName)) {
                unlink($fileName);
            }
        }
        //Download the data
        $this->zip->download("researchData_".date('Ymd').".zip");
    }
}
