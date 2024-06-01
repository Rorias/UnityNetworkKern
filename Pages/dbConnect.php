<?php
   $db_user = 'larsmulder';
   $db_pass = 'choo0Aichigh';
   $db_host = 'localhost';
   $db_name = 'larsmulder';

   /* Hieronder de lijst aan error returns voor alle andere pages die gebruikt en vertaald word door de Unity server.
    * 0: validation of variable failed.
    * 1: succeeded.
    * 2: value check failed.
    * 3: session is invalid.
    */

    /* Open a connection */
    $mysqli = new mysqli("$db_host","$db_user","$db_pass","$db_name");
    
    /* check connection */
    if ($mysqli->connect_errno) {
       echo "Failed to connect to MySQL: (" . $mysqli->connect_errno() . ") " . $mysqli->connect_error();
       exit();
    }
    
    function showerror($error,$errornr) {
        die("Error (" . $errornr . ") " . $error);
    }
?>