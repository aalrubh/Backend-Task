using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimProductSubcategory
{
    public int ProductSubcategoryKey { get; set; }

    public int? ProductSubcategoryAlternateKey { get; set; }

    public string EnglishProductSubcategoryName { get; set; } = null!;

    public string SpanishProductSubcategoryName { get; set; } = null!;

    public string FrenchProductSubcategoryName { get; set; } = null!;

    public int? ProductCategoryKey { get; set; }

    public virtual ICollection<DimProduct> DimProducts { get; set; } = new List<DimProduct>();

    public virtual DimProductCategory? ProductCategoryKeyNavigation { get; set; }
}
