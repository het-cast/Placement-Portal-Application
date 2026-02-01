using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IRepository;
using PlacementPortal.DAL.Data;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Repository;

public class UnitOfWork : IUnitOfWork
{
    PlacementPortalDbContext _context;

    public UnitOfWork(PlacementPortalDbContext context)
    {
        _context = context;
    }

    public ICompanyProfileRepository CompanyProfileRepository => new CompanyProfileRepository(_context);

    public IAuthenticationRepository AuthenticationRepository => new AuthenticateRepository(_context);

    public IUserDetailsRepository UserDetailsRepository => new UserDetailsRepository(_context);

    public IDepartmentRepository DepartmentRepository => new DepartmentRepository(_context);

    public IStudentDetailRepository StudentDetailRepository => new StudentDetailRepository(_context);

    public ICompanyJobListingRepository CompanyJobListingRepository => new CompanyJobListingRepository(_context);

    public IResumeRepository ResumeRepository => new ResumeRepository(_context);

    public IJobApplicationRepository JobApplicationRepository => new JobApplicationRepository(_context);

    public ICompanyVisitRepository CompanyVisitRepository => new CompanyVisitRepository(_context);

    public IAuditLogRepository AuditLogRepository => new AuditLogRepository(_context);

    public INotificationRepository NotificationRepository => new NotificationRepository(_context);

    public INotificationMappingRepository NotificationMappingRepository => new NotificationMappingRepository(_context);

    public async Task<OperationStatusDTO> SaveChangesAsync()
    {
        OperationStatusDTO result = new();
        try
        {
            await _context.SaveChangesAsync();
            result.Success = true;
            result.Message = GeneralConst.SavedChangesInDb;

        }
        catch (Exception)
        {
            throw;
        }
        return result;
    }
}
