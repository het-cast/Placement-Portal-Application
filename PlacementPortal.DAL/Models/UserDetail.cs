using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class UserDetail
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public long? Phone { get; set; }

    public string? Address { get; set; }

    public bool? IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual UserAccount User { get; set; } = null!;
}
