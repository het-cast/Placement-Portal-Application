using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class JobApplication
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public DateTime AppliedDate { get; set; }

    public DateTime ModifiedDate { get; set; }

    public int CompanyListingId { get; set; }

    public int ApplyStatus { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual CompanyJobListing CompanyListing { get; set; } = null!;

    public virtual UserAccount Student { get; set; } = null!;
}
