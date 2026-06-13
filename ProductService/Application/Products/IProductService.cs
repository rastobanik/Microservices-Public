using ProductService.Application.Common;

namespace ProductService.Application.Products
{
    public interface IProductService
    {
        Task<Result<Product>> CreateProduct(Product product, CancellationToken cancellationToken = default);

        Task<Result<List<Product>?>> GetAllProducts(CancellationToken cancellationToken = default);

        Task<Result<Product?>> GetProduct(int id, CancellationToken cancellationToken = default);

        Task<Result<List<Product>?>> GetAllProduct(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
