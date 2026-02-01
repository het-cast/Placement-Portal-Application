using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.IRepository;

public interface IUnitOfWork
{
    ICompanyProfileRepository CompanyProfileRepository { get ; }

    IAuthenticationRepository AuthenticationRepository { get ; }

    IUserDetailsRepository UserDetailsRepository { get ; }

    IDepartmentRepository DepartmentRepository { get ; }

    IStudentDetailRepository StudentDetailRepository { get ; }

    ICompanyJobListingRepository CompanyJobListingRepository { get ; }

    IResumeRepository ResumeRepository { get ;}

    IJobApplicationRepository JobApplicationRepository { get ; }

    ICompanyVisitRepository CompanyVisitRepository { get ; }

    IAuditLogRepository AuditLogRepository { get ; }

    INotificationRepository NotificationRepository { get ;}

    INotificationMappingRepository NotificationMappingRepository { get ;}

    Task<OperationStatusDTO> SaveChangesAsync();

}
