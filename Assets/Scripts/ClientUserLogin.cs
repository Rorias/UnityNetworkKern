using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientUserLogin : MonoBehaviour
{
    public TMP_Text welcomeText;
    public TMP_InputField emailTMP;
    public TMP_InputField passwordTMP;

    public void OnEnable()
    {
        if (DatabaseConnection.userData != null && DatabaseConnection.userData.id != 0)
        {
            welcomeText.text = "Welcome " + DatabaseConnection.userData.username + "!";
        }
    }

    public void Login()
    {
        DatabaseConnection.user = new User() { email = emailTMP.text, password = passwordTMP.text };
        SceneManager.LoadScene(1);
    }
}
