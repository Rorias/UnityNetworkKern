using System;
using System.Linq;

using UnityEngine;

public enum ClickType { None, Circle, Cross = -1 };

public class ServerTurn : MonoBehaviour
{
    private ClickType[,] taken = new ClickType[3, 3];

    public void Restart()
    {
        for (int r = 0; r < taken.GetLength(0); r++)
        {
            for (int c = 0; c < taken.GetLength(1); c++)
            {
                taken[r, c] = ClickType.None;
            }
        }
    }

    public bool TryTurn(string _turnPosition)
    {
        Vector2Int pos = new Vector2Int(Convert.ToInt32(_turnPosition.Split(',')[0]), Convert.ToInt32(_turnPosition.Split(',')[1]));
        Vector2Int arrayPosition = new Vector2Int((pos.x / 2) + 1, (pos.y / 2) + 1);

        if (taken[arrayPosition.x, arrayPosition.y] == ClickType.None)
        {
            return true;
        }

        return false;
    }

    public int DoTurn(Vector2Int _turnPosition, ClickType _type)
    {
        Vector2Int arrayPosition = new Vector2Int((_turnPosition.x / 2) + 1, (_turnPosition.y / 2) + 1);
        taken[arrayPosition.x, arrayPosition.y] = _type;
        Server.users.First(x => x.type == _type).score -= 1;

        return CheckEndCondition();
    }

    private int CheckEndCondition()
    {
        int[] results = new int[8];

        for (int r = 0; r < taken.GetLength(0); r++)
        {
            results[0] += (int)taken[r, 0];
            results[1] += (int)taken[r, 1];
            results[2] += (int)taken[r, 2];
            results[3] += (int)taken[0, r];
            results[4] += (int)taken[1, r];
            results[5] += (int)taken[2, r];
            results[6] += (int)taken[2 - r, r];
            results[7] += (int)taken[2 - r, 2 - r];

        }

        if (results.Any(x => Mathf.Abs(x) == 3))
        {
            return 1;
        }

        bool full = true;

        for (int r = 0; r < taken.GetLength(0); r++)
        {
            for (int c = 0; c < taken.GetLength(1); c++)
            {
                if (taken[r, c] == ClickType.None)
                {
                    full = false;
                    break;
                }
            }
        }

        if (full) { return 2; }

        return 0;
    }
}
