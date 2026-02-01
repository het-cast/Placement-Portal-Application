using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Enums;

namespace PlacementPortal.BLL.IService;

public interface IDashboardStrategy
{
  RoleEnum Role { get; }

  Task<PartialViewResult> GetDashboard(string dateSelect);
}
