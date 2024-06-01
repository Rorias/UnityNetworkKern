using System.Collections;

using UnityEngine;
using UnityEngine.Networking;

public class ServerInsertScore : MonoBehaviour
{
    public void InsertScore(int _id, int _score, int _win)
    {
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
                    Debug.Log(DBErrorHandler.ErrorMessage(err));

                    if (err != 1)
                    {
                        //send error message to client?
                    }
                    break;
            }
        }
    }
}
