using OrderService.Application.Orders;
using Shared.Configuration.RateLimit;

namespace OrderService.Endpoints
{
    /// <summary>
    /// toto je trieda vykonavajuca mapovanie a je vytiahnuta z program.cs
    /// </summary>
    public static class OrderEndpoints
    {
        public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/orders", CreateOrder);

            app.MapGet("/orders/{id}", GetOrder);

            // pridanie okrem globalneho limitu aj limitu fixed
            app.MapGet("/orders", GetAllOrders).RequireRateLimiting(RateLimitPolicies.Fixed); ;
        }
                
        private static async Task<IResult> CreateOrder(
            Order order,
            IOrderService service)
        {
            return await service.CreateOrder(order);
        }

        private static async Task<IResult> GetOrder(int id, IOrderService service, CancellationToken cancellationToken = default)
        {
            // volanie sluzby na najdenie objednavky s danym id
            return await service.GetOrder(id);
        }

        private static async Task<IResult> GetAllOrders(IOrderService service, CancellationToken cancellationToken = default)
        {
            // volanie sluzby na ziskanie vsetkych objednavok
            return await service.GetAllOrders();
        }   
    }
}
