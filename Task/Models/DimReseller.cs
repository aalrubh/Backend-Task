using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimReseller
{
    public int ResellerKey { get; set; }

    public int? GeographyKey { get; set; }

    public string? ResellerAlternateKey { get; set; }

    public string? Phone { get; set; }

    public string BusinessType { get; set; } = null!;

    public string ResellerName { get; set; } = null!;

    public int? NumberEmployees { get; set; }

    public string? OrderFrequency { get; set; }

    public byte? OrderMonth { get; set; }

    public int? FirstOrderYear { get; set; }

    public int? LastOrderYear { get; set; }

    public string? ProductLine { get; set; }

    public string? AddressLine1 { get; set; }

    public string? AddressLine2 { get; set; }

    public decimal? AnnualSales { get; set; }

    public string? BankName { get; set; }

    public byte? MinPaymentType { get; set; }

    public decimal? MinPaymentAmount { get; set; }

    public decimal? AnnualRevenue { get; set; }

    public int? YearOpened { get; set; }

    public virtual ICollection<FactResellerSale> FactResellerSales { get; set; } = new List<FactResellerSale>();

    public virtual DimGeography? GeographyKeyNavigation { get; set; }
}
