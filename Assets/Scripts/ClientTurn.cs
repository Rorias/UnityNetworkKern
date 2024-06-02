using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class ClientTurn : MonoBehaviour
{
    public TMP_Text winLoseText;
    public GameObject[] buttons;

    public GameObject prefabCross;
    public GameObject prefabCircle;

    public ClickType type { get; private set; } = ClickType.None;
    public bool turn { get; private set; } = false;

    private List<GameObject> objects = new List<GameObject>();

    private Client client;

    private void Awake()
    {
        client = FindObjectOfType<Client>();
    }

    public void Restart()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            Destroy(objects[i]);
        }

        objects.Clear();

        switch (type)
        {
            case ClickType.Circle:
                type = ClickType.Cross;
                break;
            case ClickType.Cross:
                type = ClickType.Circle;
                break;
            case ClickType.None:
            default:
                break;
        }
    }

    public void SetType(ClickType _type)
    {
        type = _type;
        turn = _type == ClickType.Circle;
    }

    public void ShowTurn(Vector2Int _turnPosition, ClickType _type)
    {
        turn = !turn;

        switch (_type)
        {
            default:
            case ClickType.None:
                break;
            case ClickType.Circle:
                GameObject circle = Instantiate(prefabCircle, new Vector3(_turnPosition.x, _turnPosition.y, 0), Quaternion.identity);
                objects.Add(circle);
                break;
            case ClickType.Cross:
                GameObject cross = Instantiate(prefabCross, new Vector3(_turnPosition.x, _turnPosition.y, 0), Quaternion.identity);
                objects.Add(cross);
                break;
        }
    }

    public void EndGame(ClickType _type, int _score)
    {
        winLoseText.enabled = true;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
        }

        if (type == _type)
        {
            winLoseText.text = "You win! :) \n" + _score + " points.";
        }
        else
        {
            winLoseText.text = "You lose! :( \n0 points.";
        }
    }

    public void TieGame()
    {
        winLoseText.enabled = true;
        winLoseText.text = "It's a tie!";

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
        }
    }

    public void PlayAgain()
    {
        MessageHandler.Message msg = new MessageHandler.Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = MessageHandler.CommandType.NewGameRequest,
            userId = DatabaseConnection.userData.id,
        };

        client.SendToServer(msg);

        winLoseText.enabled = false;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
    }
}
