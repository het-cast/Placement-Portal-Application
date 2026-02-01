using System.Data.Entity.Validation;

namespace PlacementPortal.BLL.Constants;

public class Consts
{
    public class GeneralConst
    {
        public const bool StatusSuccess = true;

        public const bool StatusFailed = false;

        public const string Null = "";

        public const string UnexpectedError = "An unexpected error occurred! Please try again later";

        public const string ErrorFetchingData = "Error Occurred in fetching Data! Please try again later";

        public const string DateFetched = "Data Fetched successfully";

        public const string MailSuccess = "Mail sent successfully";

        public const string MailFailed = "Some error occurred in sending mail";

        public const string NotFound = "NotFound";

        public const string BadRequest = "BadRequest";

        public const string Content = "Content";

        public const string AlreadyExists = "Company Already exists";

        public const string SessionExpired = "Session Expired! Please Log In Again.";

        public const string NotAvailable = "N/A";
    }

    public class EntityConst
    {
        public const string CompanyProfile = "Company";

        public const string JobListing = "Job Listing(s)";

        public const string User = "User";

        public const string Student = "Student";

        public const string Profile = "Profile";

        public const string Resume = "Resume";

        public const string ResumeComment = "Resume Comment";

        public const string Notfication = "Notification";

    }

    public class NotificationConst
    {
        public const string NotificationFetchingFailed = "Failed to fetch notifications";

        public const string NotficationsFetched = "Fetched Notifications";

        public const string MarkedAsRead = "Marked as read successfully";

        public const string MarkedAsReadAll = "All Notifications marked as read successfully";

        public const string NotMarkedAsRead = "Notifications not marked as read";
    }

    public class StudentConst
    {
        public const string EligibilityCriteriaNotMet = "Student does not meet the eligibility criteria.";

        public const string AcademicDetailsNotUpdated = "Academic Details not updated";

        public const string StudentNotFound = "Student not found";

        public const string AcademicDetailsSaved = "Academic Details Saved successfully.";

        public const string AcademicDetailsNotSaved = "Academic Details Not Saved successfully.";
    }

    public class DashboardConst
    {
        public const string FetchingFailed = "Failed to Fetch Dashboard Details";
    }

    public class AuthenticationConst
    {
        public const string AuthenticationFailed = "Authentication Failed ! Try again later.";

        public const string AuthenticationSuccessfull = "Authentication Successfull";

        public const string PasswordChangeSuccessfully = "Password changed successfully";

        public const string PasswordChangeFailed = "Failed to change the password";

        public const string UserCredentialsNotAdded = "User Credentials not added successfully";

        public const string InvalidCredentials = "Invalid Credentials";

        public const string jwtTokenGenerationError = "Error in generating JWT Token";

        public const string PasswordsNotMatching = "Passwords are not matching";

        public const string MFASuccess = "MFA Registration Successfull";

        public const string OTPVerifiedSuccess = "OTP Verified Successfully";

        public const string OTPVerifyFail = "OTP Verification Failed";

        public const string UserNotRegisteredToMFA = "User has not enabled the MFA Feature";

        public const string MFAEnabled = "MFA Enabled";

        public const string FailedToDisableMFA = "Failed to Disable MFA ! Try again later";

        public const string DisabledMFA = "Disabled MFA Successfully";
    }

    public class UserConst
    {
        public const string UserNotFound = "User not found.";

        public const string UserAlreadyExists = "User Already Exists";

        public const string UserAddSuccess = "User added successfully";

        public const string UserDetailsNotSaved = "User Details not saved";

        public const string UserDetailsSaved = "User Details Saved Successfully";

    }

    public class AuditLog
    {
        public const string LogAdded = "Entry done in Audit Log Table";

        public const string LogNotAdded = "Entry Unsuccessfull in Audit Log Table";
    }

    public class OperationsForLog
    {
        public const string Added = "Added";

        public const string Modified = "Modified";

        public const string Deleted = "Deleted";
    }

    public class HashForm
    {
        public const string Converted = "Successfully converted to hash form";

        public const string NotConverted = "Not Successfully converted to hash form";
    }

    public class JobApplicationConst
    {
        public const string AlreadyApplied = "Already Applied";

        public const string FailedToApply = "Failed to apply";

