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
        //er is in de highscores lijst zelf niet zichtbaar of dit van de afgelopen maand is, maar het zit wel in de query verwerkt.
        //de lijst word gesorteerd op winRatio/gamesPlayed in de eerste plaats,
        //omdat iemand met een hogere score met 50 games played natuurlijk hoger moet staan dan iemand die 100 games heeft gedaan maar maar 1 heeft gewonnen.
        /*In de les hadden we besloten dat dit vvv
         *Alleen van de afgelopen maand. (max 10 mensen)
         * |   1   |  Bob  |  WinRatio%  |  Average Movecount/Score  | Games Played |
         * |       |       |             |                           |              |
         * voldoet aan de eisen.
         * uiteindelijk ben ik gesettled op max 5 mensen ipv max 10, voor de retro vibe :p
        */
        $query = "SELECT u.username, ROUND(AVG(s.won), 2) AS winRatio, ROUND(SUM(IF(s.won,s.score,0))/SUM(IF(s.won,1,0)), 2) AS averageScore, COUNT(*) AS gamesPlayed FROM scores s INNER JOIN users u ON (u.id = s.user_id)
                  WHERE s.date > DATE_SUB(CURRENT_DATE, INTERVAL 1 MONTH) GROUP BY u.id ORDER BY (AVG(s.won)*COUNT(*)) DESC, gamesPlayed DESC LIMIT 5;";

        if (!($result = $mysqli->query($query)))
        {
            showerror($mysqli->errno,$mysqli->error);
        }
        else
        {
            $json = "{\"highscores\":[";
            $row = $result->fetch_assoc();
            
            do {
              $json .= json_encode($row);
              $json .= ",";//kommas worden niet automatisch toegevoegd door de encode als ze per rij doet
            } while ($row = $result->fetch_assoc());
            
            $json = substr($json, 0, -1);//omdat er anders 1 komma teveel is
            $json .= "]}";
            echo $json;
        }
    }
?>