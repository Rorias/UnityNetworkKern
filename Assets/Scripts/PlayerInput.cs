using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Client c;
    private ClientTurn ct;

    private void Awake()
    {
        c = FindObjectOfType<Client>();
        ct = FindObjectOfType<ClientTurn>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && ct.turn)
        {
            Vector2Int pos = MousePosToCellPos(Input.mousePosition);

            if (IsPosValid(pos))
            {
                IsPosAvailable(pos);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            c.Disconnect();
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
            connId = DatabaseConnection.userData.connid,
            cmd = MessageHandler.CommandType.PlaceTypeRequest,
            userId = DatabaseConnection.userData.id,
            pos = _gridPos.x + "," + _gridPos.y,
            type = ct.type.ToString(),
        };

        c.SendToServer(msg);
    }
}
