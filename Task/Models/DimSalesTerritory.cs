using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimSalesTerritory
{
    public int SalesTerritoryKey { get; set; }

    public int? SalesTerritoryAlternateKey { get; set; }

    public string SalesTerritoryRegion { get; set; } = null!;

    public string SalesTerritoryCountry { get; set; } = null!;

    public string? SalesTerritoryGroup { get; set; }

    public byte[]? SalesTerritoryImage { get; set; }

    public virtual ICollection<DimEmployee> DimEmployees { get; set; } = new List<DimEmployee>();

    public virtual ICollection<DimGeography> DimGeographies { get; set; } = new List<DimGeography>();

    public virtual ICollection<FactInternetSale> FactInternetSales { get; set; } = new List<FactInternetSale>();

    public virtual ICollection<FactResellerSale> FactResellerSales { get; set; } = new List<FactResellerSale>();
}
