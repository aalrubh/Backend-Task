using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimCurrency
{
    public int CurrencyKey { get; set; }

    public string CurrencyAlternateKey { get; set; } = null!;

    public string CurrencyName { get; set; } = null!;

    public virtual ICollection<DimOrganization> DimOrganizations { get; set; } = new List<DimOrganization>();

    public virtual ICollection<FactCurrencyRate> FactCurrencyRates { get; set; } = new List<FactCurrencyRate>();

    public virtual ICollection<FactInternetSale> FactInternetSales { get; set; } = new List<FactInternetSale>();

    public virtual ICollection<FactResellerSale> FactResellerSales { get; set; } = new List<FactResellerSale>();
}
