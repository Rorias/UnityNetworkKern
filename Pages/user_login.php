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

        $email = $_GET["email"];//je hoeft geen htmlspecialchars te gebruiken omdat de filter_var dit al regelt voor je.
        $password = htmlspecialchars($_GET["pw"]);

        //email is not an email
        if (!filter_var($email, FILTER_VALIDATE_EMAIL)) {
            $check = 0;
        }

        //password contains spaces
        if (strpos($password, ' ') !== false) {
            $check = 0;
        }

        //password is longer than 24 characters
        if (strlen($password) > 24) {//de 10 chars is om te zorgen dat mogelijke queries niet doorkomen
            $check = 0;
        }

        if ($check == 1)
        {
            $passsha = hash("sha256", $password);//gebruik de php hash/salt functie ipv deze

            $query = "SELECT password FROM users WHERE email = '$email' LIMIT 1";

            if (!($result = $mysqli->query($query))) 
            {
                showerror($mysqli->errno,$mysqli->error);
            }
            else
            {
                $row = $result->fetch_assoc();

                if ($passsha == $row["password"])
                {
                    $query = "SELECT id, username FROM users WHERE email = '$email' LIMIT 1";
                    if (!($result = $mysqli->query($query)))
                    {
                        showerror($mysqli->errno,$mysqli->error);
                    }
                    $row = $result->fetch_assoc();

                    $json .= json_encode($row);
                    echo $json;
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