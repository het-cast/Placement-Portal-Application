namespace PlacementPortal.DAL.ViewModels;

public class JobListingOperationDTO
{
  public JobListingCommonData? JobListingCommon { get ; set ;}

  public List<JobListingsDTO>? JobListings { get ; set ;}  
}
