using System;

public class DatabaseConnection
{
    [NonSerialized] public static ServerResult sessionId;
    [NonSerialized] public static UserResult userData;
    [NonSerialized] public static User user;
    [NonSerialized] public static string IP;

    public static string ErrorMessage(int _errNum)
    {
        switch (_errNum)
        {
            case 0:
                return "Validation of variable failed.";
            case 2:
                return "Value check failed.";
            case 3:
                return "Session is invalid.";
            case 1:
            default:
                return "Success!";
        }
    }
}
