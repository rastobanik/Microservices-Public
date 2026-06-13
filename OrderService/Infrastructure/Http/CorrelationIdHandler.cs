namespace OrderService.Infrastructure.Http
{
    public class CorrelationIdHandler : DelegatingHandler
    {

        private readonly IHttpContextAccessor _accessor;

        public CorrelationIdHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = _accessor.HttpContext?.Items["CorrelationId"]?.ToString();

            if (!string.IsNullOrEmpty(correlationId))
            {
                request.Headers.Add("X-Correlation-ID", correlationId);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
