using TMPro;

using UnityEngine;

public class SetIP : MonoBehaviour
{
    public TMP_InputField ipIF;

    public void SetIPAddress()
    {
        DatabaseConnection.IP = ipIF.text;
    }
}
