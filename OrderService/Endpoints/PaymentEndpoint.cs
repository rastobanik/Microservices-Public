using Microsoft.AspNetCore.Http.HttpResults;
using OrderService.Application.Common;
using OrderService.Application.Orders;
using OrderService.Application.Payments;

namespace OrderService.Endpoints
{
    public static class PaymentEndpoint
    {
        // kebyze chcem dalsi endpoint, ktory bude komunikovat s payment service, tak ho tu pridam a potom ho zavolam v program.cs
        public static void MapPaymentEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/Payment", CreatePayment);

            app.MapGet("/TypePayments", GetAllTypePayments);            
        }

        private static async Task<IResult> GetAllTypePayments(IPaymentService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {
            var result = await service.GetAllTypePayments(cancellationToken);

            return result.ToHttp();
        }

        private static async Task<IResult> CreatePayment(IPaymentService service, ILogger<Program> logger, CancellationToken cancellationToken = default)
        {

            var result = await service.CreatePayment(cancellationToken);

            return result.ToHttp();       
        }
    }
}
