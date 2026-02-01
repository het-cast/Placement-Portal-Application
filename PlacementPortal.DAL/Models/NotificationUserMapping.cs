using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class NotificationUserMapping
{
    public int Id { get; set; }

    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public bool IsRead { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
