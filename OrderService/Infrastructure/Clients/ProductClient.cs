namespace OrderService.Infrastructure.Clients
{
    public class ProductClient
    {
        /// <summary>
        /// Connection to microservices Products
        /// </summary>
        private readonly HttpClient _client;

        public ProductClient(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Sends an asynchronous HTTP GET request to retrieve the product with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message for
        /// the requested product.</returns>
        public Task<HttpResponseMessage> GetProduct(int id)
            => _client.GetAsync($"products/{id}");
    }
}
