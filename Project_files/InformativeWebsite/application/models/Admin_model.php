<?php
class admin_model extends CI_Model {

    public function __construct() {
        $this->load->database();
    }

    public function get_data($slug = FALSE) {
        if ($slug === FALSE) {
            $query = $this->db->get('data');
            return $query->result_array();
        }

        $query = $this->db->get_where('data', array('slug' => $slug));

        return $query->row_array();
    }

    //Get the total number of expressions of interests
    public function get_total_page_visits() {

        $sql = "SELECT website_pages.page_name AS page, COUNT(*) AS visits ";
        $sql .= "FROM page_visits, website_pages WHERE website_pages.id = page_visits.page_visited ";
        $sql .= "GROUP BY website_pages.page_name";
        $result = $this->db->query($sql);

        return $result->result_array();
    }

    //Get the total number of expressions of interests
    public function get_all_page_visits() {

        $sql = "SELECT website_pages.page_name, page_visits.visit_time FROM page_visits, website_pages WHERE page_visits.page_visited = website_pages.id";
        $result = $this->db->query($sql);

        return $result->result_array();

    }

    public function get_total_interest_clicks() {

        $sql = "SELECT COUNT(*) FROM express_interest_clicks";
        $result = $this->db->query($sql);

        return $result->row_array()['COUNT(*)'];

    }

    //Get the total number of expressions of interests
    public function get_total_interests_submissions() {

        $sql = "SELECT COUNT(*) FROM expressions_of_interest";
        $result = $this->db->query($sql);

        return $result->row_array()['COUNT(*)'];

    }

    public function get_all_submissions_of_interest() {

        $sql = "SELECT expressions_of_interest.submission_time, expressions_of_interest.name, expressions_of_interest.email, ";
        $sql .= "age_ranges.age_range, countries.country_name, expressions_of_interest.early_access, expressions_of_interest.comment ";


        $sql .= "FROM expressions_of_interest, age_ranges, countries ";
        $sql .= "WHERE expressions_of_interest.age_range = age_ranges.id AND expressions_of_interest.country = countries.id";

        $result = $this->db->query($sql);

        return $result->result_array();

    }

    public function get_submission_age_ranges() {

        $sql = "SELECT age_ranges.age_range, COUNT(*) as count FROM age_ranges, expressions_of_interest ";
        $sql .= "WHERE age_ranges.id = expressions_of_interest.age_range GROUP BY age_ranges.age_range ";
        $sql .= "ORDER BY count DESC";
        $result = $this->db->query($sql);
        return $result->result_array();

    }

    public function get_submission_countries() {

        $sql = "SELECT countries.country_name, COUNT(*) as count FROM countries, expressions_of_interest ";
        $sql .= "WHERE countries.id = expressions_of_interest.country GROUP BY countries.country_name ";
        $sql .= "ORDER BY count DESC";
        $result = $this->db->query($sql);
        return $result->result_array();

    }

}
