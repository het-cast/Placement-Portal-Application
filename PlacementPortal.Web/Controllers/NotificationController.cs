using Microsoft.AspNetCore.Mvc;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.Web.Controllers;

public class NotificationController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUnreadNotificationsCountForCurrentUser()
    {
        try
        {
            List<NotificationDTO> notifications = await _notificationService.RetrieveNotificationsForCurrentUser();
            return Json(new ApiResponseDTO<List<NotificationDTO>>(Consts.GeneralConst.StatusSuccess, Consts.NotificationConst.NotficationsFetched, notifications));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.NotificationConst.NotificationFetchingFailed));
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReadAllNotificationsOfCurrentUser()
    {
        try
        {
            OperationStatusDTO result = await _notificationService.ReadAllNotificationsOfCurrentUser();
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch (Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.NotificationConst.NotMarkedAsRead));
        }

    }

    [HttpPost]
    public async Task<IActionResult> ReadSingularNotification(int notificationMapId)
    {
        try
        {
            OperationStatusDTO result = await  _notificationService.ReadSingularNotifications(notificationMapId);
            return Json(new ApiResponseDTO<string>(result.Success, result.Message));
        }
        catch(Exception)
        {
            return Json(ApiResponse.Fail<string>(Consts.NotificationConst.NotMarkedAsRead));
        }
    }
}