        public const string FailedToLoadApplications = "Failed to Load Applications";

        public const string FailedToLoadApplicationDetails = "Failed to load Application Details";

        public const string SubmitSuccess = "Job application submitted successfully.";

        public const string SubmitFailed = "Failed to apply to the application ! Try again Later";

        public const string JobApplicationNotFound = "No Job Application was Found";

        public const string JobApplicationUpdateSuccessfully = "Job Applications Updated Successfully";

        public const string JobApplicationNotUpdated = "Job Applications Not Updated Successfully";

        public const string AlreadyHiredStudent = "Student is already in hired status";
    }

    public class ResumeConst
    {
        public const string ResumePreviewed = "Resume Previewed successfully";

        public const string ResumeDownloaded = "Resume Downloaded successfully";

        public const string InValidFileName = "Invalid file name";

        public const string ResumeNotFound = "Resume Not Found";

        public const string ExtPreviewNotSupported = "Preview not supported in this, Please download the file";

        public const string ErrorFetchingList = "Error fetching in Resume list";

        public const string ResumeNotSaved = "Resume not saved Successfully";

        public const string ResumeSaved = "Resume Saved Successfully";

        public const string NoFileSelected = "No file selected to upload";
    }

    public class CompanyConst
    {
        public const string CompanyNotFound = "No Company Exists with the following data";

        public const string CompanyDeleted = "Company Deleted Successfully";

        public const string CompanyNotDeleted = "Company Not Deleted Successfully";

        public const string CompanyFetchingFailed = "Company Profiles not fetched successfully";

        public const string ListingsFetchingFailed = "Job Listings not fetched successfully";

        public const string NoJobListingsFound = "No Job Listings found from that Id(s)";

        public const string NoListingsFoundToUpdate = "No Job Listings found to be updated";

        public const string CompanyProfileAddedSuccessfully = "Company Profile Added Successfully";

        public const string CompanyProfileDeletedSuccessfully = "Company Profile Deleted Successfully";

        public const string CompanyProfileFailedToDelete = "Failed to delete Company Profile";

        public const string CompanyProfileFailedToUpdate = "Failed to Update Company Profile";

        public const string CompanyProfileFailedToAdd = "Failed to add Company Profile";

        public const string JobListingSavedSuccessfully = "Job Listing Saved Successfully";

        public const string JobListingDeletedSuccessfully = "Job Listing Deleted Successfully";

        public const string JobListingFailedToDelete = "Failed to delete Job Listing";

        public const string JobListingFailedToSave = "Failed to Save Job Listing/s";

        public const string JobListingFailedToAdd = "Failed to add Job Listing";

        public const string CompanyAssociatedTrue = "Company still associated with some applications In Progress";
    }

    public static string ExistsMessage(string entityName)
    {
        return $"{entityName} already exists";
    }

    public static string AddedMessage(string entityName)
    {
        return $"{entityName} Added Successfully";
    }

    public static string UpdatedMessage(string entityName)
    {
        return $"{entityName} Updated Successfully";
    }

    public static string AddedFailedMessage(string entityName)
    {
        return $"{entityName} not Added Successfully";
    }

    public static string UpdatedFailedMessage(string entityName)
    {
        return $"{entityName} not Updated Successfully.";
    }

    public static string SavedMessage(string entityName)
    {
        return $"{entityName} Saved Successfully.";
    }

    public static string FailToSaveMessage(string entityName)
    {
        return $"{entityName} not Saved Successfully.";
    }

    public static string NotFound(string entityName)
    {
        return $"{entityName} not found.";
    }

    public static string NothingToBeAdded(string entityName)
    {
        return $"There is nothing to be added in {entityName}.";
    }

    public class GenerateNotificationMessage
    {
        public static string Added(string entityName, string addedEntityName)
        {
            return $"Added New {entityName} {addedEntityName}.";
        }

        public static string Modified(string entityName, string modifiedEntityName)
        {
            return $"Modified data of {entityName} - {modifiedEntityName}.";
        }

        public static string Deleted(string entityName, string deletedCompanyName)
        {
            return $"Deleted {entityName} - {deletedCompanyName}.";
        }

        public static string AppliedToJob(string applierEmail, string companyName)
        {
            return $"{applierEmail} applied to some listing of {companyName}";
        }

    }
}
