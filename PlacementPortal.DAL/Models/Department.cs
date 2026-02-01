using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class Department
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public virtual ICollection<StudentDetail> StudentDetails { get; } = new List<StudentDetail>();
}
