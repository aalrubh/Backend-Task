using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class VTimeSeries
{
    public string? ModelRegion { get; set; }

    public int? TimeIndex { get; set; }

    public int? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public short CalendarYear { get; set; }

    public byte Month { get; set; }

    public DateTime? ReportingDate { get; set; }
}
