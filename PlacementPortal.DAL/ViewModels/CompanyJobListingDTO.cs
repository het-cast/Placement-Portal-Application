namespace PlacementPortal.DAL.ViewModels;

public class CompanyJobListingDTO
{
    public int JobListingId;

    public int CompanyId { get; set; }

    public string? CompanyIdString { get ; set; }

    public string? UniqueGeneratedKey { get ; set ; }

    public string? JobDomain { get ; set ; }

    public string? JobProfie { get ; set ; }

    public int PositionId { get; set; }

    public DateOnly ApplicationStartDate { get; set; }

    public DateOnly ApplicationEndDate { get; set; }

    public decimal? MinCgpaRequired { get; set; }  

    public int? MaxBacklogsAllowed { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public string? SalaryUnit { get; set; }
}
