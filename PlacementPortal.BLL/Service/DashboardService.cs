using System.Linq.Expressions;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.Enums;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.Models;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDashboardDTO> GetAdminDashboardDetails(string dateSelect)
    {
        try
        {
            List<AuditLogDTO> logs = await GetAuditLogs();

            int totalStudents = await TotalStudentsRegistered(dateSelect);

            int totalTPOs = await TotalTPORegistered(dateSelect);

            int totalJobListingsActive = await GetActiveJobListingCount(dateSelect);

            int rejectedOrNoResponseApplicationsCount = await GetRejectedOrNoReponseApplicationsCount(dateSelect);

            int totalStudentsHired = await GetHiredStudentsCount(dateSelect);

            return new AdminDashboardDTO
            {
                TotalActiveStudents = totalStudents,
                TotalActiveTPOs = totalTPOs,
                TotalJobListingsActive = totalJobListingsActive,
                RejectedOrNoReponseApplicationsCount = rejectedOrNoResponseApplicationsCount,
                TotalStudentsHired = totalStudentsHired,
                AuditLogs = logs
            };
        }
        catch (Exception)
        {
            return new AdminDashboardDTO();
        }
    }

    public async Task<TPODashboardDTO> GetTPODashboardDetails(string dateSelect)
    {
        try
        {
            int activeJobListings = await GetActiveJobListingCount(dateSelect);
            int activeStudents = await GetStudentCount(dateSelect);
            int applicationsCount = await GetApplicationsCount(dateSelect);
            int companiesCount = await GetCompaniesCount(dateSelect);

            TPODashboardDTO TPODashboard = new()
            {
                ActiveJobListings = activeJobListings,
                CompaniesCount = companiesCount,
                Students = activeStudents,
                TotalApplications = applicationsCount
            };

            return TPODashboard;
        }
        catch (Exception)
        {
            return new TPODashboardDTO();
        }
    }

    public async Task<StudentDashboardDTO> GetStudentDashboardDetails(string dateSelect)
    {
        int UpcomingJobRoles = await GetUpcomingJobRoles();

        StudentDashboardDTO studentDashboard = new()
        {
            UpcomingJobRoles = UpcomingJobRoles,
        };

        return studentDashboard;
    }

    public async Task<int> GetActiveJobListingCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<CompanyJobListing, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => jl => jl.IsDeleted == false && jl.CreatedAt >= today.AddDays(-7),
            FilterConst.Last30Days => jl => jl.IsDeleted == false && jl.CreatedAt >= today.AddDays(-30),
            FilterConst.CurrentMonth => jl => jl.IsDeleted == false && jl.CreatedAt.Year == today.Year && jl.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => jl => jl.IsDeleted == false &&
                                           jl.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           jl.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => jl => jl.IsDeleted == false && jl.CreatedAt.Year == today.Year,
            FilterConst.All or null or "" => jl => jl.IsDeleted == false,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.CompanyJobListingRepository.Count(countQuery);
    }

    public async Task<int> GetStudentCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<StudentDetail, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => sd => sd.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => sd => sd.CreatedAt.Year >= today.Year && sd.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => sd =>
                                           sd.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           sd.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => sd => sd.CreatedAt.Year >= today.Year,
            FilterConst.All or null or "" => sd => true,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.StudentDetailRepository.Count(countQuery);
    }

    public async Task<int> GetApplicationsCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<JobApplication, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ja => ja.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => ja => ja.CreatedAt.Year >= today.Year && ja.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => ja => ja.IsDeleted == false &&
                                           ja.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           ja.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => ja => ja.CreatedAt.Year >= today.Year,
            FilterConst.All or null or "" => ja => true,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.JobApplicationRepository.Count(countQuery);
    }

    public async Task<int> GetHiredStudentsCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<JobApplication, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ja => ja.CreatedAt >= today.AddDays(-7) && ja.ApplyStatus == (int)JobApplicationEnum.Hired ,
            FilterConst.CurrentMonth => ja => ja.CreatedAt.Year >= today.Year && ja.CreatedAt.Month == today.Month && ja.ApplyStatus == (int)JobApplicationEnum.Hired,
            FilterConst.PreviousMonth => ja => ja.IsDeleted == false &&
                                           ja.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           ja.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1) && ja.ApplyStatus == (int)JobApplicationEnum.Hired,
            FilterConst.CurrentYear => ja => ja.CreatedAt.Year >= today.Year && ja.ApplyStatus == (int)JobApplicationEnum.Hired,
            FilterConst.All or null or "" => ja => true && ja.ApplyStatus == (int)JobApplicationEnum.Hired,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.JobApplicationRepository.Count(countQuery);
    }

    public async Task<int> GetRejectedOrNoReponseApplicationsCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<JobApplication, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ja => ja.CreatedAt >= today.AddDays(-7) && (ja.ApplyStatus == (int)JobApplicationEnum.NoResponse || ja.ApplyStatus == (int)JobApplicationEnum.Rejected ) ,
            FilterConst.CurrentMonth => ja => ja.CreatedAt.Year >= today.Year && ja.CreatedAt.Month == today.Month && (ja.ApplyStatus == (int)JobApplicationEnum.NoResponse || ja.ApplyStatus == (int)JobApplicationEnum.Rejected ),
            FilterConst.PreviousMonth => ja => ja.IsDeleted == false &&
                                           ja.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           ja.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1) && (ja.ApplyStatus == (int)JobApplicationEnum.NoResponse || ja.ApplyStatus == (int)JobApplicationEnum.Rejected ),
            FilterConst.CurrentYear => ja => ja.CreatedAt.Year >= today.Year && (ja.ApplyStatus == (int)JobApplicationEnum.NoResponse || ja.ApplyStatus == (int)JobApplicationEnum.Rejected ),
            FilterConst.All or null or "" => ja => true && (ja.ApplyStatus == (int)JobApplicationEnum.NoResponse || ja.ApplyStatus == (int)JobApplicationEnum.Rejected ),
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.JobApplicationRepository.Count(countQuery);
    }

    public async Task<int> GetCompaniesCount(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<CompanyProfile, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => cp => cp.IsDeleted == false && cp.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => cp => cp.IsDeleted == false && cp.CreatedAt.Year >= today.Year && cp.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => cp => cp.IsDeleted == false &&
                                           cp.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                                           cp.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => cp => cp.IsDeleted == false && cp.CreatedAt.Year >= today.Year,
            FilterConst.All or null or "" => cp => cp.IsDeleted == false && true,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.CompanyProfileRepository.Count(countQuery);
    }

    public async Task<int> GetUpcomingJobRoles()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        Expression<Func<CompanyJobListing, bool>> countQuery = jl => ((jl.CompanyVisit != null && jl.CompanyVisit.ApplicationEndDate >= today) || (jl.CompanyVisit != null && jl.CompanyVisit.ApplicationStartDate >= today)) && jl.IsDeleted == false;

        return await _unitOfWork.CompanyJobListingRepository.Count(countQuery);
    }

    public async Task<int> TotalStudentsRegistered(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<UserAccount, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ua => ua.RoleId == (int)RoleEnum.Student && ua.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => ua => ua.RoleId == (int)RoleEnum.Student && ua.CreatedAt.Year == today.Year && ua.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => ua => ua.RoleId == (int)RoleEnum.Student &&
                ua.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                ua.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => ua => ua.RoleId == (int)RoleEnum.Student && ua.CreatedAt.Year == today.Year,
            FilterConst.All or null or "" => ua => ua.RoleId == (int)RoleEnum.Student,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.AuthenticationRepository.Count(countQuery);
    }

    public async Task<int> TotalTPORegistered(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<UserAccount, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ua => ua.RoleId == (int)RoleEnum.TPO && ua.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => ua => ua.RoleId == (int)RoleEnum.TPO && ua.CreatedAt.Year == today.Year && ua.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => ua => ua.RoleId == (int)RoleEnum.TPO &&
                ua.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                ua.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => ua => ua.RoleId == (int)RoleEnum.TPO && ua.CreatedAt.Year == today.Year,
            FilterConst.All or null or "" => ua => ua.RoleId == (int)RoleEnum.TPO,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.AuthenticationRepository.Count(countQuery);
    }

    public async Task<int> TotalAdminsRegistered(string dateSelect)
    {
        DateTime today = DateTime.Today;

        Expression<Func<UserAccount, bool>> countQuery = dateSelect switch
        {
            FilterConst.Last7Days => ua => ua.RoleId == (int)RoleEnum.Admin && ua.CreatedAt >= today.AddDays(-7),
            FilterConst.CurrentMonth => ua => ua.RoleId == (int)RoleEnum.Admin && ua.CreatedAt.Year == today.Year && ua.CreatedAt.Month == today.Month,
            FilterConst.PreviousMonth => ua => ua.RoleId == (int)RoleEnum.Admin &&
                ua.CreatedAt.Year == (today.Month == 1 ? today.Year - 1 : today.Year) &&
                ua.CreatedAt.Month == (today.Month == 1 ? 12 : today.Month - 1),
            FilterConst.CurrentYear => ua => ua.RoleId == (int)RoleEnum.Admin && ua.CreatedAt.Year == today.Year,
            FilterConst.All or null or "" => ua => true,
            _ => throw new Exception(CustomExceptionConst.InvalidFilter)
        };

        return await _unitOfWork.AuthenticationRepository.Count(countQuery);
    }

    public async Task<List<AuditLogDTO>> GetAuditLogs()
    {
        (int totalCount, List<AuditLogDTO> audits) = await _unitOfWork.AuditLogRepository.GetListPaginated(
            al => true,
            al => new AuditLogDTO
            {
                Action = al.Action,
                ActionByEmail = al.ActionByEmail,
                Details = al.Details,
                EntityId = al.EntityId,
                EntityName = al.EntityName,
                Id = al.Id,
                Name = al.Name,
                Role = al.Role,
                Timestamp = al.Timestamp
            },
            al => al.Timestamp!,
            al => al.Timestamp!,
            false,
            1,
            100
        );

        return audits;
    }
}
