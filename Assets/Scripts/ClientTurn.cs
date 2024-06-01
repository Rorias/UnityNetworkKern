using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ClientTurn : MonoBehaviour
{
    public GameObject prefabCross;
    public GameObject prefabCircle;

    public List<GameObject> objects = new List<GameObject>();

    public void Restart()
    {
        for (int i = objects.Count - 1; i >= 0; i--)
        {
            Destroy(objects[i]);
        }

        objects.Clear();
    }

    public void ShowTurn(Vector2Int _turnPosition, ClickType _type)
    {
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
}
