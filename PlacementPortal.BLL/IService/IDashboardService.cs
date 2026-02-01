using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IService;

public interface IDashboardService
{
    Task<AdminDashboardDTO> GetAdminDashboardDetails(string dateSelect);

    Task<StudentDashboardDTO> GetStudentDashboardDetails(string dateSelect);

    Task<TPODashboardDTO> GetTPODashboardDetails(string dateSelect);

    Task<int> GetActiveJobListingCount(string dateSelect);

    Task<int> GetRejectedOrNoReponseApplicationsCount(string dateSelect);

    Task<int> GetHiredStudentsCount(string dateSelect);

    Task<int> GetStudentCount(string dateSelect);

    Task<int> GetApplicationsCount(string dateSelect);

    Task<int> GetCompaniesCount(string dateSelect);

    Task<int> GetUpcomingJobRoles();

}
