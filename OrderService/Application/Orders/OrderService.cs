using OrderService.Infrastructure.Clients;
using Serilog;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace OrderService.Application.Orders
{
    public class OrderService : IOrderService
    {       
        private readonly ProductClient _productClient;
        private readonly IDatabase _redis;

        public OrderService(ProductClient productClient, IConnectionMultiplexer muxer)
        {
            _productClient = productClient;
            _redis = muxer.GetDatabase();
        }


        /// <summary>
        /// Creates a new order using the specified order details.
        /// </summary>
        /// <param name="order">The order information to create. Must include a valid product identifier.</param>
        /// <returns>A result indicating the outcome of the operation. Returns a bad request result if the specified product does
        /// not exist; otherwise, returns a success result containing the order.</returns>
        public async Task<IResult> CreateOrder(Order order, CancellationToken cancellationToken = default)
        {  
            Log.Information("Making HTTP request to Product Service for ProductId: {ProductId}", order.ProductId);

            var response = await _productClient.GetProduct(order.ProductId);

            if (!response.IsSuccessStatusCode)
                return Results.BadRequest("Product does not exist");

            return Results.Ok(order);
        }

        public async Task<IResult> GetAllOrders(CancellationToken cancellationToken = default)
        {
            var orders = new List<Order>
            {
                new Order(1, 1, 2),
                new Order(2, 2, 3)
            };

            return Results.Ok(orders);
        }

        public async Task<IResult> GetOrder(int id, CancellationToken cancellationToken = default)
        {
            // tu by sa malo implementovat ziskavanie objednavky z databazy, ale pro jednoduchosť to nechame takto

            var keyName = $"Order_{id}";

            // zistenie, ci hodnota je v cache
            string? json = await _redis.StringGetAsync(keyName);
            if (string.IsNullOrEmpty(json))
            {

                // zatial mame napevno jednu hodnotu
                json = JsonSerializer.Serialize(new Order(id, 1, 2));
                
                var setTask = _redis.StringSetAsync(keyName, json);
                var expireTask = _redis.KeyExpireAsync(keyName, TimeSpan.FromSeconds(3600));

                await Task.WhenAll(setTask, expireTask);
            }
            
            var result = JsonSerializer.Deserialize<Order>(json);
           
            return Results.Ok(result);
        }
    }
}
