using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Role
{
    public int IdRole { get; set; }

    public string Rolee { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
