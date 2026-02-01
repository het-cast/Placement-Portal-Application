using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Strategies;

public class StudentDashboardStrategy : IDashboardStrategy
{
  private readonly IDashboardService _dashboardService;

    public StudentDashboardStrategy(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public RoleEnum Role => RoleEnum.Student;

    public async Task<PartialViewResult> GetDashboard(string dateSelect)
    {
      try
        {
            StudentDashboardDTO dashboardDetails = await _dashboardService.GetStudentDashboardDetails(dateSelect);
            return new PartialViewResult
            {
                ViewName = "_StudentDashboard",
                ViewData = new ViewDataDictionary(
                    metadataProvider: new EmptyModelMetadataProvider(),
                    modelState: new ModelStateDictionary())
                {
                    Model = dashboardDetails
                }
            };
        }
        catch (Exception)
        {
            throw new Exception(Consts.DashboardConst.FetchingFailed); 
        }
    }

}
