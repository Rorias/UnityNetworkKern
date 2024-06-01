public class DBErrorHandler
{
    public static string ErrorMessage(int _EN)
    {
        switch (_EN)
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
