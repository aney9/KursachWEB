using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Brand
{
    public int IdBrands { get; set; }

    public string Brand1 { get; set; } = null!;

    public virtual ICollection<CatalogProduct> CatalogProducts { get; set; } = new List<CatalogProduct>();
}
