using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Rolee
{
    public int IdRole { get; set; }

    public string Rolee1 { get; set; } = null!;

    public virtual ICollection<Userr> Userrs { get; set; } = new List<Userr>();
}
