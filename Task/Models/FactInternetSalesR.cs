using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class FactInternetSalesR
{
    public int ProductKey { get; set; }

    public int OrderDateKey { get; set; }

    public int DueDateKey { get; set; }

    public int ShipDateKey { get; set; }

    public int CustomerKey { get; set; }

    public int PromotionKey { get; set; }

    public int CurrencyKey { get; set; }

    public int SalesTerritoryKey { get; set; }

    public string SalesOrderNumber { get; set; } = null!;

    public byte SalesOrderLineNumber { get; set; }

    public byte RevisionNumber { get; set; }

    public short OrderQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal ExtendedAmount { get; set; }

    public double UnitPriceDiscountPct { get; set; }

    public double DiscountAmount { get; set; }

    public decimal ProductStandardCost { get; set; }

    public decimal TotalProductCost { get; set; }

    public decimal SalesAmount { get; set; }

    public decimal TaxAmt { get; set; }

    public decimal Freight { get; set; }

    public string? CarrierTrackingNumber { get; set; }

    public string? CustomerPonumber { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? ShipDate { get; set; }
}
