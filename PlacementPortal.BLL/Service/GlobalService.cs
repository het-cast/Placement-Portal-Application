using Hangfire;
using Microsoft.AspNetCore.Http;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.Helper;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class GlobalService : IGlobalService
{

    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly IJWTService _jWTService;

    private readonly IUnitOfWork _unitOfWork;

    private readonly EmailHelper _emailHelper;

    public GlobalService(IHttpContextAccessor httpContextAccessor, IJWTService jWTService, IUnitOfWork unitOfWork, EmailHelper emailHelper)
    {
        _httpContextAccessor = httpContextAccessor;
        _jWTService = jWTService;
        _unitOfWork = unitOfWork;
        _emailHelper = emailHelper;
    }

    [CustomAuthorize(new string[] { "Admin", "Student", "TPO" })]
    public TokenDataDTO GetTokenData()
    {
        string token = _httpContextAccessor.HttpContext!.Request.Cookies["AuthToken"];

        (string email, string role, int userId) = _jWTService.ValidateToken(token!);

        TokenDataDTO tokenData = new()
        {
            Email = email,
            Role = role,
            UserId = userId
        };

        return tokenData;
    }

    public async Task<bool> IsStudentRegistrationComplete()
    {
        TokenDataDTO tokenData = GetTokenData();

        StudentDetail? studentAcademic = await _unitOfWork.StudentDetailRepository.GetFirstOrDefault(x => x.Student.Email == tokenData.Email)!;

        UserDetail? userDetails = await _unitOfWork.UserDetailsRepository.GetFirstOrDefault(ud => ud.User.Email == tokenData.Email)!;

        bool isStudentRegistrationComplete = userDetails != null && studentAcademic != null;

        return isStudentRegistrationComplete;
    }

    public async Task<bool> IsEverythingSubmittedByStudent()
    {
        TokenDataDTO tokenData = GetTokenData();

        StudentDetail? studentAcademic = await _unitOfWork.StudentDetailRepository.GetFirstOrDefault(x => x.Student.Email == tokenData.Email)!;

        UserDetail? userDetails = await _unitOfWork.UserDetailsRepository.GetFirstOrDefault(ud => ud.User.Email == tokenData.Email)!;

        Resume? resume = await _unitOfWork.ResumeRepository.GetFirstOrDefault(r => r.Student.Email == tokenData.Email)!;

        bool isEverythingReady = userDetails != null && studentAcademic != null && resume != null;

        return isEverythingReady;
    }

    public int RoleIdOfLoggedInUser()
    {
        TokenDataDTO tokenData = GetTokenData();
        RoleEnum roleEnumValue = Enum.Parse<RoleEnum>(tokenData.Role);
        int roleId = (int)roleEnumValue;

        return roleId;
    }

    public int IdOfLoggedInUser()
    {
        TokenDataDTO tokenData = GetTokenData();
        return tokenData.UserId;
    }

    public async void SendEmailToAllStudents()
    {
        List<UserAccount> users = await _unitOfWork.AuthenticationRepository.GetList(ua => ua.RoleId == (int)RoleEnum.Student);

        foreach (var user in users)
        {
            BackgroundJob.Enqueue(() => _emailHelper.SendEmailAsync(user.Email!, "subject", "body"));
        }
    }
}
