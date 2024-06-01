using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class ServerGetHighscores : MonoBehaviour
{
    private Server server;
    private MessageHandler.Message incoming;

    public void GetHighscores(Server _server, MessageHandler.Message _incoming)
    {
        server = _server;
        incoming = _incoming;
        string url = "https://studenthome.hku.nl/~lars.mulder/statistics.php?" + "sessid=" + DatabaseConnection.sessionId.result;

        StartCoroutine(GetHighscores(url));
    }

    private IEnumerator GetHighscores(string _url)
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
                    Highscores hs = JsonUtility.FromJson<Highscores>(webRequest.downloadHandler.text.Substring(1));
                    Debug.Log(DBErrorHandler.ErrorMessage(hs.error));

                    if (hs.error == 1)
                    {
                        MessageHandler.Message msg = new MessageHandler.Message()
                        {
                            connId = incoming.connId,
                            cmd = MessageHandler.CommandType.HighscoreAccept,
                            userId = incoming.userId,
                            type = webRequest.downloadHandler.text.Substring(1),
                        };

                        server.SendToClientConn4096(msg.connId, msg);
                    }
                    else
                    {
                        //send error message to client?
                    }
                    break;
            }
        }


    }
}
