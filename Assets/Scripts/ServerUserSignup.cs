using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public class ServerUserSignup : MonoBehaviour
{
    private Server server;
    private MessageHandler.Message signupMsg;

    public void SignupAsUser(Server _server, MessageHandler.Message _signup)
    {
        server = _server;
        signupMsg = _signup;
        string url = "https://studenthome.hku.nl/~lars.mulder/user_signup.php?" + "sessid=" + DatabaseConnection.sessionId.result + "&email=" + signupMsg.email + "&user=" + signupMsg.usrname + "&bdate=" + signupMsg.bdate + "&pw=" + signupMsg.pass;

        StartCoroutine(SignupAsUser(url));
    }

    private IEnumerator SignupAsUser(string _url)
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
                    int err = JsonUtility.FromJson<ServerResult>(webRequest.downloadHandler.text.Substring(1)).error;
                    Debug.Log(DBErrorHandler.ErrorMessage(err));

                    if (err == 1)
                    {
                        MessageHandler.Message msg = new MessageHandler.Message()
                        {
                            cmd = MessageHandler.CommandType.LoginUserRequest,
                            email = signupMsg.email,
                            pass = signupMsg.pass,
                        };

                        GetComponent<ServerUserLogin>().LoginAsUser(server, msg);
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
