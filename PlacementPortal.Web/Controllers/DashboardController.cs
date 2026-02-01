using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Service;
using PlacementPortal.BLL.Services;
using PlacementPortal.BLL.Strategies;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
public class DashboardController : Controller
{
    private readonly DashboardStrategyFactory _dashboardStrategyFactory;

    public DashboardController(DashboardStrategyFactory dashboardStrategyFactory)
    {
        _dashboardStrategyFactory = dashboardStrategyFactory;
    }

    [CustomAuthorize(new string[] {"Admin", "Student", "TPO" })]
    [HttpGet]
    public IActionResult Dashboard()
    {
        return View();
    }

    [CustomAuthorize(new string[] { "Admin", "Student", "TPO" })]
    [HttpGet]
    public async Task<IActionResult> GetDashboardDetails(string dateSelect)
    {
        try
        {
            var strategy = _dashboardStrategyFactory.GetStrategy();
            return await strategy.GetDashboard(dateSelect);
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>("Failed to load dashboard."));
        }
    }

    [HttpGet]
    public IActionResult SuperAdminDashboard(){
        return PartialView("_AdminDashboard");
    }

    
    // [HttpGet]
    // public async Task<IActionResult> GetDashboardDetails(string dateSelect)
    // {
    //     try
    //     {
    //         var tokenDetails = _globalService.GetTokenData();
    //         if (tokenDetails.Role == RoleEnum.TPO.ToString())
    //         {
    //             return await GetTPODashboardDetails(dateSelect);
    //         }
    //         else if (tokenDetails.Role == RoleEnum.Student.ToString())
    //         {
    //             return await GetStudentDashboardDetails(dateSelect);
    //         }
    //         return RedirectToAction("Login", "Authentication");
    //     }
    //     catch (Exception)
    //     {
    //         return RedirectToAction("Dashboard");
    //     }
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetTPODashboardDetails(string dateSelect)
    // {
    //     try
    //     {
    //         var dashboardDetails = await _dashboardService.GetTPODashboardDetails(dateSelect);
    //         return PartialView("_TPODashboard", dashboardDetails);
    //     }
    //     catch (Exception)
    //     {
    //         return Json(ApiResponse.Fail<string>(Consts.DashboardConst.FetchingFailed));
    //     }
    // }

    // [HttpGet]
    // public async Task<IActionResult> GetStudentDashboardDetails(string dateSelect)
    // {
    //     try
    //     {
    //         var dashboardDetails = await _dashboardService.GetStudentDashboardDetails(dateSelect);
    //         return PartialView("_StudentDashboard", dashboardDetails);
    //     }
    //     catch (Exception)
    //     {
    //         return Json(ApiResponse.Fail<string>(Consts.DashboardConst.FetchingFailed));
    //     }
    // }
}
