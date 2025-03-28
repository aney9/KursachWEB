using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Favorite
{
    public int IdFavorite { get; set; }

    public int UserId { get; set; }

    public int CatalogId { get; set; }

    public DateTime AddedAt { get; set; }

    public virtual CatalogProduct Catalog { get; set; } = null!;

    public virtual Userr User { get; set; } = null!;
}
