namespace PlacementPortal.DAL.ViewModels;

public class JobListingCommonData
{
  public string? CompanyId { get ; set ; }

  public DateOnly ApplicationStartDate { get; set; }

  public DateOnly ApplicationEndDate { get; set; }

  public decimal? MinCgpaRequired { get; set; }

  public int? MaxBacklogsAllowed { get; set; }

  public string? SalaryUnit { get; set; } = "LPA";
}
