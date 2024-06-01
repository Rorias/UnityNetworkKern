using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientUserSignup : MonoBehaviour
{
    public TMP_InputField emailTMP;
    public TMP_InputField usernameTMP;
    public TMP_InputField bdateTMP;
    public TMP_InputField passwordTMP;
    public void Signup()
    {
        DatabaseConnection.user = new User() { email = emailTMP.text, username = usernameTMP.text, birthdate = bdateTMP.text, password = passwordTMP.text };
        SceneManager.LoadScene(1);
    }
}
