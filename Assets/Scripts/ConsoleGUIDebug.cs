using UnityEngine;

public class ConsoleGUIDebug : MonoBehaviour
{
    static string myLog = "";
    private string output;
    private string stack;
    private string prevOutput;

    private bool active = false;

    private void Awake()
    {
        Application.logMessageReceived += Log;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            active = !active;
        }
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        if (output != prevOutput)
        {
            stack = stackTrace;
            myLog = output + " STACKTRACE: " + stack + "\n" + myLog;
            if (myLog.Length > 5000)
            {
                myLog = myLog.Substring(0, 4000);
            }
            prevOutput = output;
        }
    }

    private void OnGUI()
    {
        if (active) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
        {
            myLog = GUI.TextArea(new Rect(100, 100, Screen.width - 100, Screen.height - 100), myLog);
        }
    }
}
