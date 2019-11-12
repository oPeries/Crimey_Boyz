<?php
class data_model extends CI_Model {
 
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
    
    public function get_data_by_id($id = 0) {
        if ($id === 0) {
            $query = $this->db->get('data');
            return $query->result_array();
        }
 
        $query = $this->db->get_where('data', array('id' => $id));
        return $query->row_array();
    }
    
    public function set_data($id = 0) {
        $this->load->helper('url');
 
        $slug = url_title($this->input->post('name'), '-', TRUE);
 
        $data = array(
            'name' => $this->input->post('name'),
            'slug' => $slug,
            'description' => $this->input->post('description')
        );
        
        if ($id == 0) {
            return $this->db->insert('data', $data); 
        } else {
            $this->db->where('id', $id);
            return $this->db->update('data', $data);
        }
    }
    
    public function get_results($search_term='default') {
        // Use the Active Record class for safer queries.
        $this->db->select('*');
        $this->db->from('data');
        $this->db->like('name',$search_term);

        $query = $this->db->get();

        return $query->result_array();
    }
    
    public function delete_data($id) {
        $this->db->where('id', $id);
        return $this->db->delete('data');
    }
}