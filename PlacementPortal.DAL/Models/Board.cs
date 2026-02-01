using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class Board
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool? IsDeleted { get; set; }
}
