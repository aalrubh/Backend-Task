namespace MyApp.DTOs;

public class ProductDto
{
        public string EnglishProductName { get; set; } = null!;
        public string? Color { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        public string? Size { get; set; }
        public decimal? Weight { get; set; }
        public string ProductCategory { get; set; } = null!;
        public string ProductSubcategory { get; set; } = null!;
}