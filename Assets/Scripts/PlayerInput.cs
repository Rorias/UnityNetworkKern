using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public TMP_Text winLoseText;
    public GameObject[] buttons;

    public ClickType type = ClickType.None;
    public bool turn = false;

    private MessageHandler mh;
    private Client c;

    private void Awake()
    {
        mh = FindObjectOfType<MessageHandler>();
        c = FindObjectOfType<Client>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && turn)
        {
            Vector2Int pos = MousePosToCellPos(Input.mousePosition);

            if (IsPosValid(pos))
            {
                IsPosAvailable(pos);
            }
        }
    }

    private Vector2Int MousePosToCellPos(Vector2 _mousePosition)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(_mousePosition);
        Vector2Int actualPos = new Vector2Int(Mathf.RoundToInt(worldPos.x / 2f) * 2, Mathf.RoundToInt(worldPos.y / 2f) * 2);

        return actualPos;
    }

    private bool IsPosValid(Vector2Int _gridPos)
    {
        bool valid = true;

        if (_gridPos.x > 2 || _gridPos.x < -2 || _gridPos.y > 2 || _gridPos.y < -2)
        {
            valid = false;
        }

        return valid;
    }

    private void IsPosAvailable(Vector2Int _gridPos)
    {
        MessageHandler.Message msg = new MessageHandler.Message()
        {
            cmd = MessageHandler.CommandType.PlaceTypeRequest,
            userId = DatabaseConnection.userData.id,
            pos = _gridPos.x + "," + _gridPos.y,
            type = type.ToString(),
        };

        c.SendToServer(msg);
    }

    public void EndGame(ClickType _type)
    {
        winLoseText.enabled = true;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(true);
        }

        if (type == _type)
        {
            winLoseText.text = "You win! :)";
        }
        else
        {
            winLoseText.text = "You lose! :(";
        }
    }

    public void PlayAgain()
    {
        c.SendToServer(mh.C_SendNewGameRequest());

        winLoseText.enabled = false;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
    }

    public void LeaveMatch()
    {
        c.Disconnect();
        SceneManager.LoadScene(0);
    }
}
