using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class ClientUserEdit : MonoBehaviour
{
    public GameObject editButton;
    public GameObject editPage;

    public TMP_InputField emailIF;
    public TMP_InputField usernameIF;
    public TMP_InputField passwordIF;

    public void OpenUserEdit()
    {
        editButton.SetActive(false);
        emailIF.text = DatabaseConnection.user.email;
        usernameIF.text = DatabaseConnection.user.username;
        editPage.SetActive(true);
    }

    public void SaveUserEdit()
    {
        MessageHandler.Message msg = new MessageHandler.Message()
        {
            connId = DatabaseConnection.userData.connid,
            cmd = MessageHandler.CommandType.UpdateUserRequest,
            userId = DatabaseConnection.userData.id,
        };

        if (!string.IsNullOrWhiteSpace(emailIF.text) && emailIF.text != DatabaseConnection.user.email)
        {
            msg.email = emailIF.text;
        }

        if (!string.IsNullOrWhiteSpace(usernameIF.text) && usernameIF.text != DatabaseConnection.user.username)
        {
            msg.usrname = usernameIF.text;
        }

        if (!string.IsNullOrWhiteSpace(passwordIF.text) && passwordIF.text != DatabaseConnection.user.password)
        {
            msg.pass = passwordIF.text;
        }

        GetComponent<Client>().SendToServer(msg);
    }

    public void CloseUserEdit()
    {
        editPage.SetActive(false);
        editButton.SetActive(true);
    }
}
