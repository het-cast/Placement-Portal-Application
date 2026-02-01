namespace PlacementPortal.DAL.ViewModels;

public class JobApplicationDTO
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public DateTime AppliedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int JobListingId { get; set; }

    public int ApplyStatus { get; set; }
}
