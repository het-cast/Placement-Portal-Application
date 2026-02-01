namespace PlacementPortal.DAL.ViewModels;

public class CompanyProfileDTO
{
    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public string? CompanyWebsite { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }
}
