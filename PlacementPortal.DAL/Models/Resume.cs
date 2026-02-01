using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class Resume
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public string ResumeName { get; set; } = null!;

    public string ResumeFileUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Comment { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual UserAccount Student { get; set; } = null!;
}
