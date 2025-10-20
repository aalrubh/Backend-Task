using Microsoft.AspNetCore.Mvc;
using MyApp.Data;

namespace MyApp.Controllers;

[ApiController]
public class ProductController : ControllerBase
{
    
    private readonly IProductRepository _productRepository;

    public ProductController(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    [HttpGet("api/products")]
    public async Task<IActionResult> GetProducts()
    {
        return Ok(await _productRepository.GetAllProductsAsync());
    }
}