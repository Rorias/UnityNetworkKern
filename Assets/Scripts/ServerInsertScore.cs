using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class ServerInsertScore : MonoBehaviour
{
    private Server server;

    public void InsertScore(Server _server, int _id, int _score, int _win)
    {
        server = _server;

        string url = "https://studenthome.hku.nl/~lars.mulder/score_insert.php?" + "sessid=" + DatabaseConnection.sessionId.result + "&user_id=" + _id + "&score=" + _score + "&win=" + _win;

        StartCoroutine(InsertScore(url));
    }

    private IEnumerator InsertScore(string _url)
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
                    int err = JsonUtility.FromJson<ServerResult>(webRequest.downloadHandler.text.Substring(1)).error;
                    Debug.Log(DatabaseConnection.ErrorMessage(err));

                    if (err == 3)
                    {
                        GetComponent<ServerRelogin>().ReloginToServer();
                    }
                    else if (err != 1)
                    {
                        MessageHandler.Message msg = new MessageHandler.Message()
                        {
                            cmd = MessageHandler.CommandType.Error,
                            message = DatabaseConnection.ErrorMessage(err),
                        };

                        server.SendToClients(msg);
                    }
                    break;
            }
        }
    }
}
