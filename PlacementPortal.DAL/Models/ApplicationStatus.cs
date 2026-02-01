using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class ApplicationStatus
{
    public int Id { get; set; }

    public string StatusTitle { get; set; } = null!;
}
