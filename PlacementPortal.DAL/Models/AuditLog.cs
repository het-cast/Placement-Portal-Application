using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class AuditLog
{
    public int Id { get; set; }

    public string ActionByEmail { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Action { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string EntityId { get; set; } = null!;

    public string? Name { get; set; }

    public string? Details { get; set; }

    public DateTime? Timestamp { get; set; }
}
