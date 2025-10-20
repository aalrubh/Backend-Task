using AutoMapper;
using Dapper;
using SqlKata;
using MyApp.DTOs;

namespace MyApp.Data;

public class ProductRepository : IProductRepository
{
    private readonly ISqlServerConnectionProvider _connectionProvider;
    
    public ProductRepository(ISqlServerConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }
    
    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        using var connection = _connectionProvider.GetConnection();
        var query = new Query("Dbo.DimProduct").Select(
                "Dbo.DimProduct.EnglishProductName",
                "Dbo.DimProduct.Color",
                "Dbo.DimProduct.StandardCost",
                "Dbo.DimProduct.ListPrice",
                "Dbo.DimProduct.Size",
                "Dbo.DimProduct.Weight",
                "Dbo.DimProductCategory.EnglishProductCategoryName as ProductCategory",
                "Dbo.DimProductSubcategory.EnglishProductSubcategoryName as ProductSubcategory"
            )
            .Join("Dbo.DimProductSubcategory", "Dbo.DimProduct.ProductSubcategoryKey", "Dbo.DimProductSubcategory.ProductSubcategoryKey")
            .Join("Dbo.DimProductCategory", "Dbo.DimProductSubcategory.ProductCategoryKey", "Dbo.DimProductCategory.ProductCategoryKey");
        
        var compiler = new SqlKata.Compilers.SqlServerCompiler();
        var compiledQuery = compiler.Compile(query);
        var products = await connection.QueryAsync<ProductDto>(compiledQuery.Sql);
        return products.ToList();
    }
}