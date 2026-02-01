using System;
using System.Collections.Generic;

namespace PlacementPortal.DAL.Models;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<UserAccount> UserAccounts { get; } = new List<UserAccount>();
}
