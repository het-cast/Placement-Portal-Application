using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class UserAccount
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsInitialLogin { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsTwoFactorsEnabled { get; set; }

    public string? MfaSecretKey { get; set; }

    public virtual ICollection<JobApplication> JobApplications { get; } = new List<JobApplication>();

    public virtual ICollection<NotificationUserMapping> NotificationUserMappings { get; } = new List<NotificationUserMapping>();

    public virtual ICollection<PlacedStudent> PlacedStudents { get; } = new List<PlacedStudent>();

    public virtual ICollection<Resume> Resumes { get; } = new List<Resume>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<StudentDetail> StudentDetails { get; } = new List<StudentDetail>();

    public virtual ICollection<UserDetail> UserDetails { get; } = new List<UserDetail>();
}
