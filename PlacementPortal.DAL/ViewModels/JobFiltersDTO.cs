namespace PlacementPortal.DAL.ViewModels;

public class JobFiltersDTO
{
    public decimal MinCgpaRequired { get ; set ; }

    public int MaxBacklogsAllowed { get ; set ; } = -1;

    public decimal MinPackage { get ; set ; }

    public decimal MaxPackage { get ; set ; }

    public string? JobDomain { get ; set ; }

    public decimal StudentCGPA { get ; set ; }

    public int StudentBacklog { get ; set ; } = -1;

}
