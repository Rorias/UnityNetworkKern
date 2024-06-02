using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class ServerUserEdit : MonoBehaviour
{
    private Server server;

    private MessageHandler.Message updateMsg;
    private MessageHandler.Message returnMsg;

    public void UpdateUser(Server _server, MessageHandler.Message _update)
    {
        server = _server;
        updateMsg = _update;
        returnMsg = new MessageHandler.Message() { cmd = MessageHandler.CommandType.UpdateUserAccept };
        string url = "https://studenthome.hku.nl/~lars.mulder/user_edit.php?sessid=" + DatabaseConnection.sessionId.result + "&id=" + updateMsg.userId;

        if (!string.IsNullOrWhiteSpace(updateMsg.email))
        {
            url += "&email=" + updateMsg.email;
            returnMsg.email = updateMsg.email;
        }

        if (!string.IsNullOrWhiteSpace(updateMsg.usrname))
        {
            url += "&user=" + updateMsg.usrname;
            returnMsg.usrname = updateMsg.usrname;
        }

        if (!string.IsNullOrWhiteSpace(updateMsg.bdate))
        {
            url += "&bdate=" + updateMsg.bdate;
            returnMsg.bdate = updateMsg.bdate;
        }

        if (!string.IsNullOrWhiteSpace(updateMsg.pass))
        {
            url += "&pw=" + updateMsg.pass;
            returnMsg.pass = updateMsg.pass;
        }

        StartCoroutine(UpdateUser(url));
    }

    private IEnumerator UpdateUser(string _url)
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
                    if (!string.IsNullOrWhiteSpace(webRequest.downloadHandler.text))
                    {
                        int err = JsonUtility.FromJson<ServerResult>(webRequest.downloadHandler.text.Substring(1)).error;
                        Debug.Log(DatabaseConnection.ErrorMessage(err));

                        if (err == 1)
                        {
                            server.SendToClientId(updateMsg.userId, returnMsg);
                        }
                        else if (err == 3)
                        {
                            GetComponent<ServerRelogin>().ReloginToServer();
                        }
                        else
                        {
                            MessageHandler.Message msg = new MessageHandler.Message()
                            {
                                cmd = MessageHandler.CommandType.Error,
                                message = DatabaseConnection.ErrorMessage(err),
                            };

                            server.SendToClientConn512(updateMsg.connId, msg);
                        }
                    }
                    break;
            }
        }
    }
}
