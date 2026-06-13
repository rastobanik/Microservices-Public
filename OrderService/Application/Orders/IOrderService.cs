namespace OrderService.Application.Orders
{
    public interface IOrderService
    {
        Task<IResult> CreateOrder(Order order, CancellationToken cancellationToken = default);

        Task<IResult> GetOrder(int id, CancellationToken cancellationToken = default);

        Task<IResult> GetAllOrders(CancellationToken cancellationToken = default);
    }
}
