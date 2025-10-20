using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimSalesReason
{
    public int SalesReasonKey { get; set; }

    public int SalesReasonAlternateKey { get; set; }

    public string SalesReasonName { get; set; } = null!;

    public string SalesReasonReasonType { get; set; } = null!;

    public virtual ICollection<FactInternetSale> FactInternetSales { get; set; } = new List<FactInternetSale>();
}
