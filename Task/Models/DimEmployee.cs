using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class DimEmployee
{
    public int EmployeeKey { get; set; }

    public int? ParentEmployeeKey { get; set; }

    public string? EmployeeNationalIdalternateKey { get; set; }

    public string? ParentEmployeeNationalIdalternateKey { get; set; }

    public int? SalesTerritoryKey { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public bool NameStyle { get; set; }

    public string? Title { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? LoginId { get; set; }

    public string? EmailAddress { get; set; }

    public string? Phone { get; set; }

    public string? MaritalStatus { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public bool? SalariedFlag { get; set; }

    public string? Gender { get; set; }

    public byte? PayFrequency { get; set; }

    public decimal? BaseRate { get; set; }

    public short? VacationHours { get; set; }

    public short? SickLeaveHours { get; set; }

    public bool CurrentFlag { get; set; }

    public bool SalesPersonFlag { get; set; }

    public string? DepartmentName { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public byte[]? EmployeePhoto { get; set; }

    public virtual ICollection<FactResellerSale> FactResellerSales { get; set; } = new List<FactResellerSale>();

    public virtual ICollection<FactSalesQuotum> FactSalesQuota { get; set; } = new List<FactSalesQuotum>();

    public virtual ICollection<DimEmployee> InverseParentEmployeeKeyNavigation { get; set; } = new List<DimEmployee>();

    public virtual DimEmployee? ParentEmployeeKeyNavigation { get; set; }

    public virtual DimSalesTerritory? SalesTerritoryKeyNavigation { get; set; }
}
