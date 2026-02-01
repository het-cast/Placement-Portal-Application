using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IService;

namespace PlacementPortal.BLL.Strategies;

public class AdminDashboardStrategy : IDashboardStrategy
{
  private readonly IDashboardService _dashboardService;

    public AdminDashboardStrategy(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public RoleEnum Role => RoleEnum.Admin;

    public async Task<PartialViewResult> GetDashboard(string dateSelect)
    {
      try
        {
            var dashboardDetails = await _dashboardService.GetAdminDashboardDetails(dateSelect);
            return new PartialViewResult
            {
                ViewName = "_AdminDashboard",
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
