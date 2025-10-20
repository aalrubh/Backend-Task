using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimOrganization
{
    public int OrganizationKey { get; set; }

    public int? ParentOrganizationKey { get; set; }

    public string? PercentageOfOwnership { get; set; }

    public string? OrganizationName { get; set; }

    public int? CurrencyKey { get; set; }

    public virtual DimCurrency? CurrencyKeyNavigation { get; set; }

    public virtual ICollection<DimOrganization> InverseParentOrganizationKeyNavigation { get; set; } = new List<DimOrganization>();

    public virtual DimOrganization? ParentOrganizationKeyNavigation { get; set; }
}
