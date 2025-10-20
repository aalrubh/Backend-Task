using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class FactFinance
{
    public int FinanceKey { get; set; }

    public int DateKey { get; set; }

    public int OrganizationKey { get; set; }

    public int DepartmentGroupKey { get; set; }

    public int ScenarioKey { get; set; }

    public int AccountKey { get; set; }

    public double Amount { get; set; }

    public DateTime? Date { get; set; }

    public virtual DimAccount AccountKeyNavigation { get; set; } = null!;

    public virtual DimDate DateKeyNavigation { get; set; } = null!;

    public virtual DimDepartmentGroup DepartmentGroupKeyNavigation { get; set; } = null!;

    public virtual DimOrganization OrganizationKeyNavigation { get; set; } = null!;

    public virtual DimScenario ScenarioKeyNavigation { get; set; } = null!;
}
