using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class AuthenticationController : Controller
{
    private readonly IAuthenticateService _authenticateService;

    private readonly IJWTService _jWTService;

    private readonly EmailHelper emailHelper;

    public AuthenticationController(IAuthenticateService authenticateService, IJWTService jWTService, EmailHelper emailHelper)
    {
        _authenticateService = authenticateService;
        _jWTService = jWTService;
        this.emailHelper = emailHelper;
    }

    [HttpGet]
    public IActionResult Login()
    {
        try
        {
            string rememberMe = Request.Cookies["RememberMe"]!;
            string authToken = Request.Cookies["AuthToken"]!;

            if (!string.IsNullOrEmpty(rememberMe) && !string.IsNullOrEmpty(authToken))
            {
                (string Email, string Role, int userId) = _jWTService.ValidateToken(authToken);
                if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Role))
                {
                    return View();
                }

                return RedirectToAction("Dashboard", "Dashboard");
            }

            return View();

        }
        catch (Exception)
        {
            return View();
        }
    }

    [HttpGet]
    public IActionResult MFAOtpVerify(string protectedEmail){
        MultipleFactorDTO data = new();
        return View("MFAOtpVerify", data);
    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole, RoleConsts.TPORole })]
    [HttpGet]
    public IActionResult AddStudent()
    {
        return View();
    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole })]
    [HttpGet]
    public IActionResult AddUserAdmin()
    {
        return View();
    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole, RoleConsts.TPORole })]
    [HttpPost]
    public async Task<IActionResult> AddStudentCredentials(AddRegisterStudentViewModel addRegisterStudent)
    {
        try
        {
            OperationStatusDTO? result = await _authenticateService.AddStudentCredentials(addRegisterStudent);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message!, Url.Action("Dashboard", "Dashboard")));
        }
        catch (Exception)
        {
            return Json(new ApiResponseDTO<string>(Consts.GeneralConst.StatusFailed, Consts.AuthenticationConst.UserCredentialsNotAdded));
        }
    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole })]
    [HttpPost]
    public async Task<IActionResult> AddUserCredentials(AddRegisterUserViewModel addUser)
    {
        try
        {
            OperationStatusDTO? result = await _authenticateService.AddUserCredentials(addUser);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message!, Url.Action("Dashboard", "Dashboard")));
        }
        catch (Exception)
        {
            return Json(new ApiResponseDTO<string>(Consts.GeneralConst.StatusFailed, Consts.AuthenticationConst.UserCredentialsNotAdded));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AuthenticateUser(UserCredentialsViewModel userCredentials, bool rememberMe)
    {
        try
        {
            AuthenticationResultDTO result = await _authenticateService.AuthenticateUser(userCredentials);

            if (result.Success)
            {
                List<Claim> claims = new(){
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()!),
                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.Role, result.Role)
                };


                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal principal = new(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    });

                CookieOptions authCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1),
                    IsEssential = true
                };
                CookieOptions email = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                CookieOptions rememberMeCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                CookieOptions role = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                Response.Cookies.Append("AuthToken", result.Token, authCookieOptions);

                Response.Cookies.Append("Email", result.Email.ToLower().Trim(), authCookieOptions);

                Response.Cookies.Append("Role", result.Role ?? "Student", authCookieOptions);

                if (rememberMe)
                {
                    rememberMeCookieOptions.Expires = DateTime.UtcNow.AddDays(30);
                    Response.Cookies.Append("RememberMe", userCredentials.Email, rememberMeCookieOptions);
                }
                else
                {
                    Response.Cookies.Delete("RememberMe");
                }

                return Json(new { success = result.Success, message = result.Message, redirectToUrl = Url.Action("Dashboard", "Dashboard") });
            }

            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(new ApiResponseDTO<string>(success: Consts.GeneralConst.StatusFailed, message: Consts.AuthenticationConst.AuthenticationFailed));
        }
    }

    [HttpPost]
    public async Task<IActionResult> AuthenticateUserMFA(UserCredentialsViewModel userCredentials, bool rememberMe)
    {
        try
        {
            AuthenticationResultDTO result = await _authenticateService.AuthenticateUserMFA(userCredentials);

            if (result.Success)
            {
                if (rememberMe)
                {
                    CookieOptions rememberMeCookieOptions = new()
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(30)
                    };
                    Response.Cookies.Append("RememberMe", userCredentials.Email, rememberMeCookieOptions);
                }
                else
                {
                    Response.Cookies.Delete("RememberMe");
                }

                return Json(new { success = result.Success, message = result.Message, data = result.Email, mfaEnabled = result.Token ,redirectToUrl = Url.Action("MFAOtpVerify", "Authentication") });
            }

            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(new ApiResponseDTO<string>(success: Consts.GeneralConst.StatusFailed, message: Consts.AuthenticationConst.AuthenticationFailed));
        }
    }

    public async Task<IActionResult> VerifyOTPandLogin(string email, string otp)
    {
        try
        {
            AuthenticationResultDTO result = await _authenticateService.VerifyOtpWithLogin(email, otp);
            if (result.Success)
            {
                List<Claim> claims = new(){
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()!),
                    new Claim(ClaimTypes.Email, result.Email),
                    new Claim(ClaimTypes.Role, result.Role)
                };


                ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                ClaimsPrincipal principal = new(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1)
                    });

                CookieOptions authCookieOptions = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1),
                    IsEssential = true
                };

                CookieOptions emailCookie = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                CookieOptions role = new()
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                };

                Response.Cookies.Append("AuthToken", result.Token, authCookieOptions);

                Response.Cookies.Append("Email", result.Email.ToLower().Trim(), authCookieOptions);

                Response.Cookies.Append("Role", result.Role ?? "Student", authCookieOptions);

                return Json(new { success = result.Success, message = result.Message, redirectToUrl = Url.Action("Dashboard", "Dashboard") });
            }
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(new ApiResponseDTO<string>(success: Consts.GeneralConst.StatusFailed, message: Consts.AuthenticationConst.AuthenticationFailed));
        }
    }

    [HttpGet]
    public IActionResult LogOut()
    {
        try
        {
            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies.Delete("AuthToken");
                Response.Cookies.Delete("Email");
                Response.Cookies.Delete("Role");
            }

            return RedirectToAction("Login", "Authentication");
        }
        catch (Exception)
        {
            return RedirectToAction("Login", "Authentication");
        }
    }

    [HttpGet]
    public IActionResult ForgotPassword(string email)
    {

        if (!string.IsNullOrEmpty(email))
        {
            email = string.IsNullOrWhiteSpace(email) ? string.Empty : email.Trim().ToLower();
        }
        return View("ForgotPassword", new ForgotPasswordViewModel() { Email = email });
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPasswordPost(string email)
    {
        try
        {
            email = email.Trim().ToLower();

            string token = _jWTService.GenerateJwtToken(-1, email, "User", 1440);

            string? resetUrl = Url.Action("ResetPassword", "Authentication", new
            {
                email,
                token
            }, Request.Scheme);

            OperationStatusDTO result = await _authenticateService.ForgotPasswordPost(email, resetUrl ?? "");

            TempData[result.Success.ToString()] = result.Message;

            return RedirectToAction("Login");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = Consts.GeneralConst.UnexpectedError;
            return RedirectToAction("ForgotPassword");
        }
    }

    // [CustomAuthorize(new string[] { "Student", "TPO" })]
    [HttpGet]
    public IActionResult ResetPassword(string email, string token)
    {
        (string emailFromToken, string role, int userId) = _jWTService.ValidateToken(token);

        if (email != emailFromToken)
        {
            TempData["ErrorMessage"] = "Email Mismatch, Please Try Again later!";
            return RedirectToAction("ForgotPassword");
        };

        if (!string.IsNullOrEmpty(email))
        {
            email = WebUtility.UrlDecode(email.Trim().ToLower());
        }
        else
        {
            email = "";
        }
        PasswordResetDTO resetData = new() { Email = email };

        return View("ResetPassword", resetData);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordPost(PasswordResetDTO passwordResetDTO)
    {
        try
        {
            OperationStatusDTO result = await _authenticateService.ResetPassword(passwordResetDTO);
            TempData[result.Success.ToString()] = result.Message;

            return RedirectToAction("Login");
        }
        catch (Exception)
        {
            TempData[Consts.GeneralConst.StatusFailed.ToString()] = "Password not changed Successfully!";

            return RedirectToAction("ResetPassword", new { email = passwordResetDTO.Email });
        }
    }

    [HttpPost]
    public async Task<IActionResult> VerifyOtp(string email, string otp)
    {
        try
        {
            var result = await _authenticateService.VerifyOtp(email, otp);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception Ex)
        {
            return Json(ApiResponse.Fail<string>(Consts.AuthenticationConst.OTPVerifyFail));
        }
    }

}
