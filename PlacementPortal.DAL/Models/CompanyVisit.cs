using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class CompanyVisit
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public DateOnly ApplicationStartDate { get; set; }

    public DateOnly ApplicationEndDate { get; set; }

    public decimal? MinCgpaRequired { get; set; }

    public int? MaxBacklogsAllowed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual CompanyProfile Company { get; set; } = null!;

    public virtual ICollection<CompanyJobListing> CompanyJobListings { get; } = new List<CompanyJobListing>();
}
