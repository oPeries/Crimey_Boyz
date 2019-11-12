<?php
class Visits_model extends CI_Model {
    public function __construct() {
        $this->load->database();
    }

    //To be called when a page is visited
    //
    public function count_page_visit() {
        //Get the current page
        $curPage = uri_string();

        if(!isset($_SESSION['page'])) {
            $_SESSION['page'] = '';
        }

        //do not recount if just a reload
        if ($_SESSION['page'] != $curPage) {
            //set current page as session variable
            $_SESSION['page'] = $curPage;

            //Get the ID of the page visited from the DB
            $sql = "SELECT id FROM website_pages WHERE uri=? LIMIT 1;";
            $result = $this->db->query($sql, [$this->db->escape_Str($curPage)]);
            $page = $result->row_array();

            //If there is a valid ID for this page in the database, save the visit to this page
            if ($page && isset($page['id'])) {
                $sql = "INSERT INTO page_visits (page_visited, visit_time) VALUES (?, ?);";
                $this->db->query($sql, [$page['id'], mdate('%Y-%m-%d %H:%i:%s', time())]);
            }
        }
    }
}
