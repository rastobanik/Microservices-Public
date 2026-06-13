using Newtonsoft.Json.Linq;
using OrderService.Application.Common;
using Serilog.Core;

namespace OrderService.Application.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ILogger<PaymentService> logger)
        {
            _logger = logger;
        }


        public Task<Result<Payment>> CreatePayment(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<Payment>> GetAllTypePayments(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Starting to do slow work");        

            await Task.Delay(10_000, cancellationToken);

            var message = "Finished slow delay of 10 seconds.";
            _logger.LogInformation(message);

            return Result<Payment>.Ok(new Payment(10, 125, 25)); // alebo reálne dáta
        }
    }
}
