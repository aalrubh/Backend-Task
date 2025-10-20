namespace MyApp.Models;

public class ProductJoinModel
{
    public string EnglishProductName { get; set; } = null!;
    public string? Color { get; set; }
    public decimal StandardCost { get; set; }
    public decimal ListPrice { get; set; }
    public string? Size { get; set; }
    public decimal? Weight { get; set; }
    public string EnglishProductCategoryName { get; set; } = null!;
    public string EnglishProductSubcategoryName { get; set; } = null!;
}