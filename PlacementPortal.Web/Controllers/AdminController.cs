using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.BLL.Services;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public IActionResult TPOs(){
        return View();
    }

    [CustomAuthorize(new string[] { RoleConsts.AdminRole })]
    [HttpPost]
    public async Task<IActionResult> GetTPOsListPaginated(PaginationRequestDTO paginationRequest)
    {
        try
        {
            PaginationDTO<TPOListDTO> tpoList = await _adminService.GetTPOsPaginated(paginationRequest);
            return PartialView("_TPOList", tpoList);
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.GeneralConst.ErrorFetchingData));
        }
    }
}
