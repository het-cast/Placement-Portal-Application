namespace PlacementPortal.DAL.ViewModels;

public class JobApplicationDataDTO
{
  public int StudentId { get; set; }

  public DateTime AppliedDate { get; set; }

  public DateTime ModifiedDate { get; set; }

  public int JobListingId { get; set; }

  public int ApplyStatus { get; set; }

  public decimal MinPackageOffered { get ; set ; }

  public decimal MaxPackageOffered { get ; set ; }

  public string? JobProfile { get ; set ; }

  public string? JobDomain { get ; set ; }

  public string? CompanyName { get ; set ;}

  public string? StudentName { get ; set ; }

  public string? StudentEmail { get ; set ; }

  public string? StudentDepartment { get ; set ;}

  public string? ResumePath { get ; set ; }

  public string? ResumeFileName { get ; set ; }
}
