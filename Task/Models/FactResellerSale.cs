using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class FactResellerSale
{
    public int ProductKey { get; set; }

    public int OrderDateKey { get; set; }

    public int DueDateKey { get; set; }

    public int ShipDateKey { get; set; }

    public int ResellerKey { get; set; }

    public int EmployeeKey { get; set; }

    public int PromotionKey { get; set; }

    public int CurrencyKey { get; set; }

    public int SalesTerritoryKey { get; set; }

    public string SalesOrderNumber { get; set; } = null!;

    public byte SalesOrderLineNumber { get; set; }

    public byte? RevisionNumber { get; set; }

    public short? OrderQuantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? ExtendedAmount { get; set; }

    public double? UnitPriceDiscountPct { get; set; }

    public double? DiscountAmount { get; set; }

    public decimal? ProductStandardCost { get; set; }

    public decimal? TotalProductCost { get; set; }

    public decimal? SalesAmount { get; set; }

    public decimal? TaxAmt { get; set; }

    public decimal? Freight { get; set; }

    public string? CarrierTrackingNumber { get; set; }

    public string? CustomerPonumber { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ShipDate { get; set; }

    public virtual DimCurrency CurrencyKeyNavigation { get; set; } = null!;

    public virtual DimDate DueDateKeyNavigation { get; set; } = null!;

    public virtual DimEmployee EmployeeKeyNavigation { get; set; } = null!;

    public virtual DimDate OrderDateKeyNavigation { get; set; } = null!;

    public virtual DimProduct ProductKeyNavigation { get; set; } = null!;

    public virtual DimPromotion PromotionKeyNavigation { get; set; } = null!;

    public virtual DimReseller ResellerKeyNavigation { get; set; } = null!;

    public virtual DimSalesTerritory SalesTerritoryKeyNavigation { get; set; } = null!;

    public virtual DimDate ShipDateKeyNavigation { get; set; } = null!;
}
