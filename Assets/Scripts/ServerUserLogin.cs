using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class ServerUserLogin : MonoBehaviour
{
    private Server server;

    public void LoginAsUser(Server _server, MessageHandler.Message _login)
    {
        server = _server;
        string url = "https://studenthome.hku.nl/~lars.mulder/user_login.php?" + "sessid=" + DatabaseConnection.sessionId.result + "&email=" + _login.email + "&pw=" + _login.pass;

        StartCoroutine(LoginAsUser(url));
    }

    private IEnumerator LoginAsUser(string _url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = _url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    DatabaseConnection.userData = JsonUtility.FromJson<UserResult>(webRequest.downloadHandler.text.Substring(1));
                    break;
            }
        }

        Debug.Log(DatabaseConnection.ErrorMessage(DatabaseConnection.userData.error));

        if (DatabaseConnection.userData.error == 1)
        {
            MessageHandler.Message msg = new MessageHandler.Message()
            {
                cmd = MessageHandler.CommandType.LoginUserAccept,
                userId = DatabaseConnection.userData.id,
                connId = server.m_Connections.Length - 1,
                usrname = DatabaseConnection.userData.username,
            };

            server.SendToClientConn512(msg.connId, msg);
        }
        else if (DatabaseConnection.userData.error == 3)
        {
            GetComponent<ServerRelogin>().ReloginToServer();
        }
        else
        {
            MessageHandler.Message msg = new MessageHandler.Message()
            {
                cmd = MessageHandler.CommandType.Error,
                message = DatabaseConnection.ErrorMessage(DatabaseConnection.userData.error),
            };

            server.SendToClientConn512(server.m_Connections.Length - 1, msg);
        }
    }
}
