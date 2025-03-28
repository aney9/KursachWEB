using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class CatalogProduct
{
    public int IdCatalogproducts { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal PriceOfProduct { get; set; }

    public string DescriptionProduct { get; set; } = null!;

    public string Img { get; set; } = null!;

    public int Quantity { get; set; }

    public int BrandsId { get; set; }

    public int CategoriesId { get; set; }

    public virtual Brand Brands { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Categorie Categories { get; set; } = null!;

    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
