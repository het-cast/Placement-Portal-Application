namespace PlacementPortal.DAL.ViewModels;

public class JobListingsDTO
{
  public int JobListingId;

  public string? UniqueGeneratedKey { get ; set ; }

  public string? JobDomain { get ; set ; }

  public string? JobProfie { get ; set ; }

  public int PositionId { get; set; }

  public decimal? MinimumSalary { get ; set ; }

  public decimal? MaximumSalary { get ; set ; }

  
}
