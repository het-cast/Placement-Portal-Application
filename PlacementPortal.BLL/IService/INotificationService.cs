using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface INotificationService
{
    Task<OperationStatusDTO> SaveNotificationForSpecificUser(int userId, string message);

    Task<OperationStatusDTO> SaveNotificationAsyncsForAllStudents(string message);

    Task<OperationStatusDTO> SaveNotificationAsyncsForAllTPOs(string message);

    Task NotifyUserAsync(string userId, string message);
    
    Task NotifyRoleAsync(string role, string message);

    Task<List<NotificationDTO>> RetrieveNotificationsForCurrentUser();

    Task<OperationStatusDTO> ReadAllNotificationsOfCurrentUser();

    Task<OperationStatusDTO> ReadSingularNotifications(int notificationMapId);

    Task<OperationStatusDTO> SaveNotificationsForStudentsEnhancedMessage(int senderId, string message);
}
