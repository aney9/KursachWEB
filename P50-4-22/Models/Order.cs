using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Order
{
    public int IdOrders { get; set; }

    public int OrderNumber { get; set; }

    public DateOnly DateOrder { get; set; }

    public decimal Ordersum { get; set; }

    public int UsersId { get; set; }

    public virtual ICollection<PosOrder> PosOrders { get; set; } = new List<PosOrder>();

    public virtual User Users { get; set; } = null!;
}
