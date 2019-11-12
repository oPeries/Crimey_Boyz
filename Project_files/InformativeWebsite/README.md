## Tutorial 3 presentation structure


###General: 
    MVC
    CodeIgniter Urls Structure https://codeigniter.com/user_guide/general/urls.html

    Structure of a codeigniter project, contoller, views, model
    Important directories:
        - application/config
        - application/controllers
        - application/models
        - application/views

###Demo: 
	- Basic configuration
	    -> application/config/config.php - set base_url
	    -> application/config/database.php - set connection detail
	    -> application/config/routes.php - set default route

	- Code migration for the front end: just a controller loading views, header, footer
	    -> 3 ways of using session variables
	        - session_start() in the contructor
	        - $this->load->library("session")
	        - do the same in autoload.php
	- Database access to complete the implementation of login
	    -> How to create the model, 
	    -> Use Database Reference $this->load->database();
	    -> 2 ways to query the data
	        - use simple query
	        - use query builder e.g get_where() https://www.codeigniter.com/userguide3/database/query_builder.html
	    -> How to use a model in controller.
	    -> Use Cookie helper (in autoload, identical to set_cookie, provide delete_cookie)
	- routing, getting rid of index.php, on an apache server (localhost) and nginx (zone)

###Additional Library: 
    Security Library: https://www.codeigniter.com/user_guide/libraries/security.html
    Shoping Cart Library: https://www.codeigniter.com/user_guide/libraries/cart.html
    Encryption Library: https://www.codeigniter.com/user_guide/libraries/encryption.html
    Email Library: https://www.codeigniter.com/user_guide/libraries/email.html

###Additional Helper:
    Helpers, as the name suggests, help you with tasks. Each helper file is simply a collection of functions in a particular category. There are URL Helpers, that assist in creating links, there are Form Helpers that help you create form elements, Text Helpers perform various text formatting routines, Cookie Helpers set and read cookies, File Helpers help you deal with files, etc.

        URL Helper
        Captcha Helper