using System.Web;
using Hangfire.PostgreSql.Properties;

namespace PlacementPortal.BLL.Constants;

public class EmailConsts
{
    public const string PasswordRequestSubject = "Password Reset Request";

    public const string StudentAddEmailSubject = "You were added as a Student in System.";

    public const string CompanyUpdate = "Company Update Notification";

    public const string DeadlineSoon = $"📢 Reminder: Application Deadline Soon!";

    public static string GenerateAddedUserSubject(string roleName){
        return $"You were added as a {roleName} in the system.";
    }

    public static string GenerateResetUrlHtml(string resetUrl)
    {
        string htmlMessage = $@" 
                                <div style='background-color: #0066a7; height: 100px; display: flex;justify-content:center; gap-2; align-items:center ' class=''> 
                                <h2 style='color:white;'>Placement Portal Application</h2> 
                                </div> 
                                <div class='m-3'> 
                                <p>Please click <a href='{HttpUtility.HtmlEncode(resetUrl)}'>here</a> to reset your account Password</p>
                                <p>If you encounter any issues or have any questions, please do not hesitate to contact our support team.</p>
                                <p><span style='color: orange;'>Important Note:</span> For security reasons, the link will expire in 24 hours. If you did not request a password reset, please ignore this email or contact our support team immediately</p>
                                </div>";

        return htmlMessage;
    }

    public static string GenerateAddUserHtml(string email, string password)
    {
        string htmlMessage = @$"
                            <div style='background-color: #0066a7; height: 100px; display: flex;justify-content:center; gap-2; align-items:center ' class=''>
                                <img style='height: 80px;'  alt=''>
                                <h2 style='color:white;'>PLacement Portal Application</h2>
                            </div>
                            <div>

                            <div>
                              <p>Welcome to Placement Portal - :) </p>
                            </div>
                            <div class='m-3'>
                              <h4>Login Details : </h4>
                            </div>
                            <div class='m-3 d-flex flex-column'>
                                <span >Username : {email}</span>
                                <span> Password : {password}</span>
                            </div
                            </div>";
        return htmlMessage;
    }

    public static string GenerateCompanyUpateBody(string companyName)
    {
        string body = $"Dear Student,\n\nA {companyName}'s Job Listing has been updated on the Placement Portal. Please check the portal for updated job listings and company information.\n\nBest regards,\nPlacement Cell";

        return body;
    }

    public static string GenerateDeadlineSoonBody(string jobName)
    {
        string body = $"\nThis is a friendly reminder that the job posting for {jobName} will close soon\nMake sure to apply before it's too late!\n\nRegards,\nPlacement Portal";

        return body;
    }

}
