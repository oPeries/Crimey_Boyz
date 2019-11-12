<?php
class Express_interest_model extends CI_Model {
    public function __construct() {
        $this->load->database();
    }

    public function get_age_ranges() {

        $sql = "SELECT * FROM age_ranges ORDER BY id ASC;";
        $query = $this->db->query($sql);

        return $query->result_array();

    }

    public function get_countries() {

        $sql = "SELECT * FROM countries ORDER BY country_name ASC;";
        $query = $this->db->query($sql);

        return $query->result_array();

    }


    public function insert_expression_of_interest($data) {
        if (isset($_POST['express_interest'])) {
            $sql = "INSERT INTO expressions_of_interest (submission_time, name, email, age_range, country, early_access, comment) VALUES (?, ?, ?, ?, ?, ?, ?)";
            $time = mdate('%Y-%m-%d %H:%i:%s', time());
            $query = $this->db->query($sql, [$time, $data['name'], $data['email'], $data['age'], $data['country'], $data['early_access'], $data['comment']]);
        } else {
            echo 'not set';
        }
    }

    //To be called when the user hits the "Express Interest In Crimey Boyz" button on the home page
    public function count_interest_clicks() {
        if (isset($_POST['count_click'])) {

            //Save this visit in the DB
            $sql = "INSERT INTO express_interest_clicks (click_time) VALUES (?);";
            $this->db->query($sql, [mdate('%Y-%m-%d %H:%i:%s', time())]);
        }
    }
}
