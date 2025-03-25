using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class PosOrder
{
    public int IdPosorder { get; set; }

    public int CountProduct { get; set; }

    public int OrdersId { get; set; }

    public int ProductsId { get; set; }

    public virtual Order Orders { get; set; } = null!;

    public virtual CatalogProduct Products { get; set; } = null!;
}
