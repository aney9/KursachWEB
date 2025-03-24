using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Cart
{
    public int IdCart { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public string UserId { get; set; } = null!;

    public int CatalogId { get; set; }

    public virtual CatalogProduct Catalog { get; set; } = null!;
}
