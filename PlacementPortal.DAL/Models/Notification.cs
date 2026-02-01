using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class Notification
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? RedirectUrl { get; set; }

    public virtual ICollection<NotificationUserMapping> NotificationUserMappings { get; } = new List<NotificationUserMapping>();
}
