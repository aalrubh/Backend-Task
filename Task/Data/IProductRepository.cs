using MyApp.DTOs;

namespace MyApp.Data;

public interface IProductRepository
{
    public Task<List<ProductDto>> GetAllProductsAsync();

}