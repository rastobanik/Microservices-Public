using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductService.Application.Common;

namespace ProductService.Application.Products
{
    public class ProductService : IProductService
    {
        private static List<Product> _products = new List<Product>()
        {
             new(1, "Laptop", 1000),
                new(2, "Mouse", 20)
        };

        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductService> _logger;


        public ProductService(IMemoryCache memoryCache, ILogger<ProductService> logger) 
        {  
            _cache = memoryCache;
            _logger = logger;
        }

        public async Task<Result<Product>> CreateProduct(Product product, CancellationToken cancellationToken = default)
        {
            _products.Add(product);
            return Result<Product>.Ok(product);
        }

        public async Task<Result<List<Product>?>> GetAllProduct(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var products = await _cache.GetOrCreateAsync($"AllProducts_{page}_{pageSize}_CacheKey", async entry =>
            {                
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(30))
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                     .SetPriority(CacheItemPriority.High);               

                _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", $"AllProducts_{page}_{pageSize}_CacheKey");
                return _products;
            });

            products = products?.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return products is null
                ? Result<List<Product>?>.Fail("Not found")
                : Result<List<Product>?>.Ok(products);
        }

        public async Task<Result<List<Product>?>>  GetAllProducts(CancellationToken cancellationToken = default)
        {
            var products = await _cache.GetOrCreateAsync("AllProductsCacheKey", async entry =>
            {                
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(30))
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                     .SetPriority(CacheItemPriority.High);                 

                _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", "AllProductsCacheKey");
                return _products;
            });

            return products is null
                ? Result<List<Product>?>.Fail("Not found")
                : Result<List<Product>?>.Ok(products);
        }

        public async Task<Result<Product>> GetProduct(int id, CancellationToken cancellationToken = default)
        {
            var product = await _cache.GetOrCreateAsync($"Product_{id}_CacheKey", async entry =>
            {
                // pozriet sa na nastavenie velkosti cache
                entry.SetSlidingExpiration(TimeSpan.FromSeconds(30))
                     .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                     .SetPriority(CacheItemPriority.High);                

                _logger.LogInformation("Cache miss for key: {CacheKey}. Fetching from database.", $"Product_{id}_CacheKey");
                return _products.FirstOrDefault(p => p.Id == id);
            });

            return product is null
                ? Result<Product>.Fail("Not found")
                : Result<Product>.Ok(product);
        }      
    }
}
