using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DatabaseLog
{
    public int DatabaseLogId { get; set; }

    public DateTime PostTime { get; set; }

    public string DatabaseUser { get; set; } = null!;

    public string Event { get; set; } = null!;

    public string? Schema { get; set; }

    public string? Object { get; set; }

    public string Tsql { get; set; } = null!;

    public string XmlEvent { get; set; } = null!;
}
