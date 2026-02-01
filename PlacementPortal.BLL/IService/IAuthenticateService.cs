using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IAuthenticateService
{
    Task<AuthenticationResultDTO> AuthenticateUser(UserCredentialsViewModel userCredentials);

    Task<AuthenticationResultDTO> AuthenticateUserMFA(UserCredentialsViewModel userCredentials);

    Task<OperationStatusDTO> ResetPassword(PasswordResetDTO passwordReset);

    Task<bool> CheckUserExistsByEmail(string email);

    Task<OperationStatusDTO> AddStudentCredentials(AddRegisterStudentViewModel addRegisterStudent);

    Task<OperationStatusDTO> AddUserCredentials(AddRegisterUserViewModel addUser);

    Task<OperationStatusDTO> ForgotPasswordPost(string email, string resetUrl);

    Task<GenericReturnDTO> ValidateUserCredentials(UserCredentialsViewModel userCredentials);

    Task<MultipleFactorDTO> GetMFAData(string protectedEmail);

    Task<OperationStatusDTO> VerifyOtp(string email, string otp);

    Task<AuthenticationResultDTO> VerifyOtpWithLogin(string email, string otp);

    Task<OperationStatusDTO> CheckIfMFAEnabled();

    Task<OperationStatusDTO> DisableMFA(UserCredentialsViewModel userCredentials);
}
