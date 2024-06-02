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

        $set = "";
        $userId = $_GET["id"];//filter var voor int ($userId) alleen htmlspecialchars gebruiken als het een variabele characterset kan zijn

        if (isset($_GET["email"]))
        {
            $email = $_GET["email"];

            if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
                $check = 0;
                echo "{\"error\":0}";
            }

            $set .= "email='$email'";
        }

        if ($check == 1 && isset($_GET["user"]))
        {
            $username = htmlspecialchars($_GET["user"]);

            if (strpos($username, ' ') !== false) {
                $check = 0;
                echo "{\"error\":0}";
            }

            if ($set != "")
            {
                $set .= ",";
            }

            $set .= "username='$username'";
        }

        if ($check == 1 && isset($_GET["bdate"]))
        {
            $birthdate = htmlspecialchars($_GET["bdate"]);

            if (strpos($birthdate, ' ') !== false) {
                $check = 0;
                echo "{\"error\":0}";
            }

            $date = date_parse_from_format("Y/m/d", $birthdate);
            if (!checkdate($date['month'], $date['day'], $date['year'])) {
                $check = 0;
                echo "{\"error\":0}";
            }

            if ($set != "")
            {
                $set .= ",";
            }

            $set .= "birthdate='$birthdate'";
        }

        if ($check == 1 && isset($_GET["pw"]))
        {
            $password = htmlspecialchars($_GET["pw"]);

            if (strpos($password, ' ') !== false) {
                $check = 0;
                echo "{\"error\":0}";
            }

            if (strlen($password) > 24) {
                $check = 0;
                echo "{\"error\":0}";
            }

            if ($set != "")
            {
                $set .= ",";
            }

            $passsha = hash("sha256", $password);
            $set .= "password='$passsha'";
        }

        if ($check == 1)
        {
            if ($set != "")
            {
                $query = "UPDATE users SET $set WHERE id=$userId";

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
                echo "{\"error\":2}";
            }
        }
    }
?>