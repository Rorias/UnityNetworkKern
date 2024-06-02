using System;

using TMPro;

using UnityEngine;

public class ClientHighscores : MonoBehaviour
{
    public GameObject prefabHighscore;

    public GameObject highscoresPage;
    public GameObject highscoresButton;
    public Transform highscoresHolder;

    public void OpenHighscoreList()
    {
        MessageHandler.Message msg = new MessageHandler.Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = MessageHandler.CommandType.HighscoreRequest,
            userId = DatabaseConnection.userData.id,
        };

        GetComponent<Client>().SendToServer(msg);

        highscoresPage.SetActive(true);
        highscoresButton.SetActive(false);
    }

    public void CloseHighscoreList()
    {
        for (int i = highscoresHolder.childCount - 1; i >= 1; i--)
        {
            Destroy(highscoresHolder.GetChild(i).gameObject);
        }

        highscoresButton.SetActive(true);
        highscoresPage.SetActive(false);
    }

    public void LoadHighscores(string _highscores)
    {
        Highscores hs = JsonUtility.FromJson<Highscores>(_highscores);

        for (int i = 0; i < hs.highscores.Length; i++)
        {
            GameObject hiScore = Instantiate(prefabHighscore, highscoresHolder);

            hiScore.transform.GetChild(0).GetComponent<TMP_Text>().text = (i + 1).ToString();
            hiScore.transform.GetChild(1).GetComponent<TMP_Text>().text = hs.highscores[i].username;
            hiScore.transform.GetChild(2).GetComponent<TMP_Text>().text = (Convert.ToSingle(hs.highscores[i].winRatio) * 100) + "%";
            hiScore.transform.GetChild(3).GetComponent<TMP_Text>().text = hs.highscores[i].averageScore + "m";
            hiScore.transform.GetChild(4).GetComponent<TMP_Text>().text = hs.highscores[i].gamesPlayed;
        }
    }
}
