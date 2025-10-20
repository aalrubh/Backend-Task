using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimProductCategory
{
    public int ProductCategoryKey { get; set; }

    public int? ProductCategoryAlternateKey { get; set; }

    public string EnglishProductCategoryName { get; set; } = null!;

    public string SpanishProductCategoryName { get; set; } = null!;

    public string FrenchProductCategoryName { get; set; } = null!;

    public virtual ICollection<DimProductSubcategory> DimProductSubcategories { get; set; } = new List<DimProductSubcategory>();
}
