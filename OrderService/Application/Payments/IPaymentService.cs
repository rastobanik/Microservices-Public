using OrderService.Application.Common;

namespace OrderService.Application.Payments
{
    public interface IPaymentService
    {
        Task<Result<Payment>> GetAllTypePayments(CancellationToken cancellationToken = default);
        Task<Result<Payment>> CreatePayment(CancellationToken cancellationToken = default);
    }
}
