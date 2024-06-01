<?php
    include "dbConnect.php";

    $check = 1;

    $serverId = $_GET["id"];//id moet altijd int zijn dus hoeft geen htmlspecialchars te doen
    $password = htmlspecialchars($_GET["pw"]);

    //server id is not an integer
    if (!filter_var($serverId, FILTER_VALIDATE_INT)) {
        $check = 0;
    }

    //password contains spaces
    if (strpos($password, ' ') !== false) {
        $check = 0;
    }

    if ($check == 1)
    {
        $passsha = hash("sha256", $password);

        $query = "SELECT server_id FROM servers WHERE server_id = $serverId AND server_password = '$passsha' LIMIT 1";

        if (!($result = $mysqli->query($query)))
        {
            showerror($mysqli->errno,$mysqli->error);
        }
        else
        {
            $row = $result->fetch_assoc();

            if ($row["server_id"] == $serverId)
            {
                session_start();
                $_SESSION["server_id"] = $serverId;

                echo "{\"result\":\"".session_id()."\"}";
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
?>