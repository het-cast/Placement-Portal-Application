using System.Buffers;
using System.Runtime.CompilerServices;
using System.Web;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;
using BcryptHash = BCrypt.Net.BCrypt;

namespace PlacementPortal.BLL.Service;

public class AuthenticateService : IAuthenticateService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IJWTService _jWTService;

    private readonly EmailHelper emailHelper;

    private readonly IGlobalService _globalService;

    private readonly ProtectorHelper _protector;

    private readonly IMFAAuthenticator _authenticator;


    public AuthenticateService(IUnitOfWork unitOfWork, IJWTService jWTService, EmailHelper emailHelper, IGlobalService globalService, ProtectorHelper protector, IMFAAuthenticator authenticator)
    {
        _unitOfWork = unitOfWork;
        _jWTService = jWTService;
        this.emailHelper = emailHelper;
        _globalService = globalService;
        _protector = protector;
        _authenticator = authenticator;
    }

    public async Task<AuthenticationResultDTO> AuthenticateUser(UserCredentialsViewModel userCredentials)
    {
        AuthenticationResultDTO result = new();
        try
        {

            UserAccount? user = await _unitOfWork.AuthenticationRepository
                                                 .GetFirstOrDefault(u => u.Email.Equals(userCredentials.Email.ToLower().Trim()))!;

            if (user == null || !BcryptHash.Verify(userCredentials.Password, user.Password))
            {
                result.Success = false;
                result.Message = Consts.AuthenticationConst.InvalidCredentials;
                return result;
            }

            string role = (RoleEnum)user.RoleId switch
            {
                RoleEnum.Admin => RoleEnum.Admin.ToString(),
                RoleEnum.TPO => RoleEnum.TPO.ToString(),
                RoleEnum.Student => RoleEnum.Student.ToString(),
                _ => RoleConsts.Unknown
            };

            string token = _jWTService.GenerateJwtToken(user.Id, user.Email, role, 1440);

            if (string.IsNullOrEmpty(token))
            {
                result.Success = false;
                result.Message = Consts.AuthenticationConst.jwtTokenGenerationError;
                return result;
            }

            result.Success = true;
            result.Message = Consts.AuthenticationConst.AuthenticationSuccessfull;
            result.Token = token;
            result.Role = role;
            result.Email = user.Email;
            result.UserId = user.Id;

        }
        catch (Exception)
        {
            result.Success = false;
            result.Message = Consts.AuthenticationConst.AuthenticationFailed;
        }
        return result;
    }

    public async Task<AuthenticationResultDTO> AuthenticateUserMFA(UserCredentialsViewModel userCredentials)
    {
        AuthenticationResultDTO result = new();
        try
        {
            UserAccount? user = await _unitOfWork.AuthenticationRepository
                                                 .GetFirstOrDefault(u => u.Email.Equals(userCredentials.Email.ToLower().Trim()))!;

            if (user == null || !BcryptHash.Verify(userCredentials.Password, user.Password))
            {
                result.Success = false;
                result.Message = Consts.AuthenticationConst.InvalidCredentials;
                return result;
            }
            string isMFAEnabled = (user.MfaSecretKey == null || string.IsNullOrEmpty(user.MfaSecretKey)) ? string.Empty : Consts.AuthenticationConst.MFAEnabled;
            result.Success = true;
            result.Message = Consts.AuthenticationConst.AuthenticationSuccessfull;

            // This empty token will tell js that this is emmpty and like user has not enabled two
            // factor authentication
            result.Token = isMFAEnabled;
            result.Role = string.Empty;
            result.Email = _protector.ProtectString(user.Email);
            result.UserId = user.Id;
        }
        catch (Exception)
        {
            result.Success = false;
            result.Message = Consts.AuthenticationConst.AuthenticationFailed;
        }
        return result;
    }

    public async Task<OperationStatusDTO> ResetPassword(PasswordResetDTO passwordReset)
    {

        OperationStatusDTO result = new() { Success = false };

        UserAccount? userAccount = await _unitOfWork.AuthenticationRepository
                                                    .GetFirstOrDefault(ua => ua.Email.ToLower().Trim() == passwordReset.Email.ToLower().Trim())!;

        if (userAccount == null)
        {
            result.Message = Consts.UserConst.UserNotFound;
            return result;
        }

        if (!passwordReset.PasswordReset.Equals(passwordReset.PasswordResetConfirm))
        {
            result.Message = Consts.AuthenticationConst.PasswordsNotMatching;
            return result;
        }

        userAccount.Password = BcryptHash.HashPassword(passwordReset.PasswordResetConfirm);

        GenericReturnDTO status = _unitOfWork.AuthenticationRepository.Update(userAccount);
        await _unitOfWork.SaveChangesAsync();

        if (!status.Success)
        {
            result.Message = Consts.AuthenticationConst.PasswordChangeFailed;
            return result;
        }

        result.Success = true;
        result.Message = Consts.AuthenticationConst.PasswordChangeSuccessfully;

        return result;
    }

    public async Task<bool> CheckUserExistsByEmail(string email)
    {
        string emailToBeSearched = email.Trim().ToLower();
        UserAccount? existingUser = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == emailToBeSearched)!;

        if (existingUser != null)
        {
            return true;
        }

        return false;
    }

    public async Task<OperationStatusDTO> ForgotPasswordPost(string email, string resetUrl)
    {
        OperationStatusDTO result = new();
        bool userExists = await CheckUserExistsByEmail(email);

        if (!userExists)
        {
            result.Message = GeneralConst.UserNotFound;
            return result;
        }

        string htmlMessage = EmailConsts.GenerateResetUrlHtml(resetUrl);
        bool emailSent = await emailHelper.SendEmailAsync(email, EmailConsts.PasswordRequestSubject, htmlMessage);

        if (emailSent)
        {
            result.Message = Consts.GeneralConst.MailSuccess;
            return result;
        }

        result.Message = Consts.GeneralConst.MailFailed;
        return result;
    }

    public async Task<OperationStatusDTO> AddStudentCredentials(AddRegisterStudentViewModel addRegisterStudent)
    {
        OperationStatusDTO result = new();
        string formattedEmail = addRegisterStudent.Email!.ToLower().Trim();
        UserAccount? existingStudent = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == formattedEmail)!;

        if (existingStudent != null)
        {
            result.Message = Consts.UserConst.UserAlreadyExists;
            return result;
        }

        UserAccount newStudent = new()
        {
            Email = formattedEmail,
            Password = BcryptHash.HashPassword(addRegisterStudent.Password),
            RoleId = (int)RoleEnum.Student
        };

        GenericReturnDTO addStudentResult = await _unitOfWork.AuthenticationRepository.Add(newStudent);
        await _unitOfWork.SaveChangesAsync();

        result.Success = true;
        result.Message = Consts.UserConst.UserAddSuccess;

        string htmlMessage = EmailConsts.GenerateAddUserHtml(formattedEmail, addRegisterStudent.Password ?? "");
        await emailHelper.SendEmailAsync(formattedEmail, EmailConsts.StudentAddEmailSubject, htmlMessage);

        return result;
    }

    public async Task<OperationStatusDTO> AddUserCredentials(AddRegisterUserViewModel addUser)
    {
        OperationStatusDTO result = new();
        string formattedEmail = addUser.Email!.ToLower().Trim();
        UserAccount? existingUser = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == formattedEmail)!;

        if (existingUser != null)
        {
            result.Message = Consts.UserConst.UserAlreadyExists;
            return result;
        }

        UserAccount newUser = new()
        {
            Email = formattedEmail,
            Password = BcryptHash.HashPassword(addUser.Password),
            RoleId = addUser.RoleId ?? (int)RoleEnum.TPO
        };

        GenericReturnDTO addStudentResult = await _unitOfWork.AuthenticationRepository.Add(newUser);
        await _unitOfWork.SaveChangesAsync();

        result.Success = true;
        result.Message = Consts.UserConst.UserAddSuccess;

        string htmlMessage = EmailConsts.GenerateAddUserHtml(formattedEmail, newUser.Password ?? "");
        await emailHelper.SendEmailAsync(formattedEmail, EmailConsts.GenerateAddedUserSubject(((RoleEnum)addUser.RoleId!).ToString()!), htmlMessage);

        return result;
    }

    public async Task<GenericReturnDTO> ValidateUserCredentials(UserCredentialsViewModel userCredentials)
    {
        GenericReturnDTO result = new();
        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();
            if (tokenData.Email != userCredentials.Email.ToLower().Trim())
            {
                result.Message = Consts.AuthenticationConst.AuthenticationFailed;
                return result;
            }

            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == userCredentials.Email.ToLower().Trim())!;

            if (user == null || !BcryptHash.Verify(userCredentials.Password, user.Password))
            {
                result.Message = Consts.AuthenticationConst.InvalidCredentials;
                return result;
            }

            string protectedEmail = _protector.ProtectString(userCredentials.Email);

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.AuthenticationConst.AuthenticationSuccessfull;
            result.Data = protectedEmail;
            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.AuthenticationConst.AuthenticationFailed;
            return result;
        }
    }

    public async Task<OperationStatusDTO> DisableMFA(UserCredentialsViewModel userCredentials)
    {
        OperationStatusDTO result = new();
        try
        {
            GenericReturnDTO validationResult = await ValidateUserCredentials(userCredentials);

            if (!validationResult.Success)
            {
                result.Success = validationResult.Success;
                result.Message = validationResult.Message;
                return result;
            }

            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == userCredentials.Email.ToLower().Trim())!;

            user.MfaSecretKey = null;
            user.IsTwoFactorsEnabled = false;

            _unitOfWork.AuthenticationRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.AuthenticationConst.DisabledMFA;
        }
        catch (Exception)
        {
            result.Message = Consts.AuthenticationConst.FailedToDisableMFA;
        }
        return result;
    }

    public async Task<MultipleFactorDTO> GetMFAData(string protectedEmail)
    {
        MultipleFactorDTO result = new();
        try
        {

            string email = _protector.UnProtectString(protectedEmail);

            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(u => u.Email == email.ToLower().Trim())!;

            string secretKey = _authenticator.GenerateSecretKey();

            string uri = _authenticator.GenerateTQrCodeUrl(email, secretKey);

            var qrCodeImage = _authenticator.GenerateQrCode(uri);

            string base64QrCode = Convert.ToBase64String(qrCodeImage);

            user.MfaSecretKey = secretKey;
            user.IsTwoFactorsEnabled = true;

            _unitOfWork.AuthenticationRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            result.Success = Consts.GeneralConst.StatusSuccess;
            result.Message = Consts.AuthenticationConst.MFASuccess;
            result.SecretKey = secretKey;
            result.QRCode = base64QrCode;
            result.Email = email;
            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.AuthenticationConst.AuthenticationFailed;
            return result;
        }
    }

    public async Task<AuthenticationResultDTO> VerifyOtpWithLogin(string email, string otp)
    {
        AuthenticationResultDTO result = new();
        try
        {
            string formattedEmail = _protector.UnProtectString(email);
            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == formattedEmail.ToLower().Trim())!;

            string secretKey = user.MfaSecretKey ?? string.Empty;

            string role = (RoleEnum)user.RoleId switch
            {
                RoleEnum.Admin => RoleEnum.Admin.ToString(),
                RoleEnum.TPO => RoleEnum.TPO.ToString(),
                RoleEnum.Student => RoleEnum.Student.ToString(),
                _ => RoleConsts.Unknown
            };

            string token = _jWTService.GenerateJwtToken(user.Id, user.Email, role, 1440);

            if (string.IsNullOrEmpty(token))
            {
                result.Success = false;
                result.Message = Consts.AuthenticationConst.jwtTokenGenerationError;
                return result;
            }

            if (string.IsNullOrEmpty(secretKey))
            {
                result.Message = Consts.AuthenticationConst.UserNotRegisteredToMFA;
                return result;
            }

            bool otpVerified = _authenticator.ValidateOTP(secretKey, otp);

            if (otpVerified)
            {
                result.Token = token;
                result.Role = role;
                result.Email = user.Email;
                result.UserId = user.Id;
                result.Success = Consts.GeneralConst.StatusSuccess;
                result.Message = Consts.AuthenticationConst.OTPVerifiedSuccess;
                return result;
            }

            result.Message = Consts.AuthenticationConst.OTPVerifyFail;
            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.AuthenticationConst.OTPVerifyFail;
            return result;
        }
    }

    public async Task<OperationStatusDTO> VerifyOtp(string email, string otp)
    {
        OperationStatusDTO result = new();
        try
        {
            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == email.ToLower().Trim())!;
            string secretKey = user.MfaSecretKey ?? string.Empty;
            if (string.IsNullOrEmpty(secretKey))
            {
                result.Message = Consts.AuthenticationConst.UserNotRegisteredToMFA;
                return result;
            }

            bool otpVerified = _authenticator.ValidateOTP(secretKey, otp);

            if (otpVerified)
            {
                result.Success = Consts.GeneralConst.StatusSuccess;
                result.Message = Consts.AuthenticationConst.OTPVerifiedSuccess;
                return result;
            }

            result.Message = Consts.AuthenticationConst.OTPVerifyFail;
            return result;
        }
        catch (Exception)
        {
            result.Message = Consts.AuthenticationConst.OTPVerifyFail;
            return result;
        }
    }

    public async Task<OperationStatusDTO> CheckIfMFAEnabled()
    {
        OperationStatusDTO result = new();
        try
        {
            TokenDataDTO tokenData = _globalService.GetTokenData();
            UserAccount user = await _unitOfWork.AuthenticationRepository.GetFirstOrDefault(ua => ua.Email == tokenData.Email)!;

            if (!user.IsTwoFactorsEnabled || user.MfaSecretKey == null || string.IsNullOrEmpty(user.MfaSecretKey))
            {
                return result;
            }

            result.Success = Consts.GeneralConst.StatusSuccess;
            return result;
        }
        catch (Exception)
        {
            return result;
        }
    }
}
