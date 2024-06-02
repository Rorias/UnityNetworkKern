<?php
    include "dbConnect.php";

    $validSession = 1;

    if (isset($_GET['sessid'])) 
    {
        $sessid = htmlspecialchars($_GET['sessid']);
        session_id($sessid);
    }

    session_start();

    if (!isset($_SESSION["server_id"]) || $_SESSION["server_id"] == 0)
    {
        $validSession = 0;
	    echo "{\"error\":3}";
    }

    if ($validSession == 1)
    {
        $check = 1;
        $user_id = $_GET["user_id"];
        $score = $_GET["score"];
        $win = $_GET["win"];

        if (filter_var($user_id, FILTER_VALIDATE_INT) === false) {
            $check = 0;
        }

        if (filter_var($score, FILTER_VALIDATE_INT) === false) {
            $check = 0;
        }

        //Gebruik van NULL check omdat normale vergelijking "true" oplevert want "!filter_var(0) == true"
        if (filter_var($win, FILTER_VALIDATE_BOOLEAN, FILTER_NULL_ON_FAILURE) === NULL) {
            $check = 0;
        }

        if ($check == 1)
        {
            $query = "INSERT INTO scores(id, server_id, user_id, score, won) VALUES (NULL,".$_SESSION["server_id"].",$user_id,$score,$win)";

            if (!($result = $mysqli->query($query)))
            {
                showerror($mysqli->errno,$mysqli->error);
            }
            else
            {
                echo "{\"error\":1}";
            }
        }
        else
        {
            echo "{\"error\":0}";
        }
    }
?>