using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Products;
using Shared.Configuration.RateLimit;

namespace ProductService.Endpoints
{
    public static class ProductEndpoint
    {   
        public static void MapProductEndpoints(this IEndpointRouteBuilder app)
        {  
            // takto je pridany limit fixed na endpoint
            app.MapGet("/products", GetAllProducts).RequireRateLimiting(RateLimitPolicies.Fixed);

            app.MapGet("/productsPage", GetAllProductsPage).RequireRateLimiting(RateLimitPolicies.Fixed);

            // tu sa uplatnuje len globalny limiter
            app.MapGet("/products/{id}", GetProduct);

            app.MapPost("/product", CreateProduct);
        }

        private static async Task<IResult> GetProduct(int id, [FromServices] IProductService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Endpoint called for product {Id}", id);

            // volanie sluzby na najdenie objednavky s danym id
            var result = await service.GetProduct(id, cancellationToken);
            return result.ToHttp();
        }

        private static async Task<IResult> GetAllProducts([FromServices] IProductService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Endpoint called");
            // volanie sluzby na ziskanie vsetkych objednavok
            var result = await service.GetAllProducts(cancellationToken);
            return result.ToHttp();
        }

        private static async Task<IResult> CreateProduct(Product product, [FromServices] IProductService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Endpoint called");
            var result = await service.CreateProduct(product, cancellationToken);
            return result.ToHttp();
        }

        private static async Task<IResult> GetAllProductsPage([FromQuery] int? page, [FromQuery] int? pageSize, [FromServices] IProductService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            page ??= 1;
            pageSize ??= 10;

            var result = await service.GetAllProduct(page.Value, pageSize.Value, cancellationToken = default);
            return result.ToHttp();
        }

        //private static async Task<IResult> GetAllProductsPage([AsParameters]PaggingDTO paggingDTO, [FromServices] IProductService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        //{
        //    if (paggingDTO == null)
        //    {
        //        paggingDTO = new PaggingDTO() { Page = 1, PageSize = 10 }   ;
        //    }

        //    paggingDTO.Page ??= 1;
        //    paggingDTO.PageSize ??= 10;

        //    var result = await service.GetAllProduct(paggingDTO.Page.Value, paggingDTO.PageSize.Value, cancellationToken = default);
        //    return result.ToHttp();
        //}       
    }
}
