using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class JobTitle
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
