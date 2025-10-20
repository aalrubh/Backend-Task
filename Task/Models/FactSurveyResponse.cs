using System;
using System.Collections.Generic;

namespace MyApp.Models;

public partial class FactSurveyResponse
{
    public int SurveyResponseKey { get; set; }

    public int DateKey { get; set; }

    public int CustomerKey { get; set; }

    public int ProductCategoryKey { get; set; }

    public string EnglishProductCategoryName { get; set; } = null!;

    public int ProductSubcategoryKey { get; set; }

    public string EnglishProductSubcategoryName { get; set; } = null!;

    public DateTime? Date { get; set; }

    public virtual DimCustomer CustomerKeyNavigation { get; set; } = null!;

    public virtual DimDate DateKeyNavigation { get; set; } = null!;
}
