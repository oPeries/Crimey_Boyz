<?php
class data_model extends CI_Model {

    public function __construct() {
        $this->load->database();
    }

    public function get_data($slug = FALSE) {
        if ($slug === FALSE) {
            $this->db->select('*');
            $this->db->from('users');

            return $this->db->get()->result_array();
        }

        //$query = $this->db->get_where('data', array('slug' => $slug));
        //return $query->row_array();
    }

    public function get_whole_table($tableName) {
        if(!isset($tableName)) {
            return null;
        }

        $sql = "SELECT * FROM ";
        $sql .= $tableName;
        $result = $this->db->query($sql);

        return $result->result_array();
    }

    public function get_table_columns($tableName) {
        if(!isset($tableName)) {
            return null;
        }

        $sql = "SHOW COLUMNS FROM ";
        $sql .= $tableName;
        $result = $this->db->query($sql);

        $arr = $result->result_array();

        $result = array();
        foreach($arr as $row) {
            $result[] = $row["Field"];
        }

        return $result;
    }
}
