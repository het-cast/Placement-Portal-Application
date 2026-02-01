using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class CompanyProfile
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? CompanyType { get; set; }

    public string? CompanyWebsite { get; set; }

    public string? Description { get; set; }

    public string? Location { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<CompanyJobListing> CompanyJobListings { get; } = new List<CompanyJobListing>();

    public virtual ICollection<CompanyVisit> CompanyVisits { get; } = new List<CompanyVisit>();
}
