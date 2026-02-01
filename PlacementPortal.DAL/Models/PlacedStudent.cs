using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class PlacedStudent
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int CompanyId { get; set; }

    public DateOnly? PlacedDate { get; set; }

    public DateOnly? JoiningDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual CompanyJobListing Company { get; set; } = null!;

    public virtual UserAccount Student { get; set; } = null!;
}
