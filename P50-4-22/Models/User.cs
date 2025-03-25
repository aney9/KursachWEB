using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class User
{
    public int IdUsers { get; set; }

    public string Loginvhod { get; set; } = null!;

    public string Loginpassword { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int RolesId { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role Roles { get; set; } = null!;
}
