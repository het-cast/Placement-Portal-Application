namespace PlacementPortal.BLL.IService;

public interface IJobDeadlineReminderService
{
  Task SendDeadlineReminderEmailsAsync();
}
