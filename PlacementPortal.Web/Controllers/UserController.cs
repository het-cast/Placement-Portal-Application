using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

public class UserController : Controller
{
    private readonly IAuthenticateService _authenticateService;

    public UserController(IAuthenticateService authenticateService)
    {
        _authenticateService = authenticateService;

    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole, RoleConsts.StudentRole, RoleConsts.TPORole })]
    [HttpGet]
    public IActionResult EnableMFA()
    {
        return View();
    }

    [HttpGet]
    public IActionResult GetUserAuthForm()
    {
        return PartialView("_UserAuthForm");
    }

    [HttpGet]
    public IActionResult RegisterMFA()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GetMFAData(string protectedEmail)
    {
        MultipleFactorDTO mfaData = await _authenticateService.GetMFAData(protectedEmail);
        return PartialView("_AuthenticatorData", mfaData);
    }

    [HttpPost]
    public async Task<IActionResult> ValidateUserCredentials(UserCredentialsViewModel userCredentials)
    {
        try
        {
            GenericReturnDTO result = await _authenticateService.ValidateUserCredentials(userCredentials);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message, result.Data as string));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.AuthenticationConst.AuthenticationFailed));
        }
    }

    [HttpGet]
    public async Task<IActionResult> CheckIfMFAEnabled()
    {
        try
        {
            OperationStatusDTO result = await _authenticateService.CheckIfMFAEnabled();
            return Json(new ApiResponseDTO<string>(result.Success, string.Empty));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(string.Empty));
        }
    }

    [HttpPost]
    public async Task<IActionResult> DisableMFA(UserCredentialsViewModel userCredentials)
    {
        try
        {
            OperationStatusDTO result = await _authenticateService.DisableMFA(userCredentials);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.AuthenticationConst.FailedToDisableMFA));
        }

    }
}
