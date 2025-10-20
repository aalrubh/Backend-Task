using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class FactCurrencyRate
{
    public int CurrencyKey { get; set; }

    public int DateKey { get; set; }

    public double AverageRate { get; set; }

    public double EndOfDayRate { get; set; }

    public DateTime? Date { get; set; }

    public virtual DimCurrency CurrencyKeyNavigation { get; set; } = null!;

    public virtual DimDate DateKeyNavigation { get; set; } = null!;
}
