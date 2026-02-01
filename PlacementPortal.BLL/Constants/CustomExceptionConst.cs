namespace PlacementPortal.BLL.Constants;

public class CustomExceptionConst
{
    public const string StudentRegistrationInComplete = "Student Registration In Complete with resume";

    public const string InvalidFilter = "Invalid Filter Applied !";

    public const string InvalidRoleInToken = "Invalid role found in the token";

    public const string TokenExpired = "Token has expired";

    public const string TokenValidationError = "An error occurred while validating the token";

    public const string DataNotFound = "Data not found";

    public static string GetMessageOfMissingStrategy(string role){
       return $"Dashboard strategy service not registered for role: {role}";
    }
};


