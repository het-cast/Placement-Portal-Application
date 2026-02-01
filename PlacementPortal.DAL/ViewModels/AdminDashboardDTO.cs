namespace PlacementPortal.DAL.ViewModels;

public class AdminDashboardDTO
{
  public int TotalActiveStudents { get ; set ; }

  public int TotalActiveTPOs { get ; set ; }

  // public int TotalCompanyProfilesActive { get ; set ; }

  public int TotalJobListingsActive { get ; set ; }

  public int TotalStudentsHired { get ; set ; }

  public int RejectedOrNoReponseApplicationsCount { get ; set ; }

  public List<AuditLogDTO>? AuditLogs { get ; set ; }

}
