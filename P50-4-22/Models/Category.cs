using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Category
{
    public int IdCategories { get; set; }

    public string Categories { get; set; } = null!;

    public virtual ICollection<CatalogProduct> CatalogProducts { get; set; } = new List<CatalogProduct>();
}
