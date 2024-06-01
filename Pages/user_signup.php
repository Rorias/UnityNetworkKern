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

        $email = $_GET["email"];
        $username = htmlspecialchars($_GET["user"]);
        $birthdate = htmlspecialchars($_GET["bdate"]);
        $password = htmlspecialchars($_GET["pw"]);

        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
            $check = 0;
        }

        if (strpos($username, ' ') !== false) {
            $check = 0;
        }

        if (strpos($birthdate, ' ') !== false) {
            $check = 0;
        }

        $date = date_parse_from_format("Y/m/d", $birthdate);
        if (!checkdate($date['month'], $date['day'], $date['year'])) {
            $check = 0;
        }

        if (strpos($password, ' ') !== false) {
            $check = 0;
        }

        if (strlen($password) > 24) {
            $check = 0;
        }

        if ($check == 1)
        {
            $query = "SELECT id FROM users WHERE email = '$email'";
            if (!($result = $mysqli->query($query))) 
            {
                showerror($mysqli->errno,$mysqli->error);
            }
            else
            {
                $row = $result->fetch_assoc();

                if (!isset($row["id"]))//$row["id"] != "" of $row["id"] != NULL
                {
                    $passsha = hash("sha256", $password);
                    $query = "INSERT INTO users(id, email, username, birthdate, password) VALUES (NULL,'$email','$username','$birthdate','$passsha')";
                    if (!($result = $mysqli->query($query)))
                    {
                        showerror($mysqli->errno,$mysqli->error);
                    }

                    echo "{\"error\":1}";
                }
                else
                {
                    echo "{\"error\":2}";
                }
            }
        }
        else
        {
            echo "{\"error\":0}";
        }
    } 
?>