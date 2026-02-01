using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class CompanyJobListing
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public decimal? MinimumSalary { get; set; }

    public decimal? MaximumSalary { get; set; }

    public string? SalaryUnit { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? ModifiedBy { get; set; }

    public string? JobDomain { get; set; }

    public string? JobProfile { get; set; }

    public bool? IsDeleted { get; set; }

    public string? GeneratedKey { get; set; }

    public int? CompanyVisitId { get; set; }

    public virtual CompanyProfile Company { get; set; } = null!;

    public virtual CompanyVisit? CompanyVisit { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; } = new List<JobApplication>();

    public virtual ICollection<PlacedStudent> PlacedStudents { get; } = new List<PlacedStudent>();
}
