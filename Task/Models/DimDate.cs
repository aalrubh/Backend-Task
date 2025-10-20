using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimDate
{
    public int DateKey { get; set; }

    public DateOnly FullDateAlternateKey { get; set; }

    public byte DayNumberOfWeek { get; set; }

    public string EnglishDayNameOfWeek { get; set; } = null!;

    public string SpanishDayNameOfWeek { get; set; } = null!;

    public string FrenchDayNameOfWeek { get; set; } = null!;

    public byte DayNumberOfMonth { get; set; }

    public short DayNumberOfYear { get; set; }

    public byte WeekNumberOfYear { get; set; }

    public string EnglishMonthName { get; set; } = null!;

    public string SpanishMonthName { get; set; } = null!;

    public string FrenchMonthName { get; set; } = null!;

    public byte MonthNumberOfYear { get; set; }

    public byte CalendarQuarter { get; set; }

    public short CalendarYear { get; set; }

    public byte CalendarSemester { get; set; }

    public byte FiscalQuarter { get; set; }

    public short FiscalYear { get; set; }

    public byte FiscalSemester { get; set; }

    public virtual ICollection<FactCallCenter> FactCallCenters { get; set; } = new List<FactCallCenter>();

    public virtual ICollection<FactCurrencyRate> FactCurrencyRates { get; set; } = new List<FactCurrencyRate>();

    public virtual ICollection<FactInternetSale> FactInternetSaleDueDateKeyNavigations { get; set; } = new List<FactInternetSale>();

    public virtual ICollection<FactInternetSale> FactInternetSaleOrderDateKeyNavigations { get; set; } = new List<FactInternetSale>();

    public virtual ICollection<FactInternetSale> FactInternetSaleShipDateKeyNavigations { get; set; } = new List<FactInternetSale>();

    public virtual ICollection<FactProductInventory> FactProductInventories { get; set; } = new List<FactProductInventory>();

    public virtual ICollection<FactResellerSale> FactResellerSaleDueDateKeyNavigations { get; set; } = new List<FactResellerSale>();

    public virtual ICollection<FactResellerSale> FactResellerSaleOrderDateKeyNavigations { get; set; } = new List<FactResellerSale>();

    public virtual ICollection<FactResellerSale> FactResellerSaleShipDateKeyNavigations { get; set; } = new List<FactResellerSale>();

    public virtual ICollection<FactSalesQuotum> FactSalesQuota { get; set; } = new List<FactSalesQuotum>();

    public virtual ICollection<FactSurveyResponse> FactSurveyResponses { get; set; } = new List<FactSurveyResponse>();
}
