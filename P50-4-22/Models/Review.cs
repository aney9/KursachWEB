using System;
using System.Collections.Generic;

namespace P50_4_22.Models;

public partial class Review
{
    public int IdReview { get; set; }

    public int Rating { get; set; }

    public string ReviewText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int CatalogroductId { get; set; }

    public int UsersId { get; set; }

    public virtual CatalogProduct Catalogroduct { get; set; } = null!;

    public virtual User Users { get; set; } = null!;
}
