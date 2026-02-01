namespace PlacementPortal.DAL.ViewModels;

public class JobListingDataDTO
{
    public int CompanyId { get ; set ; }

    public List<CompanyJobListingDTO>? JobListings { get ; set ; } 
}
