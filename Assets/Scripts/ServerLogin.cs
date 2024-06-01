using System.Collections;

using TMPro;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ServerLogin : MonoBehaviour
{
    public TMP_InputField serverID;
    public TMP_InputField serverPW;

    public void LoginToServer()
    {
        string url = "https://studenthome.hku.nl/~lars.mulder/server_login.php?" + "id=" + serverID.text + "&pw=" + serverPW.text;

        StartCoroutine(LoginToServer(url));
    }

    private IEnumerator LoginToServer(string _url)
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
                    DatabaseConnection.sessionId = JsonUtility.FromJson<ServerResult>(webRequest.downloadHandler.text.Substring(1));
                    break;
            }
        }

        Debug.Log(DBErrorHandler.ErrorMessage(DatabaseConnection.sessionId.error));

        if (DatabaseConnection.sessionId.error == 1)//no errors
        {
            SceneManager.LoadScene(2);
        }
    }
}
