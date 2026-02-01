using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class StudentDetail
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public decimal SscPercentage { get; set; }

    public int SscPassingYear { get; set; }

    public decimal HscPercentage { get; set; }

    public short HscPassingYear { get; set; }

    public decimal? CpiDiploma { get; set; }

    public int? EnrollmentYear { get; set; }

    public int Branch { get; set; }

    public bool PlacedAlready { get; set; }

    public int? PendingBacklog { get; set; }

    public decimal? Cgpa { get; set; }

    public decimal? Cpi { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public long? EnrollmentNumber { get; set; }

    public string SscBoard { get; set; } = null!;

    public string HscBoard { get; set; } = null!;

    public virtual Department BranchNavigation { get; set; } = null!;

    public virtual UserAccount Student { get; set; } = null!;
}
